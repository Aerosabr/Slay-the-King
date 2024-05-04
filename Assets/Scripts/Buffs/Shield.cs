using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buff
{
    public int shieldAmount;

    public Shield(int shieldAmt, float duration, string source, GameObject entity)
    {
        shieldAmount = shieldAmt;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().Shield += shieldAmount;
    }

    public override void RemoveEffect()
    {
        if (Entity.GetComponent<Entity>().Shield - shieldAmount <= 0)
            Entity.GetComponent<Entity>().Shield = 0;
        else
            Entity.GetComponent<Entity>().Shield -= shieldAmount;
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        return false;
    }
}