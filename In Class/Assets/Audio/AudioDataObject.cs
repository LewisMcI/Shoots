using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Custom/Audio Data")]
public class AudioDataObject : ScriptableObject
{
    public AudioClip[] audioClip;
    public string[] audioName;
}