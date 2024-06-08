using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;
public class BombGoblin : Entity, IDamageable, IEffectable
{
    public Rigidbody2D rb;
    public float speed = 4.0f;
    public GameObject player;
    public EnemySpriteController ESC;
    public Transform attackHitBoxPos;
    public bool Attackable;
    public LayerMask Damageable;
    public bool Killed;

    //AI Pathfinding
    public Seeker seeker;
    public Path path;
    public int currentWaypoint = 0;
    public float nextWaypointDistance = 1f;
    public float repathRate = 1f;
    private float lastRepathTime = float.NegativeInfinity;
    private AIPath aiPath;

    void Awake()
    {
        player = PlayerManager.instance.Players[0].gameObject;
        GetComponent<AIDestinationSetter>().target = player.transform;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        ESC = GetComponent<EnemySpriteController>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        Damageable = LayerMask.GetMask("Player");
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        RequestPath();
    }

    void RequestPath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath((transform.position - new Vector3(0.05f, 0.4f)), player.transform.position, OnPathComplete);
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
        if (isStunned || !BattleStage.instance.Active)
        {
            ESC.PlayAnimation("Idle");
            rb.velocity = Vector2.zero;
        } 
        else if (!isStunned && currentHealth > 0)
        {
            if (currentHealth > 0 && isMovable)
            {
                if (Time.time >= lastRepathTime + repathRate)
                {
                    lastRepathTime = Time.time;
                    RequestPath();
                }

                if (path == null || path.vectorPath == null || currentWaypoint >= path.vectorPath.Count)
                    return;

                ProcessDirection((Vector2)path.vectorPath[currentWaypoint] + new Vector2(0.05f, 0.4f));
                rb.MovePosition(rb.position + (((Vector2)path.vectorPath[currentWaypoint] + new Vector2(0.05f, 0.4f) - rb.position).normalized * speed * Time.fixedDeltaTime));

                if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
                    currentWaypoint++;

                ESC.isMoving = true;
                ESC.isAttacking = false;
                ESC.PlayAnimation("Explosive");
            }
            else if (Attackable && !ESC.isAttacking)
            {
                rb.velocity = Vector2.zero;
                ESC.isMoving = false;
                Attacking();
            }
        }

        if (Buffs.Count > 0)
            HandleBuff();
    }

    public void ProcessDirection(Vector2 target)
    {
        float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x);
        ESC.currentDir = new Vector2(Mathf.Cos(angle) * 1f, Mathf.Sin(angle) * 1f);
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
            isMovable = false;
            ESC.PlayAnimation("Death");
            aiPath.canMove = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(rb);
            Destroy(GetComponent<BoxCollider2D>());
            Destroy(GetComponent<CircleCollider2D>());
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
        Vector2 force = ((transform.position - new Vector3(0.05f, 0.4f)) - origin).normalized * kb;
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
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        DropLoot();
        if (!Killed)
        {
            BattleStage.instance.enemiesKilled++;
            Killed = true;
        }
    }
    #endregion

    private void DropLoot()
    {
        //1 Gold, 5% equipment rate -> Weapon/Armor
        ItemCreation IC = ItemCreation.instance;
        IC.CreateItem(ItemCreation.instance.itemDict["Gold"], 1, transform);
        if (Random.Range(1, 101) < 101)
        {
            string equipment = IC.GenerateRandomEquipment(Random.Range(1, 3));
            List<SubStat> subStats = IC.GenerateSubstats(0);
            SubStat mainStat;
            switch (equipment)
            {
                case "Helmet":
                    mainStat = new SubStat("Health", PlayerManager.instance.GetAverageLevel());
                    break;
                case "Chestplate":
                    mainStat = new SubStat("Defense", PlayerManager.instance.GetAverageLevel());
                    break;
                case "Leggings":
                    mainStat = new SubStat("Dexterity", PlayerManager.instance.GetAverageLevel());
                    break;
                default:
                    mainStat = new SubStat("Attack", PlayerManager.instance.GetAverageLevel());
                    break;
            }

            IC.CreateEquipment(IC.equipmentDict[equipment], mainStat, subStats, transform);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 5);
            if (!Killed)
            {
                BattleStage.instance.enemiesKilled++;
                Killed = true;
            }
            Destroy(gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isMovable = true;
            Attackable = false;
        }
    }

    public void Attacking()
    {
        ESC.isAttacking = true;
        ESC.PlayAnimation("Shoot");
    }

    public Vector2 MapPoint(Vector2 point, float radius)
    {
        float angle = Mathf.Atan2(point.y, point.x);
        return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }

    public IEnumerator BasicAttack()
    {
        yield return new WaitForSeconds(.1f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        ESC.isAttacking = false;
    }
}
