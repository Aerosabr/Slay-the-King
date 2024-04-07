using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff 
{
    public string Source;
    public float maxDuration;
    public float activeDuration;
    public GameObject Entity;

    public abstract void ApplyEffect();
    public abstract void RemoveEffect();
    public abstract bool HandleEffect();
}
