using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseDefense : Buff
{
    public int flatDefIncrease;
    public float percentDefIncrease;

    public IncreaseDefense(int floatDef, float percentDef, float duration, string source, GameObject entity)
    {
        flatDefIncrease = floatDef;
        percentDefIncrease = percentDef;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().changeDefense(flatDefIncrease, percentDefIncrease);
    }

    public override void RemoveEffect()
    {
        Entity.GetComponent<Entity>().changeDefense(-flatDefIncrease, -percentDefIncrease);
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        return false;
    }
}
