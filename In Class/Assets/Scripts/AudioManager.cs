using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioLookup audioLookup;

    public void PlaySound(string soundName)
    {
        AudioClip audioClip = audioLookup.GetAudioClip(soundName);
        GameObject newGameObject = new GameObject(soundName + " Sound SFX");
        AudioSource audioSource = newGameObject.AddComponent<AudioSource>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();


        Destroy(newGameObject, audioClip.length);
    }

}
