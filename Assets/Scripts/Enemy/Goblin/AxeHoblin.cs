using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class AxeHoblin : Entity, IDamageable, IEffectable
{
    public Rigidbody2D rb;
    public float speed = 1.0f;
    public GameObject player;
    public EnemySpriteController ESC;
    public Transform attackHitBoxPos;
    public LayerMask Damageable;
    public GameObject TargetCircle;

    //Cooldowns
    public float Ability, AbilityCD; //


    void Awake()
    {
        player = GameObject.Find("PlayerManager").transform.GetChild(0).transform.GetChild(0).gameObject;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        ESC = GetComponent<EnemySpriteController>();
        attackHitBoxPos = transform.Find("AttackHitbox");
        Damageable = LayerMask.GetMask("Player");
        TargetCircle = Resources.Load<GameObject>("Prefabs/Hitboxes/TargetCircle");
        AbilityCD = 8f;

        Ability = AbilityCD;
    }

    void Update()
    {
        if (Ability > 0)
            Ability -= Time.deltaTime;
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

    public void ProcessDirection(Vector2 target)
    {
        float angle = Mathf.Atan2(target.y - (transform.position.y - 0.3f), target.x - (transform.position.x - .03f));
        ESC.currentDir = new Vector2(Mathf.Cos(angle) * 1f, Mathf.Sin(angle) * 1f);
    }

    public bool CheckAttacks()
    {
        float dist = Vector2.Distance(transform.position - new Vector3(-0.04f, -0.4f), player.transform.position);

        if (Ability <= 0 && dist < 4)
        {
            AbilityCast();
            return true;
        }
        else if (dist < 1.5f)
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
        float angle = Mathf.Atan2(player.transform.position.y - (transform.position.y - 0.3f), player.transform.position.x - (transform.position.x - .03f));
        attackHitBoxPos.localPosition = new Vector2(Mathf.Cos(angle) - .03f, Mathf.Sin(angle) - 0.3f);
        GameObject tc = Instantiate(TargetCircle, attackHitBoxPos.position, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(1.5f, .3f);
    }

    public IEnumerator BasicAttack()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 1.5f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 5);
        }
        yield return new WaitForSeconds(.15f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
    }
    #endregion

    #region Ability 1
    public void AbilityCast()
    {
        ProcessDirection(player.transform.position);
        isMovable = false;
        ESC.PlayAnimation("Spin");
        attackHitBoxPos.position = new Vector2(transform.position.x - .03f, transform.position.y - .3f);
        GameObject tc = Instantiate(TargetCircle, attackHitBoxPos.position, Quaternion.identity);
        tc.GetComponent<TargetCircle>().InitiateTarget(2f, .5f);
    }

    public IEnumerator AbilityActivate()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position, 2f, Damageable);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.GetComponent<IDamageable>().Damaged(Attack, transform.position, 5);
        }
        yield return new WaitForSeconds(.1f);
        ESC.PlayAnimation("Idle");
        yield return new WaitForSeconds(.5f);
        isMovable = true;
        Ability = AbilityCD;
    }
    #endregion
}
