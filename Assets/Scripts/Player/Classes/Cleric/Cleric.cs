using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleric : Class
{
    public override bool checkEquippable(string weapon)
    {
        return true;
    }

    public override void changeWeapon(string weapon)
    {
        ItemSO item = GameObject.Find("WeaponSlot").GetComponent<EquippedSlot>().item;

        if (!item)
        {
            if (weapon == "Tome")
                gameObject.AddComponent<Tome>();
            else if (weapon == "MaceShield")
                gameObject.AddComponent<MaceShield>();
        }
        else if (weapon != item.weaponType)
        {
            switch (item.weaponType)
            {
                case "Tome":
                    Destroy(GetComponent<Tome>());
                    break;
                case "MaceShield":
                    Destroy(GetComponent<MaceShield>());
                    break;
                default:
                    break;
            }

            if (weapon == "Tome")
                gameObject.AddComponent<Tome>();
            else if (weapon == "MaceShield")
                gameObject.AddComponent<MaceShield>();
        }
    }
}
