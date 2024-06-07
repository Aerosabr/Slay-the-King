using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;

public class SwordGoblin : Entity, IDamageable, IEffectable
{
    public Rigidbody2D rb;
    public float speed = 1.0f;
    public GameObject player;
    public EnemySpriteController ESC;
    public Transform attackHitBoxPos;
    public bool Attackable;
    public LayerMask Damageable;

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
            seeker.StartPath((transform.position - new Vector3(0.04f, 0.4f)), player.transform.position, OnPathComplete);
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
        else if (!isStunned && currentHealth > 0 && isMovable)
        {
            if (!CheckAttacks())
            {
                if (Time.time >= lastRepathTime + repathRate)
                {
                    lastRepathTime = Time.time;
                    RequestPath();
                }

                if (path == null || path.vectorPath == null || currentWaypoint >= path.vectorPath.Count)
                    return;

                ProcessDirection((Vector2)path.vectorPath[currentWaypoint] + new Vector2(0.04f, 0.4f));
                rb.MovePosition(rb.position + (((Vector2)path.vectorPath[currentWaypoint] + new Vector2(0.04f, 0.4f) - rb.position).normalized * speed * Time.fixedDeltaTime));

                if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
                    currentWaypoint++;

                ESC.PlayAnimation("Run");
            }
            else
                rb.velocity = Vector2.zero;
        }

        if (Buffs.Count > 0)
            HandleBuff();
    }

    public void ProcessDirection(Vector2 target)
    {
        float angle = Mathf.Atan2(target.y - (transform.position.y - 0.4f), target.x - (transform.position.x - 0.04f));
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
            Destroy(rb);
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
        Vector2 force = ((transform.position - new Vector3(0.04f, 0.4f)) - origin).normalized * kb;
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
    #endregion

    public IEnumerator Death(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        Instantiate(Resources.Load<GameObject>("Prefabs/Gold"), transform.position, Quaternion.identity);
        BattleStage.instance.enemiesKilled++;
    }

    public bool CheckAttacks()
    {
        float dist = Vector2.Distance(transform.position - new Vector3(-0.04f, -0.4f), player.transform.position);
        if (dist < 1.5f)
        {
            Attacking();
            return true;
        }
        return false;
    }

    public void Attacking()
    {
        isMovable = false;
        aiPath.canMove = false;
        ESC.PlayAnimation("DSlash");
    }

    public IEnumerator BasicAttack()
    {
        float angle = Mathf.Atan2(player.transform.position.y - (transform.position.y - 0.4f), player.transform.position.x - (transform.position.x - 0.04f));
        attackHitBoxPos.localPosition = new Vector2(Mathf.Cos(angle) - .03f, Mathf.Sin(angle) - .3f);
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 1f, Damageable);
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
}
