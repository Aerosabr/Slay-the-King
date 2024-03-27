using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconLoader : MonoBehaviour
{
    //Sprite Animations
    public List<Animator> Sprites = new List<Animator>();
    private void Start()
    {
        Sprites.Add(gameObject.GetComponent<Animator>());
        for (int i = 0; i < 4; i++)
            Sprites.Add(gameObject.transform.GetChild(i).GetComponent<Animator>());
        Display();
    }
    private void Display()
    {
        foreach (Animator sprite in Sprites)
            sprite.Play("IdleS");
    }
}
