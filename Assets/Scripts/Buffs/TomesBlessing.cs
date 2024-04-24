using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomesBlessing : Buff
{
    public int attackIncrease;
    public int defIncrease;
    public int dexIncrease;

    public TomesBlessing(int attack, int def, int dex, float duration, string source, GameObject entity)
    {
        attackIncrease = attack;
        defIncrease = def;
        dexIncrease = dex;
        maxDuration = duration;
        Source = source;
        Entity = entity;
    }

    public override void ApplyEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(attackIncrease, 0);
        Entity.GetComponent<Entity>().changeDefense(defIncrease, 0);
        Entity.GetComponent<Entity>().changeDexterity(dexIncrease, 0);
    }

    public override void RemoveEffect()
    {
        Entity.GetComponent<Entity>().changeAttack(-attackIncrease, 0);
        Entity.GetComponent<Entity>().changeDefense(-defIncrease, 0);
        Entity.GetComponent<Entity>().changeDexterity(-dexIncrease, 0);
    }

    public override bool HandleEffect()
    {
        activeDuration += Time.deltaTime;
        if (activeDuration >= maxDuration)
            return true;
        return false;
    }
}
