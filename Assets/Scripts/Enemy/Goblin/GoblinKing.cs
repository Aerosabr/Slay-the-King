using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;

public class GoblinKing : Entity, IDamageable, IEffectable
{
    public Rigidbody2D rb;
    public float speed = 1.0f;
    public GameObject player;
    public EnemySpriteController ESC;
    public Transform attackHitBoxPos;
    public bool Attackable;
    public LayerMask Damageable;
    public GameObject TargetCircle;
    public GameObject Wave;
    public GameObject childCollider;
    public int A3Counter = 0;
    public bool Charging;

    //AI Pathfinding
    public Seeker seeker;
    public Path path;
    public int currentWaypoint = 0;
    public float nextWaypointDistance = 1f;
    public float repathRate = 1f;
    private float lastRepathTime = float.NegativeInfinity;
    private AIPath aiPath;
    //Cooldowns
    public float A1, A1CD; //Slams ground, rupture wave
    public float A2, A2CD; //Slams ground, rupture around king
    public float A3, A3CD; //Repeatedly slams location
    public float A4, A4CD; //Charge at target
    public float A5, A5CD; //Jumps into location

    void Awake()
    {
        player = PlayerManager.instance.Players[0].gameObject;
        GetComponent<AIDestinationSetter>().target = player.transform;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        ESC = GetComponent<EnemySpriteController>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        Damageable = LayerMask.GetMask("Player");
        TargetCircle = Resources.Load<GameObject>("Prefabs/Hitboxes/TargetCircle");
        Wave = Resources.Load<GameObject>("Prefabs/Projectiles/GoblinWave");
        childCollider = transform.GetChild(1).gameObject;
        A1CD = 8f;
        A2CD = 12f;
        A3CD = 15f;
        A4CD = 18f;
        A5CD = 20f;

        A1 = A1CD;
        A2 = A2CD;
        A3 = A3CD;
        A4 = A4CD;
        A5 = A5CD;
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        RequestPath();
        loadStats();
    }

    private void loadStats()
    {
        int multiplier = GameManager.instance.Floor;
        baseMaxHealth *= multiplier;
        maxHealth *= multiplier;
        currentHealth *= multiplier;
        baseAttack *= multiplier;
        Attack *= multiplier;
    }

    void RequestPath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath((transform.position - new Vector3(0.07f, 0.8f)), player.transform.position, OnPathComplete);
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        IncrementCDs();
        if (currentHealth <= 0)
            return;

