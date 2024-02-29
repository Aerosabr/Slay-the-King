using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Skeleton")]
public class Skeleton : Enemy, Entity
{
    public Skeleton()
    {

    }

    public void Damaged(int damage) { }
}
