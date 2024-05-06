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

	public Sprite weapon1Icon;
	public Sprite weapon2Icon;

	public GameObject characterClass;
}
