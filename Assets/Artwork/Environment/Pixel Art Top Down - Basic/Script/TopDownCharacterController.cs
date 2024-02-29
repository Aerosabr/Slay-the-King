using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {

        public Rigidbody2D body;
        public SpriteRenderer spriteRenderer;

        public List<Sprite> nSprites;
        public List<Sprite> neSprites;
        public List<Sprite> eSprites;
        public List<Sprite> seSprites;
        public List<Sprite> sSprites;

        public List<Sprite> swSprites;
        public List<Sprite> wSprites;
        public List<Sprite> nwSprites;
        public float walkSpeed;
        public float frameRate;
        public float idleTime;
        public Vector2 direction;

        private void Start()
        {
        }


        private void Update()
        {
            direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            body.velocity = direction * walkSpeed;

            HandleSpriteFlip();

            List<Sprite> directionSprites = GetSpriteDirection();
            
            if(directionSprites != null)
            {
                float playTime = Time.time - idleTime;
                int totalFrames = (int)(playTime*frameRate);
                int frame = totalFrames % directionSprites.Count;
                spriteRenderer.sprite = directionSprites[frame];
            }
            else
            {

            }
        }

        public void HandleSpriteFlip()
        {
            if(!spriteRenderer.flipX && direction.x < 0 )
            {
                spriteRenderer.flipX = true;
            }
            else if(spriteRenderer.flipX && direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }

        public List<Sprite> GetSpriteDirection()
        {
            List<Sprite> selectedSprites = null;
            if(direction.y > 0)
            {
                if(Mathf.Abs(direction.x) > 0)
                {
                    selectedSprites = neSprites;
                }
                else if(Mathf.Abs(direction.x) == 0)
                {
                    selectedSprites = nSprites;
                }
                else if(Mathf.Abs(direction.x) < 0)
                {
                    selectedSprites = nwSprites;
                }
            }
            else if(direction.y < 0)
            {
                if(Mathf.Abs(direction.x) > 0)
                {
                    selectedSprites = seSprites;
                }
                else if(Mathf.Abs(direction.x) == 0)
                {
                    selectedSprites = sSprites;
                }
                else if(Mathf.Abs(direction.x) < 0)
                {
                    selectedSprites = swSprites;
                }
            }
            else
            {
                if(Mathf.Abs(direction.x) > 0)
                {
                    selectedSprites = eSprites;
                }
            }
            return selectedSprites;
        }
    }
}
