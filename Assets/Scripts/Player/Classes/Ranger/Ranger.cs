using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Class
{
    public override bool checkEquippable(string weapon)
    {
        return true;
    }

    public override void changeWeapon()
    {
        if (!kitChangeable)
            return;

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
        if (weapon == "Knives")
            Destroy(GetComponent<Knives>());
        else if (weapon == "Bow")
            Destroy(GetComponent<Bow>());
    }

    public override void equipCurrent()
    {
        string weapon = gameObject.GetComponent<Player>().Weapon;
        if (weapon == "Knives")
            gameObject.AddComponent<Knives>();
        else if (weapon == "Bow")
            gameObject.AddComponent<Bow>();
        CooldownManager.instance.LoadCooldowns();
    }
}
