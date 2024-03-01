using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Almanac : MonoBehaviour
{
    //Script intended to represent the in-game wiki
    public static Almanac instance;
    public List<Class> Classes = new List<Class>();

    void Awake()
    {
        instance = this;
    }
}
