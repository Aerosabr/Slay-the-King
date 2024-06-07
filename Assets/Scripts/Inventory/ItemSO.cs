using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
	public Sprite itemSprite;
    public string weaponType;
    public ItemType itemType;
    
	[TextArea]
	[SerializeField]
	public string itemDescription;
	
    public virtual void BuildItem(ItemSO item)
    {
        itemName = item.itemName;
        itemSprite = item.itemSprite;
        weaponType = item.weaponType;
        itemType = item.itemType;
        itemDescription = item.itemDescription;
    }

	public virtual bool UseItem(Player player)
    {
        return false;
    }

}
