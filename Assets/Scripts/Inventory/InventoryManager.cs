using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;
            Debug.Log("Inventory toggled: " + menuActivated);
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            InventoryMenu.SetActive(true);
            menuActivated = true;
            Debug.Log("Inventory toggled: " + menuActivated);
        }
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
        Debug.Log("ItemName = " + itemName + "quantity = " + quantity + "itemSprite = " + itemSprite);
    }
}
