using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public AttributeToChange attributeToChange = new AttributeToChange();
    public int amountToChangeStat;
    public int amountToChangeAttribute;

    public bool UseItem()
    {
        Debug.Log("UseItem called.");
        if (statToChange == StatToChange.health)
        {
            Debug.Log($"Healing with amount: {amountToChangeStat}");
            PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
            if (playerHealth.health == playerHealth.maxHealth)
            {
                return false;
            }
            else
            {
                playerHealth.ChangeHealth(amountToChangeStat);
                return true;
            }
        }
        return false;

    }

    public enum StatToChange
    {
        none,
        health,
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
