using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Class : MonoBehaviour
{
    //Main class that all character classes derive from
    public string Name;
    public bool kitChangeable = true;

    public abstract bool checkEquippable(string weapon);

    public abstract void changeWeapon();

    public abstract void unequipWeapon(string weapon);

    public abstract void equipCurrent();
}
