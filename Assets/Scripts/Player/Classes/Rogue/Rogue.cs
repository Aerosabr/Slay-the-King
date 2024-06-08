using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Class
{
    public override bool checkEquippable(string weapon)
    {
        if (weapon == "Daggers" || weapon == "Scythe")
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
        if (weapon == "Scythe")
            Destroy(GetComponent<Scythe>());
        else if (weapon == "Daggers")
            Destroy(GetComponent<Daggers>());
    }

    public override void equipCurrent()
    {
        string weapon = gameObject.GetComponent<Player>().Weapon;
        if (weapon == "Scythe")
            gameObject.AddComponent<Scythe>();
        else if (weapon == "Daggers")
            gameObject.AddComponent<Daggers>();
        CooldownManager.instance.LoadCooldowns();
    }
}
