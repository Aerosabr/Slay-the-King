using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthLiquidAnimation : MonoBehaviour
{
    public RawImage rawImage;
    public float scrollSpeed = 1.0f;
    public float scrollSpeedMultiplier = 1.0f;
    public float amplitudeX = 0.5f;
    public float amplitudeY = 0.5f;

    private float time = 0.0f;

    void Update()
    {
        time += Time.deltaTime * scrollSpeed * scrollSpeedMultiplier;
        float offsetX = Mathf.Repeat(time, 8.0f) * amplitudeX;
        float offsetY = Mathf.Sin(time) * amplitudeY;

        rawImage.uvRect = new Rect(offsetX, offsetY, 1, 1);
    }
}
