using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamagePopup : MonoBehaviour
{
    public static int sortingOrder;
    public const float DISAPPEAR_TIMER_MAX = 1f;
    public TextMeshPro textMesh;
    public float disappearTimer;
    public Color textColor;
    public Vector3 moveVector;

    void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        textColor = textMesh.color;
        disappearTimer = 1f;
    }

    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;
        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
                Destroy(gameObject);
        }
    }

    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCrit)
    {
        Transform temp;
        Transform damagePopupTransform = Instantiate(Resources.Load<GameObject>("Prefabs/PopupText").transform, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCrit);

        return damagePopup;
    }

    public void Setup(int damageAmount, bool isCrit)
    {
        textMesh.SetText(damageAmount.ToString());
        if (!isCrit) //Not a crit
        {
            textMesh.fontSize = 3;
            textColor = Color.yellow;
        }
        else //Is a crit
        {
            textMesh.fontSize = 4;
            textColor = Color.red;
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
        moveVector = new Vector3(.7f, 1) * 5f;
    }
}
