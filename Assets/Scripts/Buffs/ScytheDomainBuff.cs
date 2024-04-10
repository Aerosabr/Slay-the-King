using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheDomainBuff : Buff
{
    public int attackIncrease;
    public int cdrIncrease;
    public float msIncrease;
    public float asIncrease;

    public ScytheDomainBuff(int attack, int cdr, float ms, float atk, float duration, string source, GameObject entity)
    {
        attackIncrease = attack;
        cdrIncrease = cdr;
        msIncrease = ms;
        asIncrease = atk;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(attackIncrease, 0);
        Entity.GetComponent<Entity>().CDR += cdrIncrease;
        Entity.GetComponent<Entity>().changeMovementSpeed(0, msIncrease);
        Entity.GetComponent<Entity>().changeAttackSpeed(0, asIncrease);
    }

    public override void RemoveEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(-attackIncrease, 0);
        Entity.GetComponent<Entity>().CDR -= cdrIncrease;
        Entity.GetComponent<Entity>().changeMovementSpeed(0, -msIncrease);
        Entity.GetComponent<Entity>().changeAttackSpeed(0, -asIncrease);
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        return false;
    }
}
