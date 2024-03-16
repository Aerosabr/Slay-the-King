using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Interaction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float overlapRadius = 0.2f;  // Adjust the radius based on your needs
    public Transform ShopUI;
    public Transform Prompt;
    public LayerMask doorLayer;
    public Transform playerTransform;
    public Door door;

    private void Update()
    {
        // Check if the player is close to any collider on the specified layer with the given tag
        Collider2D collider = Physics2D.OverlapCircle(playerTransform.position, overlapRadius, doorLayer);

        if (collider != null && collider.CompareTag("Door"))
        {
            collider.GetComponent<Door>().QueuePlayer(this);
            door = collider.GetComponent<Door>();
        }
        else if (collider != null && collider.CompareTag("Store"))
        {
            if(!ShopUI.gameObject.activeSelf)
                Prompt.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(!ShopUI.gameObject.activeSelf)
                {
                    ShopUI.gameObject.SetActive(true);
                    Prompt.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Prompt.gameObject.SetActive(false);
            if(door != null)
            {
                door.DequeuePlayer(this);
            }
        }
    }

}
