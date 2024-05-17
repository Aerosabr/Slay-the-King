using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerAssetLibrary : ScriptableObject
{
	public Animator shirt;
	public Animator pant;
	public Animator boot;
	public Animator glove;
	public Animator weapon1;
	public Animator weapon2;
	public bool weapon1twoHanded;
	public bool weapon2twoHanded;
	public Sprite weapon1Icon;
	public Sprite weapon2Icon;
    public string weapon1Name;
    public string weapon2Name;

  	public GameObject characterClass;
}
