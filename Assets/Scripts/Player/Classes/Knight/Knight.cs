using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Class
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
            if (weapon == "SwordShield")
                gameObject.AddComponent<SwordShield>();
            else if (weapon == "Hammer")
                gameObject.AddComponent<Hammer>();
        }
        else if (weapon != item.weaponType)
        {
            switch (item.weaponType)
            {
                case "SwordShield":
                    Destroy(GetComponent<SwordShield>());
                    break;
                case "Hammer":
                    Destroy(GetComponent<Hammer>());
                    break;
                default:
                    break;
            }

            if (weapon == "SwordShield")
                gameObject.AddComponent<SwordShield>();
            else if (weapon == "Hammer")
                gameObject.AddComponent<Hammer>();
        }
    }
}
