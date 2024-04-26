using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Class
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
            if (weapon == "Scythe")
                gameObject.AddComponent<Scythe>();
            else if (weapon == "Daggers")
                gameObject.AddComponent<Daggers>();
        }
        else if (weapon != item.weaponType)
        {
            switch (item.weaponType)
            {
                case "Scythe":
                    Destroy(GetComponent<Scythe>());
                    break;
                case "Daggers":
                    Destroy(GetComponent<Daggers>());
                    break;
                default:
                    break;
            }

            if (weapon == "Scythe")
                gameObject.AddComponent<Scythe>();
            else if (weapon == "Daggers")
                gameObject.AddComponent<Daggers>();
        }
    }
}
