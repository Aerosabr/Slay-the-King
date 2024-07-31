using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Class
{
    public override bool checkEquippable(string weapon)
    {
        if (weapon == "Greatsword" || weapon == "DualAxes")
            return true;
        return false;
    }

    public override void changeWeapon()
    {
        ItemSO item = GameObject.Find("WeaponSlot").GetComponent<EquippedSlot>().item;

        if (!item)
            equipCurrent();      
        else if (gameObject.GetComponent<Player>().Weapon != item.weaponType)
        {
            unequipWeapon(item.weaponType);
            equipCurrent();
        }
    }

    public override void unequipWeapon(string weapon)
    {
        if (weapon == "Greatsword")
            Destroy(GetComponent<Greatsword>());
        else if (weapon == "DualAxes")
            Destroy(GetComponent<DualAxes>());
    }

    public override void equipCurrent()
    {
        string weapon = gameObject.GetComponent<Player>().Weapon;
        if (weapon == "Greatsword")
            gameObject.AddComponent<Greatsword>();
        else if (weapon == "DualAxes")
            gameObject.AddComponent<DualAxes>();
        CooldownManager.instance.LoadCooldowns();
    }
}
