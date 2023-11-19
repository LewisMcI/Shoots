using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public AudioLookup audioLookup;
    public static AudioManager instance= null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySound(string soundName)
    {
        Debug.Log("Playing sound");
        AudioClip audioClip = audioLookup.GetAudioClip(soundName);
        GameObject newGameObject = new GameObject(soundName + " Sound SFX");
        AudioSource audioSource = newGameObject.AddComponent<AudioSource>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();

        Destroy(newGameObject, audioClip.length);
    }

    public void PlaySoundToAll(string soundName)
    {
        Debug.Log("Trying play sound all");
        PlaySoundServerRpc(soundName);
    }

    [ServerRpc(RequireOwnership=false)]
    private void PlaySoundServerRpc(string soundName)
    {
        Debug.Log("Server: Trying play sound all");
        // Tell Players to play sound
        PlaySoundClientRpc(soundName);
    }

    [ClientRpc]
    private void PlaySoundClientRpc(string soundName)
    {
        Debug.Log("Client: Trying play sound all");
        PlaySound(soundName);
    }

}
