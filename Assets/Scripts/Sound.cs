using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;
    [Range(0f, 1f)]
    public float volumn;
    public AudioClip audioClip;
}
