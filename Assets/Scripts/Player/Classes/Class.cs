using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Class : ScriptableObject
{
    //Main class that all character classes derive from
    public string Name; 
    public string Description; 
    public Sprite ClassIcon;
    public abstract Class createClass(Class classObject);

    //Passive
    public Sprite PassiveIcon;
    public float PassiveCD;
    public bool passiveActive; 
    public abstract void Passive();
    public abstract List<string> PassiveDescription();

    //Basic Attack
    public Sprite AttackIcon;
    public float AttackCD;
    public bool attackActive;
    public abstract void Attack();
    public abstract List<string> AttackDescription();

    //Ability 1
    public Sprite Ability1Icon;
    public float Ability1CD;
    public bool ability1Active;
    public abstract void Ability1();
    public abstract List<string> Ability1Description();

    //Ability 2
    public Sprite Ability2Icon;
    public float Ability2CD;
    public bool ability2Active;
    public abstract void Ability2();
    public abstract List<string> Ability2Description();

    //Ultimate
    public Sprite UltimateIcon;
    public float UltimateCD;
    public bool ultimateActive;
    public abstract void Ultimate();
    public abstract List<string> UltimateDescription();
    
}
