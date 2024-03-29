using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ghost : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeSpeed = 10f; // Adjust as needed

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Reduce the alpha value of the sprite renderer's color gradually
        Color color = spriteRenderer.color;
        color.a -= fadeSpeed * Time.deltaTime;
        spriteRenderer.color = color;

        // Destroy the GameObject if the alpha value reaches 0
        if (color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
