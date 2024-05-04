using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceStun : Buff
{

    public MaceStun(float duration, string source, GameObject entity)
    {
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().isStunned = true;
    }

    public override void RemoveEffect()
    {
        Entity.GetComponent<Entity>().isStunned = false;
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        Entity.GetComponent<Entity>().isStunned = true;
        return false;
    }
}
