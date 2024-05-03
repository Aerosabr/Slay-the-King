using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Class
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
            if (weapon == "Staff")
                gameObject.AddComponent<Staff>();
            else if (weapon == "Wand")
                gameObject.AddComponent<Wand>();
        }
        else if (weapon != item.weaponType)
        {
            switch (item.weaponType)
            {
                case "Staff":
                    Destroy(GetComponent<Staff>());
                    break;
                case "Wand":
                    Destroy(GetComponent<Wand>());
                    break;
                default:
                    break;
            }

            if (weapon == "Staff")
                gameObject.AddComponent<Staff>();
            else if (weapon == "Wand")
                gameObject.AddComponent<Wand>();
        }
    }
}
