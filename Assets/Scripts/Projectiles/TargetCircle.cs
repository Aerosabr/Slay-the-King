using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCircle : MonoBehaviour
{
    public Transform fill;
    public float duration;

    public IEnumerator FillTarget()
    {
        fill.localScale += new Vector3(.04f, .04f);
        yield return new WaitForSeconds(duration / 50);
        if (fill.localScale.x < 2)
            StartCoroutine(FillTarget());
        else
            Destroy(gameObject);
    }

    public void InitiateTarget(float rad, float dur)
    {
        transform.localScale = new Vector3(rad, rad, rad);
        duration = dur;
        StartCoroutine(FillTarget());
    }
}
