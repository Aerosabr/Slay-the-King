using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using System.IO;
using static TreeEditor.TextureAtlas;

public class TextureSlicer : MonoBehaviour
{
	// Base directory containing the spritesheets
	public string baseDirectory = "Assets/Resources/PlayerSprites";

	// Set your desired sprite width and height here
	public int sliceWidth = 256;
	public int sliceHeight = 256;

	public List<string> foldersToLoad = new List<string>();

	void Start()
	{
		LoadTextures();
	}

	void LoadTextures()
	{
		foreach (string folder in foldersToLoad)
		{
			string[] textureNames = Directory.GetFiles(Application.dataPath + "/Resources/" + folder, "*.png");
			foreach (string textureName in textureNames)
			{
				string resourceName = folder + '/' + Path.GetFileNameWithoutExtension(textureName);

				Texture2D texture = Resources.Load<Texture2D>(resourceName);
				// Do something with the loaded texture
				SliceTexture(texture, sliceWidth, sliceHeight);
				Debug.Log("Loaded texture: " + resourceName);
			}
		}

	}

	public static void SliceTexture(Texture2D texture, int sliceWidth, int sliceHeight)
	{

		string assetPath = AssetDatabase.GetAssetPath(texture);
		Debug.Log(assetPath);
		TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

		if (textureImporter == null)
		{
			Debug.LogError("Failed to get TextureImporter for texture");
		}
		else
		{
			// Setup the texture import settings
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.spriteImportMode = SpriteImportMode.Multiple;
			textureImporter.spritePixelsPerUnit = 100; // Adjust this value as needed
			textureImporter.mipmapEnabled = false;
			textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
			textureImporter.maxTextureSize = 8192;

			// Reimport the texture with updated settings
			AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

			// Generate sprite rects for slicing
			SpriteRect[] spriteRects = GenerateSpriteRectData(texture.width, texture.height, sliceWidth, sliceHeight, texture);

			// Apply sprite rects
			var factory = new SpriteDataProviderFactories();
			factory.Init();
			ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(texture);
			dataProvider.InitSpriteEditorDataProvider();
			dataProvider.SetSpriteRects(spriteRects);
			dataProvider.Apply();

			// Save and reimport the asset
			var assetImporter = dataProvider.targetObject as AssetImporter;
			assetImporter.SaveAndReimport();

			// Load all sliced sprites
			Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
		}
	}

	private static SpriteRect[] GenerateSpriteRectData(int textureWidth, int textureHeight, int sliceWidth, int sliceHeight, Texture2D texture)
	{
		List<SpriteRect> spriteRects = new List<SpriteRect>();
		int counter = 0;

		for (int y = textureHeight; y > 0; y -= sliceHeight)
		{
			for (int x = 0; x < textureWidth; x += sliceWidth)
			{
				if(counter != 20)
				{
					SpriteRect spriteRect = new SpriteRect();
					spriteRect.rect = new Rect(x, y - sliceHeight, sliceWidth, sliceHeight);
					spriteRect.pivot = new Vector2(0.5f, 0f);
					spriteRect.name = texture.name + "_" + counter;
					spriteRect.alignment = SpriteAlignment.BottomCenter;
					spriteRect.border = new Vector4(0, 0, 0, 0);
					counter += 1;
					spriteRects.Add(spriteRect);
				}
			}
		}

		return spriteRects.ToArray();
	}
}
