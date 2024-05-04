using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Class
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
            if (weapon == "Bow")
                gameObject.AddComponent<Bow>();
            else if (weapon == "Knives")
                gameObject.AddComponent<Knives>();
        }
        else if (weapon != item.weaponType)
        {
            switch (item.weaponType)
            {
                case "Bow":
                    Destroy(GetComponent<Bow>());
                    break;
                case "Knives":
                    Destroy(GetComponent<Knives>());
                    break;
                default:
                    break;
            }

            if (weapon == "Bow")
                gameObject.AddComponent<Bow>();
            else if (weapon == "Knives")
                gameObject.AddComponent<Knives>();
        } 
    }
}
