using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceUltimate : Buff
{
    public int shieldAmount;

    public MaceUltimate(int shieldAmt, float duration, string source, GameObject entity)
    {
        shieldAmount = shieldAmt;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().Shield += shieldAmount;
        Entity.GetComponent<Entity>().ccImmune = true;
    }

    public override void RemoveEffect()
    {      
        if (Entity.GetComponent<Entity>().Shield - shieldAmount <= 0)
            Entity.GetComponent<Entity>().Shield = 0;
        else
            Entity.GetComponent<Entity>().Shield -= shieldAmount;

        Entity.GetComponent<Entity>().ccImmune = false;
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        Entity.GetComponent<Entity>().ccImmune = true;
        return false;
    }
}
