using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioLookup : MonoBehaviour
{
    // List of audios and their names
    private Dictionary<string, AudioClip> audioDataTable = new Dictionary<string, AudioClip>();
    public AudioDataObject audioData;

    // Populate the lookup table from the Sprite Data asset.
    private void Awake()
    {
        InitializeAudioDictionary(audioDataTable, audioData);
    }

    private void InitializeAudioDictionary(Dictionary<string, AudioClip> dataTable, AudioDataObject data)
    {
        if (data == null)
        {
            Debug.LogError("Sprite Data asset is not assigned.");
            return;
        }


        AudioClip[] audioClip = data.audioClip;
        for (int i = 0; i < audioClip.Length; i++)
        {
            dataTable.Add(data.audioName[i], audioClip[i]);
        }
    }


    public AudioClip GetAudioClip(string name)
    {
        if (audioDataTable.ContainsKey(name))
        {
            return audioDataTable[name];
        }
        else
        {
            Debug.LogError("Audio not found for name: " + name);
            return null;
        }
    }
}
