using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CUIColorPicker1 : MonoBehaviour
{
	public Color Color { get { return _color; } set { Setup(value); } }
	public CharacterCustomization character;
	public void SetOnValueChangeCallback(Action<Color> onValueChange)
	{
		_onValueChange = onValueChange;
	}
	private Color _color = Color.red;
	private Action<Color> _onValueChange;
	private Action _update;

	private static void RGBToHSV(Color color, out float h, out float s, out float v)
	{
		var cmin = Mathf.Min(color.r, color.g, color.b);
		var cmax = Mathf.Max(color.r, color.g, color.b);
		var d = cmax - cmin;
		if (d == 0)
		{
			h = 0;
		}
		else if (cmax == color.r)
		{
			h = Mathf.Repeat((color.g - color.b) / d, 6);
		}
		else if (cmax == color.g)
		{
			h = (color.b - color.r) / d + 2;
		}
		else
		{
			h = (color.r - color.g) / d + 4;
		}
		s = cmax == 0 ? 0 : d / cmax;
		v = cmax;
	}

	private static bool GetLocalMouse(GameObject go, out Vector2 result)
	{
		var rt = (RectTransform)go.transform;
		var mp = rt.InverseTransformPoint(Input.mousePosition);
		result.x = Mathf.Clamp(mp.x, rt.rect.min.x + 10f, rt.rect.max.x - 10f);
		result.y = Mathf.Clamp(mp.y, rt.rect.min.y + 25f, rt.rect.max.y - 25f);
		return rt.rect.Contains(mp);
	}

	private static Vector2 GetWidgetSize(GameObject go)
	{
		var rt = (RectTransform)go.transform;
		return rt.rect.size;
	}

	private GameObject GO(string name)
	{
		return transform.Find(name).gameObject;
	}

	private void Setup(Color inputColor)
	{
		var hueGO = GO("Hue");
		var hueKnob = GO("Hue/Knob");
		var result = GO("Result");
		var hueColors = new Color[] {
		new Color(0.36f, 0.24f, 0.18f),  // Very dark skin tone
        new Color(0.56f, 0.40f, 0.30f),  // Dark skin tone
        new Color(0.71f, 0.56f, 0.43f),  // Medium-dark skin tone
        new Color(0.85f, 0.66f, 0.54f),  // Medium-light skin tone
        new Color(0.94f, 0.79f, 0.67f),  // Light skin tone
        new Color(1f, 0.88f, 0.78f)      // Very light skin tone
		};

		var hueTex = new Texture2D(1, 6);
		for (int i = 0; i < 6; i++)
		{
			hueTex.SetPixel(0, i, hueColors[i]);
		}
		hueTex.Apply();
		hueGO.GetComponent<Image>().sprite = Sprite.Create(hueTex, new Rect(0, 0, 1, 6), new Vector2(0.5f, 0.5f));
		var hueSz = GetWidgetSize(hueGO);

		float Hue;
		RGBToHSV(inputColor, out Hue, out _, out _); // Ignore saturation and value

		Action applyHue = () => {
			var resultColor = hueColors[Mathf.Clamp((int)Hue, 0, 5)];
			var resImg = result.GetComponent<Image>();
			resImg.color = resultColor;
			character.ApplySkinColor(resImg.color);
			if (_color != resultColor)
			{
				if (_onValueChange != null)
				{
					_onValueChange(resultColor);
				}
				_color = resultColor;
			}
		};

		applyHue();
		hueKnob.transform.localPosition = new Vector2(hueKnob.transform.localPosition.x, Hue / 5 * hueSz.y);

		Action dragH = null;
		Action idle = () => {
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 mp;
				if (GetLocalMouse(hueGO, out mp))
				{
					_update = dragH;
				}
			}
		};

		dragH = () => {
			Vector2 mp;
			GetLocalMouse(hueGO, out mp);
			Hue = mp.y / hueSz.y * 5;

			// Prevent repeats after the dark skin tone
			if (Hue >= 2 && Hue < 3)
			{
				Hue = 3; // Skip the dark skin tone
			}

			applyHue();
			hueKnob.transform.localPosition = new Vector2(hueKnob.transform.localPosition.x, mp.y);
			if (Input.GetMouseButtonUp(0))
			{
				_update = idle;
			}
		};

		_update = idle;
	}


	public void SetRandomColor()
	{
		var rng = new System.Random();
		var r = (rng.Next() % 1000) / 1000.0f;
		var g = (rng.Next() % 1000) / 1000.0f;
		var b = (rng.Next() % 1000) / 1000.0f;
		Color = new Color(r, g, b);
	}

	void Awake()
	{
		Color = Color.red;
	}

	void Update()
	{
		_update();
	}
}
