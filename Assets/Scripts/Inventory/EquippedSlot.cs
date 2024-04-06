using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    // SLOT APPEARANCE
    [SerializeField]
    private Image slotImage;

    // SLOT DATA
    [SerializeField]
    public Item item;

    private InventoryManager inventoryManager;

    // OTHER VARIABLES
    private bool slotInUse;
    [SerializeField]
    public GameObject selectedShader;
    [SerializeField]
    public bool thisItemSelected;
    [SerializeField]
    private Sprite emptySprite;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    void OnLeftClick()
    {
        if (thisItemSelected)
        {
            if (slotInUse)
            {
                UnEquipGear();
            }
            else
            {
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(false);
                thisItemSelected = false;
            }
        }
        else
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;

            Player player = FindObjectOfType<Player>(); // Find the player in the scene
            for (int i = 0; i < this.equipmentSOLibrary.equipmentSO.Length; i++)
            {
                if (equipmentSOLibrary.equipmentSO[i].itemName == this.itemName)
                    equipmentSOLibrary.equipmentSO[i].PreviewEquipment(player);
            }
        }
    }

    void OnRightClick()
    {
        UnEquipGear();
    }

    public void EquipGear(Sprite itemSprite, string itemName, string itemDescription)
    {
        // Check if item already there
        if (slotInUse)
            UnEquipGear();

        // Update Image
        this.itemSprite = itemSprite;
        slotImage.sprite = this.itemSprite;

        // Update Data
        this.itemName = itemName;
        this.itemDescription = itemDescription;

        // Update Player Stats
        Player player = FindObjectOfType<Player>(); // Find the player in the scene
        for (int i = 0; i < equipmentSOLibrary.equipmentSO.Length; i++)
        {
            if (equipmentSOLibrary.equipmentSO[i].itemName == this.itemName)
                equipmentSOLibrary.equipmentSO[i].EquipItem(player);
        }

        slotInUse = true;
    }

    public void UnEquipGear()
    {
        if (!slotInUse || string.IsNullOrEmpty(item.itemName))
        {
            return;
        }

        inventoryManager.DeselectAllSlots();
        inventoryManager.AddItem(item);

        slotImage.sprite = emptySprite;
        slotInUse = false;

        Player player = FindObjectOfType<Player>(); // Find the player in the scene
        for (int i = 0; i < equipmentSOLibrary.equipmentSO.Length; i++)
        {
            if (equipmentSOLibrary.equipmentSO[i].itemName == item.itemName)
            {
                equipmentSOLibrary.equipmentSO[i].UnEquipItem(player);
            }
        }

        itemName = "";
        itemDescription = "";

        if (player != null)
        {
            player.TurnOffPreviewStats();
        }
    }
}
