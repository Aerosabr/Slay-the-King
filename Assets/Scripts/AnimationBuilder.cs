using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using System.Net.NetworkInformation;

public class AnimationBuilder : MonoBehaviour
{
	public string[] combineFolders; // Example array of folder names
    public string[] folderNamesToCompare;

	void Awake()
    {
        // Get all subdirectories (folders) within the parent folder
        
        foreach (string combineFolder in combineFolders)
        {
			string[] subDirectories = Directory.GetDirectories("Assets/Resources/PlayerSprites/" + combineFolder);
			foreach (string subDirectory in subDirectories)
            {
                // Get the name of the subdirectory (folder)
                string folderName = Path.GetFileName(subDirectory);
                string[] subDirections = Directory.GetDirectories(subDirectory);
                // || folderName == "Block" || folderName == "DSlash" || folderName == "HandCast"
                if (Array.Exists(folderNamesToCompare, name => name == folderName))
                {
                    foreach (string subDirection in subDirections)
                    {
                        Debug.Log(subDirection);
                        string directionName = Path.GetFileName(subDirection);
                        directionName = char.ToUpper(directionName[0]) + directionName.Substring(1);

                        // Load all sprites from the specified folder path using Resources.LoadAll
                        Sprite[] sprites = Resources.LoadAll<Sprite>(Path.Combine("PlayerSprites/" + combineFolder, folderName, directionName));
                        CreateAnimationClip(folderName + directionName, sprites, subDirection, combineFolder);

                    }
                }
            }
        }
    }
    private void CreateAnimationClip(string folderName, Sprite[] sprites, string parentDirectory, string combineFolder)
    {
        // Create a new animation clip
        AnimationClip animationClip = new AnimationClip();
        animationClip.frameRate = 60; // Set frame rate to 5 frames per second

        // Add sprite animation curve to the animation clip
        EditorCurveBinding curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(SpriteRenderer);
        curveBinding.path = "";
        curveBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Length];
        // Add keyframes every 5 seconds
        float duration = 5f /60f;
        int numKeyframes = 10; // Adjust the number of keyframes as needed
        for (int i = 0; i < numKeyframes; i++)
        {
            ObjectReferenceKeyframe keyframe = new ObjectReferenceKeyframe();
            keyframe.time = i * duration;
            keyframe.value = sprites[i];
            keyframes[i] = keyframe;
        }
        AnimationUtility.SetObjectReferenceCurve(animationClip, curveBinding, keyframes);

        // Save animation clip as asset
        string animationClipPath = Path.Combine("Assets/Animations/Player/" + combineFolder, folderName + ".anim");
        AssetDatabase.CreateAsset(animationClip, animationClipPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Save textures in the parent directory
        
    }

}
