using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteToImageCharacter : MonoBehaviour
{
    public SpriteRenderer originalCharacter;
    public Image visualCharacter;

    void Start()
    {
        visualCharacter = transform.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        visualCharacter.sprite = originalCharacter.sprite;
        visualCharacter.color = originalCharacter.color;
    }
}
