using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AnimationBuilder : MonoBehaviour
{
	public string[] combineFolders; // Example array of folder names
	public string[] folderNamesToCompare;

	void Awake()
	{
		// Iterate through each folder in combineFolders
		foreach (string combineFolder in combineFolders)
		{
			// Get all sprite sheets in the specified combine folder
			string[] spriteSheets = Directory.GetFiles("Assets/Resources/PlayerSprites/" + combineFolder, "*.png");

			foreach (string spriteSheet in spriteSheets)
			{
				// Get the name of the sprite sheet (without extension)
				string spriteSheetName = Path.GetFileNameWithoutExtension(spriteSheet);
				string normalizedSpriteSheetName = NormalizeName(spriteSheetName);

				// Ensure the directory exists
				string directoryPath = Path.Combine("Assets/Animations/Player/" + combineFolder);
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}

				// Load the sprites from the sprite sheet
				Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet)
											  .OfType<Sprite>()
											  .ToArray();

				Debug.Log($"Loaded {sprites.Length} sprites from {spriteSheetName}");

				if (sprites.Length > 0)
				{
					CreateOrUpdateAnimationClip(normalizedSpriteSheetName, sprites, combineFolder);
				}
				else
				{
					Debug.LogError($"No sprites found in sprite sheet: PlayerSprites/{combineFolder}/{spriteSheetName}");
				}
			}
		}
	}

	private void CreateOrUpdateAnimationClip(string animationName, Sprite[] sprites, string combineFolder)
	{
		// Check if the animation clip already exists
		string animationClipPath = Path.Combine("Assets/Animations/Player/" + combineFolder, animationName + ".anim");
		Debug.Log($"Animation clip path: {animationClipPath}");

		AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationClipPath);

		if (animationClip == null)
		{
			// Create a new animation clip if it doesn't exist
			animationClip = new AnimationClip
			{
				frameRate = 60 // Set frame rate to 60 frames per second
			};

			if (animationName.Contains("Run") || animationName.Contains("Idle"))
			{
				animationClip.wrapMode = WrapMode.Loop;
			}
		}

		// Store existing events
		AnimationEvent[] existingEvents = AnimationUtility.GetAnimationEvents(animationClip);

		// Add or update sprite animation curve in the animation clip
		EditorCurveBinding curveBinding = new EditorCurveBinding
		{
			type = typeof(SpriteRenderer),
			path = "",
			propertyName = "m_Sprite"
		};

		ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Length];

		// Calculate duration for each frame
		float duration = 5f / animationClip.frameRate;

		for (int i = 0; i < sprites.Length; i++)
		{
			keyframes[i] = new ObjectReferenceKeyframe
			{
				time = i * duration,
				value = sprites[i]
			};
		}
		AnimationUtility.SetObjectReferenceCurve(animationClip, curveBinding, keyframes);

		// Set the existing events back to the animation clip
		AnimationUtility.SetAnimationEvents(animationClip, existingEvents);

		if (AssetDatabase.Contains(animationClip))
		{
			// If the animation clip already exists, just save and refresh
			EditorUtility.SetDirty(animationClip);
		}
		else
		{
			// If it's a new animation clip, create an asset
			AssetDatabase.CreateAsset(animationClip, animationClipPath);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private string NormalizeName(string name)
	{
		return name.Replace("_", "");
	}
}
