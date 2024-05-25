using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using System.IO;
using static TreeEditor.TextureAtlas;

public class SpriteSheetSlicer : EditorWindow
{
	// Base directory containing the spritesheets
	private string baseDirectory = "Assets/Resources/PlayerSprites";

	// Set your desired sprite width and height here
	private int sliceWidth = 256;
	private int sliceHeight = 256;

	private List<string> foldersToLoad = new List<string>();

	[MenuItem("Tools/SpriteSheet Slicer")]
	public static void ShowWindow()
	{
		GetWindow<SpriteSheetSlicer>("SpriteSheet Slicer");
	}

	void OnGUI()
	{
		GUILayout.Label("Texture Slicer", EditorStyles.boldLabel);

		baseDirectory = EditorGUILayout.TextField("Base Directory", baseDirectory);
		sliceWidth = EditorGUILayout.IntField("Slice Width", sliceWidth);
		sliceHeight = EditorGUILayout.IntField("Slice Height", sliceHeight);

		EditorGUILayout.LabelField("Folders to Load");
		if (GUILayout.Button("Add Folder"))
		{
			foldersToLoad.Add("");
		}

		for (int i = 0; i < foldersToLoad.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			foldersToLoad[i] = EditorGUILayout.TextField(foldersToLoad[i]);
			if (GUILayout.Button("Remove"))
			{
				foldersToLoad.RemoveAt(i);
				i--;
			}
			EditorGUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Load Textures and Slice"))
		{
			LoadTextures();
		}
	}

	void LoadTextures()
	{
		foreach (string folder in foldersToLoad)
		{
			string[] textureNames = Directory.GetFiles(Path.Combine(baseDirectory, folder), "*.png");
			foreach (string textureName in textureNames)
			{
				string relativePath = textureName;
				Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);

				if (texture != null)
				{
					SliceTexture(texture, sliceWidth, sliceHeight);
					Debug.Log("Loaded and sliced texture: " + relativePath);
				}
				else
				{
					Debug.LogError("Failed to load texture: " + relativePath);
				}
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
				if (counter != 10)
				{
					SpriteRect spriteRect = new SpriteRect();
					spriteRect.rect = new Rect(x, y - sliceHeight, sliceWidth, sliceHeight);
					spriteRect.pivot = new Vector2(0.5f, 0f);
					spriteRect.name = texture.name + "_" + counter;
					spriteRect.alignment = SpriteAlignment.Center;
					spriteRect.border = new Vector4(0, 0, 0, 0);
					counter += 1;
					spriteRects.Add(spriteRect);
				}
			}
		}

		return spriteRects.ToArray();
	}
}