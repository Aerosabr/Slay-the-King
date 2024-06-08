using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public EquippedSlot weapon;
    // Start is called before the first frame update
    void Start()
    {
		DontDestroyOnLoad(gameObject);
	}

    IEnumerator WaitTilLoaded()
    {
        while (weapon.item == null)
            yield return null;
		DontDestroyOnLoad(gameObject);
		inventoryManager.enabled = true;
	}
}
