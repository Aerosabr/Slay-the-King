using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
	void Awake()
	{
		EnsureSingleAudioListener();
	}

	private void EnsureSingleAudioListener()
	{
		// Find all AudioListener components in the scene
		AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();

		// If there is more than one, disable the extras
		if (audioListeners.Length > 1)
		{
			Debug.LogWarning("Multiple AudioListeners found! Disabling extras.");

			for (int i = 1; i < audioListeners.Length; i++)
			{
				audioListeners[i].enabled = false;
			}
		}
	}
}
