using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeEnrage : Buff
{
    public int attackIncrease;
    public float defIncrease;
    public float asIncrease;

    public AxeEnrage(int attack, float def, float atk, float duration, string source, GameObject entity)
    {
        attackIncrease = attack;
        defIncrease = def; 
        asIncrease = atk;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(attackIncrease, 0);
        Entity.GetComponent<Entity>().changeDefense(0, defIncrease);
        Entity.GetComponent<Entity>().changeAttackSpeed(0, asIncrease);
    }

    public override void RemoveEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(-attackIncrease, 0);
        Entity.GetComponent<Entity>().changeDefense(0, -defIncrease);
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
