using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;

public class ClubHoblin : Entity, IDamageable, IEffectable
{
    public Rigidbody2D rb;
    public float speed = 1.0f;
    public GameObject player;
    public EnemySpriteController ESC;
    public Transform attackHitBoxPos;
    public LayerMask Damageable;
    public GameObject TargetCircle;

    //AI Pathfinding
    public Seeker seeker;
    public Path path;
    public int currentWaypoint = 0;
    public float nextWaypointDistance = 1f;
    public float repathRate = 1f;
    private float lastRepathTime = float.NegativeInfinity;
    private AIPath aiPath;

    //Cooldowns
    public float Ability, AbilityCD; 

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
        AbilityCD = 8f;

        Ability = AbilityCD;
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
            seeker.StartPath((transform.position - new Vector3(0.03f, 0.3f)), player.transform.position, OnPathComplete);
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
        if (currentHealth <= 0)
            return;

        if (Ability > 0)
            Ability -= Time.deltaTime;
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

                ProcessDirection((Vector2)path.vectorPath[currentWaypoint] + new Vector2(0.03f, 0.3f));
                rb.MovePosition(rb.position + (((Vector2)path.vectorPath[currentWaypoint] + new Vector2(0.03f, 0.3f) - rb.position).normalized * speed * Time.fixedDeltaTime));

                if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
                    currentWaypoint++;

                ESC.PlayAnimation("Run");
            }
        }

        if (Buffs.Count > 0)
            HandleBuff();
    }

    public void ProcessDirection(Vector2 target)
    {
        float angle = Mathf.Atan2(target.y - (transform.position.y - 0.3f), target.x - (transform.position.x - .03f));
        ESC.currentDir = new Vector2(Mathf.Cos(angle) * 1f, Mathf.Sin(angle) * 1f);
    }

    public bool CheckAttacks()
    {
        float dist = Vector2.Distance(transform.position - new Vector3(-0.03f, -0.3f), player.transform.position);

        if (Ability <= 0 && dist < 4)
        {
            AbilityCast();
            return true;
        }
        else if (dist < 2f)
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
        DamagePopup.Create(rb.transform.position, damage, false);

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
        Vector2 force = ((transform.position - new Vector3(0.03f, 0.3f)) - origin).normalized * kb;
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
        //5 Gold, 10% equipment rate -> Weapon/Armor
        ItemCreation IC = ItemCreation.instance;
        IC.CreateItem(ItemCreation.instance.itemDict["Gold"], 5, transform);
        if (Random.Range(1, 101) <= 10)
        {
            string equipment = IC.GenerateRandomEquipment(Random.Range(1, 3));
            List<SubStat> subStats = IC.GenerateSubstats(1);
            SubStat mainStat;
            switch (equipment)
            {
                case "Helmet":
                    mainStat = new SubStat("Health", PlayerManager.instance.GetAverageLevel() + 1);
                    break;
                case "Chestplate":
                    mainStat = new SubStat("Defense", PlayerManager.instance.GetAverageLevel() + 1);
                    break;
                case "Leggings":
                    mainStat = new SubStat("Dexterity", PlayerManager.instance.GetAverageLevel() + 1);
                    break;
                default:
                    mainStat = new SubStat("Attack", PlayerManager.instance.GetAverageLevel() + 1);
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
        float angle = Mathf.Atan2(player.transform.position.y - (transform.position.y - .3f), player.transform.position.x - (transform.position.x - .03f));
        attackHitBoxPos.localPosition = new Vector2((Mathf.Cos(angle) - .03f), (Mathf.Sin(angle) - .3f));
        GameObject tc = Instantiate(TargetCircle, attackHitBoxPos.position, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(1.5f, .3f);
    }

    public IEnumerator BasicAttack()
    {
        Debug.Log(attackHitBoxPos.position);
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 1.5f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 5);
        }
        yield return new WaitForSeconds(.15f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
        aiPath.canMove = true;
    }
    #endregion

    #region Ability 1
    public void AbilityCast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
        aiPath.canMove = false;
        ESC.PlayAnimation("Slam");
        float angle = Mathf.Atan2(player.transform.position.y - (transform.position.y - .3f), player.transform.position.x - (transform.position.x - .03f));
        attackHitBoxPos.localPosition = new Vector2((Mathf.Cos(angle) - .03f), (Mathf.Sin(angle) - .3f));
        GameObject tc = Instantiate(TargetCircle, attackHitBoxPos.position, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(2f, .3f);
    }

    public IEnumerator AbilityActivate()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 2f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 5);
            if (collider.gameObject.GetComponent<Entity>().currentHealth > 0)
                collider.gameObject.GetComponent<IEffectable>().ApplyBuff(new MaceStun(2f, "Club Hoblin - Ability", collider.gameObject));
        }
        yield return new WaitForSeconds(.15f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
        aiPath.canMove = true;
        Ability = AbilityCD;
    }
    #endregion
}
