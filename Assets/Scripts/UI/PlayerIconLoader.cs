using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconLoader : MonoBehaviour
{
    //Sprite Animations
    public List<Animator> Sprites = new List<Animator>();
    public int maxBodyParts = 4;
    private void Start()
    {
        Sprites.Add(gameObject.GetComponent<Animator>());
        for (int i = 0; i < maxBodyParts; i++)
            Sprites.Add(gameObject.transform.GetChild(i).GetComponent<Animator>());
        Display();
    }
    private void Display()
    {
        foreach (Animator sprite in Sprites)
            sprite.Play("IdleS");
    }
}
