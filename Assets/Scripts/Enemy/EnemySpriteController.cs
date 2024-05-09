using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteController : MonoBehaviour
{
    public Animator sprite;
    public Vector2 currentDir = Vector2.zero;
    public bool isMoving;
    public bool isAttacking;

    public void PlayAnimation(string Name)
    {
        float angle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
        switch (angle)
        {
            case float _ when angle > -11.25f && angle <= 11.25f:
                Name += "E";
                break;
            case float _ when angle > 11.25f && angle <= 33.75f:
                Name += "ENE";
                break;
            case float _ when angle > 33.75f && angle <= 56.25f:
                Name += "NE";
                break;
            case float _ when angle > 56.25f && angle <= 78.75f:
                Name += "NNE";
                break;
            case float _ when angle > 78.75f && angle <= 101.25f:
                Name += "N";
                break;
            case float _ when angle > 101.25f && angle <= 123.75f:
                Name += "NNW";
                break;
            case float _ when angle > 123.75f && angle <= 146.25f:
                Name += "NW";
                break;
            case float _ when angle > 146.25f && angle <= 168.75f:
                Name += "WNW";
                break;
            case float _ when angle > 168.75f || angle <= -168.75f:
                Name += "W";
                break;
            case float _ when angle > -168.75f && angle <= -146.25f:
                Name += "WSW";
                break;
            case float _ when angle > -146.25f && angle <= -123.75f:
                Name += "SW";
                break;
            case float _ when angle > -123.75f && angle <= -101.25f:
                Name += "SSW";
                break;
            case float _ when angle > -101.25f && angle <= -78.75f:
                Name += "S";
                break;
            case float _ when angle > -78.75f && angle <= -56.25f:
                Name += "SSE";
                break;
            case float _ when angle > -56.25f && angle <= -33.75f:
                Name += "SE";
                break;
            case float _ when angle > -33.75f && angle <= -11.25f:
                Name += "ESE";
                break;
        }
        sprite.Play(Name);
    }
    
}