        if (isStunned || !BattleStage.instance.Active)
        {
            if (currentHealth > 0)
            {
                ESC.PlayAnimation("Idle");
                rb.velocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezePosition;
            }
        }
        else if (!isStunned && currentHealth > 0 && isMovable)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (!CheckAttacks())
            {
                if (Time.time >= lastRepathTime + repathRate)
                {
                    lastRepathTime = Time.time;
                    RequestPath();
                }

                if (path == null || path.vectorPath == null || currentWaypoint >= path.vectorPath.Count)
                    return;

                ProcessDirection((Vector2)path.vectorPath[currentWaypoint] + new Vector2(0.07f, 0.8f));
                rb.MovePosition(rb.position + (((Vector2)path.vectorPath[currentWaypoint] + new Vector2(0.07f, 0.8f) - rb.position).normalized * speed * Time.fixedDeltaTime));

                if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
                    currentWaypoint++;

                ESC.PlayAnimation("Run");
            }
        }

        if (Buffs.Count > 0)
            HandleBuff();     
    }

    public void IncrementCDs()
    {
        if (A1 > 0)
            A1 -= Time.deltaTime;

        if (A2 > 0)
            A2 -= Time.deltaTime;

        if (A3 > 0)
            A3 -= Time.deltaTime;

        if (A4 > 0)
            A4 -= Time.deltaTime;

        if (A5 > 0)
            A5 -= Time.deltaTime;
    }

    public void ProcessDirection(Vector2 target)
    {
        float angle = Mathf.Atan2(target.y - (transform.position.y - 1.4f), target.x - (transform.position.x - .16f));
        ESC.currentDir = new Vector2(Mathf.Cos(angle) * 1f, Mathf.Sin(angle) * 1f);
    }

    public bool CheckAttacks()
    {
        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (A5 <= 0 && dist > 7)
        {
            StartCoroutine(Ability5Cast());
            return true;
        }
        else if (A4 <= 0 && dist > 4 && dist < 8)
        {
            Ability4Cast();
            return true;
        }
        else if (A3 <= 0 && dist < 3.5)
        {
            StartCoroutine(Ability3Cast());
            return true;
        }
        else if (A2 <= 0 && dist < 4)
        {
            Ability2Cast();
            return true;
        }
        else if (A1 <= 0)
        {
            Ability1Cast();
            return true;
        }
        else if (dist < 3)
        {
            BasicAttackCast();
            return true;
        }
        return false;
    }

    #region IEffectable Components
    public Dictionary<string, Buff> Buffs = new Dictionary<string, Buff>();

    public void ApplyBuff(Buff buff)
    {
        if (Buffs.ContainsKey(buff.Source))
            RemoveBuff(Buffs[buff.Source]);

        Buffs.Add(buff.Source, buff);
        Buffs[buff.Source].ApplyEffect();
    }

    public void RemoveBuff(Buff buff)
    {
        buff.RemoveEffect();
        Buffs.Remove(buff.Source);
    }

    public void HandleBuff()
    {
        List<string> keys = Buffs.Keys.ToList();
        foreach (string key in keys)
        {
            if (Buffs[key].HandleEffect())
                RemoveBuff(Buffs[key]);
        }
    }
    #endregion

    #region IDamageable Components
    public int Damaged(int amount, Vector3 origin, float kb)
    {
        amount = Mathf.Abs(amount);
        int damage = (amount - Defense > 0) ? amount - Defense : 1;

        if (currentHealth - damage > 0)
            currentHealth -= damage;
        else
        {
            damage = currentHealth;
            currentHealth = 0;
        }

        if (kbResistance < kb)
            StartCoroutine(KnockCoroutine(origin, kb - kbResistance));
        DamagePopup.Create(rb.transform.position, Mathf.Abs(damage), false);

        if (currentHealth <= 0)
        {
            ESC.PlayAnimation("Death");
            aiPath.canMove = false;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(GetComponent<BoxCollider2D>());
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            StartCoroutine(Death(2f));
        }

        return damage;
    }

    public int trueDamaged(int amount)
    {
        int damage;
        if (amount > currentHealth)
        {
            damage = currentHealth;
            currentHealth = 0;
        }
        else
        {
            damage = currentHealth - amount;
            currentHealth -= amount;
        }

        return damage;
    }

    public IEnumerator KnockCoroutine(Vector3 origin, float kb)
    {
        Vector2 force = ((transform.position - new Vector3(0.25f, 0.2f)) - origin).normalized * kb;
        isMovable = false;
        rb.velocity = force;
        yield return new WaitForSeconds(.3f);
        isMovable = true;
        if (currentHealth > 0)
            rb.velocity = new Vector2();
    }

    public int Healed(int amount)
    {
        return 0;
    }

    public IEnumerator Death(float time)
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        DropLoot();
        BattleStage.instance.enemiesKilled++;
    }
    #endregion

    private void DropLoot()
    {
        //50 Gold, 25% equipment rate -> Accessories
        ItemCreation IC = ItemCreation.instance;
        IC.CreateItem(ItemCreation.instance.itemDict["Gold"], 50, transform);
        if (Random.Range(1, 101) <= 25)
        {
            List<string> substatNames = new List<string> { "Health", "Attack", "Defense", "Dexterity", "Cooldown Reduction", "Attack Speed", "Luck" };
            string equipment = IC.GenerateRandomEquipment(3);
            List<SubStat> subStats = IC.GenerateSubstats(2);
            SubStat mainStat;
            switch (equipment)
            {
                case "Ring":
                    mainStat = new SubStat(substatNames[Random.Range(0, substatNames.Count)], PlayerManager.instance.GetAverageLevel() + 2);
                    break;
                case "Amulet":
                    mainStat = new SubStat(substatNames[Random.Range(0, substatNames.Count)], PlayerManager.instance.GetAverageLevel() + 2);
                    break;
                default:
                    mainStat = new SubStat("Attack", PlayerManager.instance.GetAverageLevel() + 2);
                    break;
            }

            IC.CreateEquipment(IC.equipmentDict[equipment], mainStat, subStats, transform);
        }
    }

    #region Basic Attack
    public void BasicAttackCast()
    {
        ProcessDirection(player.transform.position); 
        isMovable = false;
        aiPath.canMove = false;
        ESC.PlayAnimation("Slash");
        float angle = Mathf.Atan2(player.transform.position.y - (transform.position.y - 1.4f), player.transform.position.x - (transform.position.x - .16f));
        attackHitBoxPos.localPosition = new Vector2((Mathf.Cos(angle)) - .16f, (Mathf.Sin(angle)) - 1f);
        GameObject tc = Instantiate(TargetCircle, attackHitBoxPos.position, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(2f, .3f);
    }

    public IEnumerator BasicAttack()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 2f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 1);
        }
        yield return new WaitForSeconds(.15f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
        aiPath.canMove = true;
    }
    #endregion

    #region Ability 1
    public void Ability1Cast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
        aiPath.canMove = false;
        ESC.PlayAnimation("WaveSlam");
    }

    public IEnumerator Ability1()
    {
        GameObject wave = Instantiate(Wave, new Vector2(transform.position.x - .16f, transform.position.y - 1.4f), transform.rotation);
        wave.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(ESC.currentDir.y, ESC.currentDir.x) * Mathf.Rad2Deg);
        wave.GetComponent<Slashwave>().EditSlashwave(4f, 20);
        wave.GetComponent<Rigidbody2D>().velocity = 8f * ESC.currentDir;
        yield return new WaitForSeconds(.2f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
        aiPath.canMove = true;
        A1 = A1CD;
    }
    #endregion

    #region Ability 2
    public void Ability2Cast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
        aiPath.canMove = false;
        ESC.PlayAnimation("SurroundSlam");
        attackHitBoxPos.position = new Vector2(transform.position.x - .16f, transform.position.y - 1.4f);
        GameObject tc = Instantiate(TargetCircle, attackHitBoxPos.position, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(3f, .9f);
    }

    public IEnumerator Ability2()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 3f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 5);
        }
        yield return new WaitForSeconds(.25f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
        aiPath.canMove = true;
        A2 = A2CD;
    }

    public IEnumerator KnockCoroutine(Rigidbody2D enemy)
    {
        
        Vector2 force = (enemy.transform.position - transform.position).normalized * 5f;
        Debug.Log(force);
        enemy.GetComponent<Entity>().isMovable = false;
        enemy.GetComponent<PlayerSpriteController>().Movable = false;
        enemy.velocity = force;
        yield return new WaitForSeconds(.3f);
        enemy.GetComponent<Entity>().isMovable = true;
        enemy.velocity = new Vector2();
        enemy.GetComponent<PlayerSpriteController>().Movable = true;
        
    }
    #endregion

    #region Ability 3
    public IEnumerator Ability3Cast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
        aiPath.canMove = false;
        ESC.PlayAnimation("InitialSpam");
        yield return new WaitForSeconds(1.5f);
        ProcessDirection(player.transform.position);
        ESC.PlayAnimation("SpamSlam");
        float angle = Mathf.Atan2(player.transform.position.y - (transform.position.y - 1.4f), player.transform.position.x - (transform.position.x - .16f));
        attackHitBoxPos.localPosition = new Vector2((Mathf.Cos(angle)) - .16f, (Mathf.Sin(angle)) - 1f);
        GameObject tc = Instantiate(TargetCircle, attackHitBoxPos.position, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(2f, .15f);
    }

    public IEnumerator Ability3()
    {
        A3Counter++;

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 2f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 0);
        }

        if (A3Counter == 6)
        {
            yield return new WaitForSeconds(.15f);
            ESC.PlayAnimation("Idle");
            yield return new WaitForSeconds(.5f);
            isMovable = true;
            aiPath.canMove = true;
            A3 = A3CD;
            A3Counter = 0;
        }
    }
    #endregion

    #region Ability 4
    public void Ability4Cast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
        aiPath.canMove = false;
        Charging = true;
        ESC.PlayAnimation("Charge");
        float angle = Mathf.Atan2(player.transform.position.y - (transform.position.y - 1.4f), player.transform.position.x - (transform.position.x - .16f));
        GetComponent<Rigidbody2D>().velocity = new Vector2((Mathf.Cos(angle)) * 10, (Mathf.Sin(angle)) * 10);
    }

    public IEnumerator Ability4()
    {
        if (Charging)
        {
            yield return new WaitForSeconds(.05f);
            Charging = false;
            ESC.PlayAnimation("Idle");
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            yield return new WaitForSeconds(.5f);
            isMovable = true;
            aiPath.canMove = true;
        }
        A4 = A4CD;
    }

    public void EnemyCharged(Collision2D collision)
    {
        collision.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 5);
        StartCoroutine(KnockCoroutine(collision.gameObject.GetComponent<Rigidbody2D>()));
    }
    #endregion

    #region Ability 5
    public IEnumerator Ability5Cast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
        aiPath.canMove = false;
        ESC.PlayAnimation("Jump");
        yield return new WaitForSeconds(1.5f);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        childCollider.SetActive(false);
        yield return new WaitForSeconds(1.2f);
        GameObject tc = Instantiate(TargetCircle, player.transform.position, Quaternion.identity);
        attackHitBoxPos.localPosition = new Vector2(-0.08f, -.7f);
        tc.GetComponent<TargetCircle>().InitiateTarget(4f, .8f);
        transform.position = new Vector2(player.transform.position.x + 0.16f, player.transform.position.y + 1.4f);
        yield return new WaitForSeconds(.8f);
        GetComponent<SpriteRenderer>().enabled = true;
        ESC.PlayAnimation("LandSlam");
    }

    public IEnumerator Ability5()
    {
        childCollider.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().simulated = true;
        
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 4f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.gameObject.tag == "Player" && collider.GetType().ToString() == "UnityEngine.BoxCollider2D")
                collider.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 5);         
        }
        
        yield return new WaitForSeconds(.5f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
        aiPath.canMove = true;
        A5 = A5CD;
    }
    #endregion
}
