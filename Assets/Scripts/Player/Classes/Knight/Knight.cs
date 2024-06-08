using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Class
{
    public override bool checkEquippable(string weapon)
    {
        if(weapon == "Hammer" || weapon == "SwordShield")
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
        if (weapon == "SwordShield")
            Destroy(GetComponent<SwordShield>());
        else if (weapon == "Hammer")
            Destroy(GetComponent<Hammer>());
    }

    public override void equipCurrent()
    {
        string weapon = gameObject.GetComponent<Player>().Weapon;
        if (weapon == "SwordShield")
            gameObject.AddComponent<SwordShield>();
        else if (weapon == "Hammer")
            gameObject.AddComponent<Hammer>();
        CooldownManager.instance.LoadCooldowns();
    }
}
