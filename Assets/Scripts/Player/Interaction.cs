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
    public string doorTag = "Door";
    public float overlapRadius = 0.2f;  // Adjust the radius based on your needs
    public LayerMask doorLayer;
    public Transform playerTransform;
    public Door door;

    private void Update()
    {
        // Check if the player is close to any collider on the specified layer with the given tag
        Collider2D collider = Physics2D.OverlapCircle(playerTransform.position, overlapRadius, doorLayer);

        if (collider != null && collider.CompareTag(doorTag))
        {
            // Player is on a door
            Debug.Log("Player is on a door!");
            collider.GetComponent<Door>().QueuePlayer(this);
            door = collider.GetComponent<Door>();
        }
        else
        {
            // Player is off any door
            Debug.Log("Player is off any door!");
            if(door != null)
            {
                door.DequeuePlayer(this);
            }
        }
    }

}
