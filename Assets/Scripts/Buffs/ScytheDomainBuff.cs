using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheDomainBuff : Buff
{
    public int attackIncrease;
    public int cdrIncrease;
    public float msIncrease;

    public ScytheDomainBuff(int attack, int cdr, float ms, float duration, string source, GameObject entity)
    {
        attackIncrease = attack;
        cdrIncrease = cdr;
        msIncrease = ms;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(attackIncrease, 0);
        Entity.GetComponent<Entity>().CDR += cdrIncrease;
        Entity.GetComponent<Entity>().changeMovementSpeed(0, msIncrease);
    }

    public override void RemoveEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(-attackIncrease, 0);
        Entity.GetComponent<Entity>().CDR -= cdrIncrease;
        Entity.GetComponent<Entity>().changeMovementSpeed(0, -msIncrease);
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        return false;
    }
}
