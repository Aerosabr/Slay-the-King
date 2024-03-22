using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    //SLOT APPEARANCE
    [SerializeField]
    private Image slotImage;

    [SerializeField]
    private TMP_Text slotName;

    //SLOT DATA
    [SerializeField]
    private ItemType itemType = new ItemType();

    private Sprite itemSprite;
    private string itemName;
    private string itemDescription;

    private InventoryManager inventoryManager;
    private EquipmentSOLibrary equipmentSOLibrary;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        equipmentSOLibrary = GameObject.Find("InventoryCanvas").GetComponent<EquipmentSOLibrary>();
    }

    //OTHER VARIABLES
    private bool slotInUse;
    [SerializeField]
    public GameObject selectedShader;

    [SerializeField]
    public bool thisItemSelected;

    [SerializeField]
    private Sprite emptySprite;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
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

            for (int i = 0; i < this.equipmentSOLibrary.equipmentSO.Length; i++)
            {
                if (equipmentSOLibrary.equipmentSO[i].itemName == this.itemName)
                    equipmentSOLibrary.equipmentSO[i].PreviewEquipment();
            }
        }
    }


    void OnRightClick()
    {
        UnEquipGear();
    }

    public void EquipGear(Sprite itemSprite, string itemName, string itemDescription)
    {
        //Check if item already there
        if (slotInUse)
            UnEquipGear();

        //Update Image
        this.itemSprite = itemSprite;
        slotImage.sprite = this.itemSprite;
        slotName.enabled = false;

        //Update Data
        this.itemName = itemName;
        this.itemDescription = itemDescription;

        //Update Player Stats
        for (int i = 0; i <equipmentSOLibrary.equipmentSO.Length; i++ )
        {
            if (equipmentSOLibrary.equipmentSO[i].itemName == this.itemName)
                equipmentSOLibrary.equipmentSO[i].EquipItem();
        }

        slotInUse = true;
    }

    public void UnEquipGear()
    {
        if (!slotInUse || string.IsNullOrEmpty(itemName))
        {
            return;
        }

        inventoryManager.DeselectAllSlots();
        inventoryManager.AddItem(itemName, 1, itemSprite, itemDescription, itemType);

        slotImage.sprite = emptySprite;
        slotName.enabled = true;
        slotInUse = false;


        for (int i = 0; i < equipmentSOLibrary.equipmentSO.Length; i++)
        {
            if (equipmentSOLibrary.equipmentSO[i].itemName == this.itemName)
            {
                equipmentSOLibrary.equipmentSO[i].UnEquipItem();
            }
        }

        itemName = "";
        itemDescription = "";

        GameObject.Find("StatManager").GetComponent<PlayerStats>().TurnOffPreviewStats();
    }


}
