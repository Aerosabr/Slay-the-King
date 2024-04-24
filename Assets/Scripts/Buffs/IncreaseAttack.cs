using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAttack : Buff
{
    public int flatAttackIncrease;
    public float percentAttackIncrease;

    public IncreaseAttack(int floatAtk, float percentAtk, float duration, string source, GameObject entity)
    {
        flatAttackIncrease = floatAtk;
        percentAttackIncrease = percentAtk;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(flatAttackIncrease, percentAttackIncrease);
    }

    public override void RemoveEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(-flatAttackIncrease, -percentAttackIncrease);
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        return false;
    }
}
