using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Class : ScriptableObject
{
    public string Name; //Class Name
    public string Description; //Class description
    public Sprite ClassIcon;
    public Sprite PassiveIcon; 
    public Sprite AttackIcon;
    public Sprite BlockIcon;
    public Sprite AbilityIcon;
    public Sprite UltimateIcon;

    public bool passiveActive; //Passive active state
    public int UltimateCD; //Ultimate Cooldown

    public virtual void ReduceCooldown()
    {
        if (UltimateCD > 0)
            UltimateCD--;
    }

    public abstract void Passive();
    public abstract List<string> PassiveDescription();

    public abstract void Attack();
    public abstract List<string> AttackDescription();

    public abstract void Block();
    public abstract List<string> BlockDescription();

    public abstract void Ability();
    public abstract List<string> AbilityDescription();

    public abstract void Ultimate();
    public abstract List<string> UltimateDescription();
}
