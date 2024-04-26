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
	

	public virtual bool UseItem(Player player)
    {
        return false;
    }

}
