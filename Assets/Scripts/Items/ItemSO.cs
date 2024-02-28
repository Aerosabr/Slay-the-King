using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange stateToChange = new StatToChange();
    public AttributeToChange attributeToChange = new AttributeToChange();
    public int amountToChangeStat;
    public int amountToChangeAttribute;

    public void UseItem()
    {

    }

    public enum StatToChange
    {
        none,
        health,
        gold,
    };

    public enum AttributeToChange
    {
        none,
        strength,
        magic,
        armor,
        resistance,
        dexterity,
    };
}
