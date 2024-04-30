using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Class
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
            if (weapon == "Greatsword")
                gameObject.AddComponent<Greatsword>();
            else if (weapon == "DualAxes")
                gameObject.AddComponent<DualAxes>();
        }
        else if (weapon != item.weaponType)
        {
            switch (item.weaponType)
            {
                case "Greatsword":
                    Destroy(GetComponent<Greatsword>());
                    break;
                case "DualAxes":
                    Destroy(GetComponent<DualAxes>());
                    break;
                default:
                    break;
            }

            if (weapon == "Greatsword")
                gameObject.AddComponent<Greatsword>();
            else if (weapon == "DualAxes")
                gameObject.AddComponent<DualAxes>();
        }
    }
}
