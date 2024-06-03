using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Class
{
    public override bool checkEquippable(string weapon)
    {
        return true;
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
        if (weapon == "Staff")
            Destroy(GetComponent<Staff>());
        else if (weapon == "Wand")
            Destroy(GetComponent<Wand>());
    }

    public override void equipCurrent()
    {
        string weapon = gameObject.GetComponent<Player>().Weapon;
        if (weapon == "Staff")
            gameObject.AddComponent<Staff>();
        else if (weapon == "Wand")
            gameObject.AddComponent<Wand>();
        CooldownManager.instance.LoadCooldowns();
    }
}
