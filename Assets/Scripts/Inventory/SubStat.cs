using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SubStat
{
	public string name;
	public int value;

    public SubStat(string Name, int Value)
    {
        name = Name;
        value = Value;
    }

    public void increaseValue()
    {
        value++;
    }
}
