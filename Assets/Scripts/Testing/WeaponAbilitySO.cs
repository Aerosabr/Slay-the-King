using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponAbilitySO : ScriptableObject
{
    public string[] abilityNames;
    public Animator[] animatorBody;

    public string idle;
    public string run;
}
