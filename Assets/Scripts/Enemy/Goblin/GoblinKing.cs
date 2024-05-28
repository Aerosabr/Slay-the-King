using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    //Cooldowns
    public float A1, A1CD; //Slams ground, rupture wave
    public float A2, A2CD; //Slams ground, rupture around king
    public float A3, A3CD; //Repeatedly slams location
    public float A4, A4CD; //Charge at target
    public float A5, A5CD; //Jumps into location

    void Awake()
    {
        player = GameObject.Find("PlayerManager").transform.GetChild(0).transform.GetChild(0).gameObject;
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
    }

    void Update()
    {
        IncrementCDs();

        if (isStunned || !BattleStage.instance.Active)
            ESC.PlayAnimation("Idle");
        else if (!isStunned && currentHealth > 0 && isMovable)
        {
            if (!CheckAttacks())
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
                ProcessDirection(player.transform.position);
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
    public int Damaged(int amount)
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
        DamagePopup.Create(rb.transform.position, Mathf.Abs(damage), false);

        if (currentHealth <= 0)
        {
            ESC.PlayAnimation("Death");
            Destroy(rb);
            Destroy(GetComponent<BoxCollider2D>());
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

    public int Healed(int amount)
    {
        return 0;
    }

    public IEnumerator Death(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        Instantiate(Resources.Load<GameObject>("Prefabs/Gold"), transform.position, Quaternion.identity);
        BattleStage.instance.enemiesKilled++;
    }
    #endregion

    #region Basic Attack
    public void BasicAttackCast()
    {
        ProcessDirection(player.transform.position); 
        isMovable = false;
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
            if (collider.transform.position.x - transform.position.x >= 0)
                collider.gameObject.GetComponent<IDamageable>().Damaged(Attack);
            else
                collider.gameObject.GetComponent<IDamageable>().Damaged(-Attack);
        }
        yield return new WaitForSeconds(.15f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
    }
    #endregion

    #region Ability 1
    public void Ability1Cast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
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
        A1 = A1CD;
    }
    #endregion

    #region Ability 2
    public void Ability2Cast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
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
            if (collider.transform.position.x - transform.position.x >= 0)
                collider.gameObject.GetComponent<IDamageable>().Damaged(Attack);
            else
                collider.gameObject.GetComponent<IDamageable>().Damaged(-Attack);
            if (collider.gameObject.GetComponent<Entity>().currentHealth > 0)
                StartCoroutine(KnockCoroutine(collider.GetComponent<Rigidbody2D>()));
        }
        yield return new WaitForSeconds(.25f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
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
            if (collider.transform.position.x - transform.position.x >= 0)
                collider.gameObject.GetComponent<IDamageable>().Damaged(Attack);
            else
                collider.gameObject.GetComponent<IDamageable>().Damaged(-Attack);
        }

        if (A3Counter == 6)
        {
            yield return new WaitForSeconds(.15f);
            ESC.PlayAnimation("Idle");
            yield return new WaitForSeconds(.5f);
            isMovable = true;
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
        }
        A4 = A4CD;
    }

    public void EnemyCharged(Collision2D collision)
    {
        if (collision.transform.position.x - transform.position.x >= 0)
            collision.gameObject.GetComponent<IDamageable>().Damaged(Attack);
        else
            collision.gameObject.GetComponent<IDamageable>().Damaged(-Attack);
        StartCoroutine(KnockCoroutine(collision.gameObject.GetComponent<Rigidbody2D>()));
    }
    #endregion

    #region Ability 5
    public IEnumerator Ability5Cast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
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
            Debug.Log(collider.name);
            if (collider.transform.position.x - transform.position.x >= 0)
                collider.gameObject.GetComponent<IDamageable>().Damaged(Attack);
            else
                collider.gameObject.GetComponent<IDamageable>().Damaged(-Attack);
        }
        
        yield return new WaitForSeconds(.5f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
        A5 = A5CD;
    }
    #endregion
}
