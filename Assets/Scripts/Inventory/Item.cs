using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	private InventoryManager inventoryManager;

	[SerializeField]
    public string itemName;
    public int quantity;
    public Sprite sprite;
    public ItemType itemType;
    [TextArea]
    [SerializeField]
    public string itemDescription;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription, itemType);
            if (leftOverItems <= 0)
                Destroy(gameObject);
            else
                quantity = leftOverItems;
        }
    }


}
