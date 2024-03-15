using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {

        public Rigidbody2D body;
        public SpriteRenderer spriteRenderer;
       public Animator animator;

        public float walkSpeed;
        public float idleTime;
        public Vector2 direction;
        public Vector2 idleDirection;

        public bool isShooting = false;

        private void Update()
        {
            if(Input.GetMouseButtonDown(1) && !isShooting)
            {
                body.velocity = new Vector2(0,0);
                isShooting = true;
                StartCoroutine(Shoot());
            }
            if(!isShooting)
            {
                direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
                body.velocity = direction * walkSpeed;

                // HandleSpriteFlip();
                UpdateChildPositions();
            }
        }
        private IEnumerator Shoot()
        {
            yield return new WaitForSeconds(1.1f);
            isShooting = false;
        }
        private void UpdateChildPositions()
        {
            // Iterate through all child objects
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                // Update child position to match the main body
                child.position = transform.position;
            }
        }
    }
}
