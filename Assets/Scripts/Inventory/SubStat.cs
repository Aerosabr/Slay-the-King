using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
