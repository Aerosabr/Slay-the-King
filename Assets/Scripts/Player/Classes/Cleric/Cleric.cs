using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleric : Class
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
        if (weapon == "Tome")
            Destroy(GetComponent<Tome>());
        else if (weapon == "MaceShield")
            Destroy(GetComponent<MaceShield>());
    }

    public override void equipCurrent()
    {
        string weapon = gameObject.GetComponent<Player>().Weapon;
        if (weapon == "Tome")
            gameObject.AddComponent<Tome>();
        else if (weapon == "MaceShield")
            gameObject.AddComponent<MaceShield>();
        CooldownManager.instance.LoadCooldowns();
    }
}
