using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseDamageTaken : Buff
{
    public float DamageIncrease;
    public int amtChanged;

    public IncreaseDamageTaken(float damageIncrease, float duration, string source, GameObject entity)
    {
        DamageIncrease = damageIncrease;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        amtChanged = (int)(Entity.GetComponent<Entity>().baseDefense * DamageIncrease) / 100;
        Entity.GetComponent<Entity>().Defense -= amtChanged;
    }

    public override void RemoveEffect()
    {
        Entity.GetComponent<Entity>().Defense += amtChanged;
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        return false;
    }
}
