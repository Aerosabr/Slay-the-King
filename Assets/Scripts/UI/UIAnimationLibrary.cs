using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationLibrary : MonoBehaviour
{

    
    public RectTransform UITarget;
    public float targetYDownPosition;
    public float targetYUpPosition;

    [Tooltip("Higher the number, the slower it is")]
    public float speed = 0.2f;

    public void Start()
    {

    }
    public void RollPanelDownWrapper(bool Open)
    {
        StartCoroutine(RollPanelDown(Open));
    }
    public IEnumerator RollPanelDown(bool Open)
    {
        if(Open)
        {
            UITarget.gameObject.SetActive(true);
        }
        Vector2 startPos = UITarget.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < speed)
        {
            float newY = Mathf.Lerp(startPos.y, targetYDownPosition, (elapsedTime / speed));
            UITarget.anchoredPosition = new Vector2(startPos.x, newY);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure panel reaches the exact target position
        UITarget.anchoredPosition = new Vector2(startPos.x, targetYDownPosition);
        if(!Open)
        {
            UITarget.gameObject.SetActive(false);
        }
    }
    public void RollPanelUpWrapper(bool Open)
    {
        StartCoroutine(RollPanelUp(Open));
    }
    public IEnumerator RollPanelUp(bool Open)
    {
        if(Open)
        {
            UITarget.gameObject.SetActive(true);
        }
        Vector2 startPos = UITarget.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < speed)
        {
            float newY = Mathf.Lerp(startPos.y, targetYUpPosition, (elapsedTime / speed));
            UITarget.anchoredPosition = new Vector2(startPos.x, newY);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure panel reaches the exact target position
        UITarget.anchoredPosition = new Vector2(startPos.x, targetYUpPosition);
        if(!Open)
        {
            UITarget.gameObject.SetActive(false);
        }
    }
}
