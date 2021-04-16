using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] backgroundMusic;
    public Sound[] humanSoundEffects;

    private AudioSource bgmPlayer;
    private AudioSource humanSoundPlayer;

    private Dictionary<string, Sound> bkMusicDict;
    private Dictionary<string, Sound> hmEffectDict;

    private void Start()
    {
        bkMusicDict = new Dictionary<string, Sound>();
        hmEffectDict = new Dictionary<string, Sound>();

        bgmPlayer = gameObject.AddComponent<AudioSource>();
        humanSoundPlayer = gameObject.AddComponent<AudioSource>();

        bgmPlayer.loop = true;
        humanSoundPlayer.loop = false;

        if(backgroundMusic != null)
        {
            foreach(Sound s in backgroundMusic)
            {
                bkMusicDict.Add(s.soundName, s);
            }
        }

        if(humanSoundEffects != null)
        {
            foreach(Sound s in humanSoundEffects)
            {
                hmEffectDict.Add(s.soundName, s);
            }
        }
    }

    private void Update()
    {
        /* Test Only
        if(Input.GetMouseButtonDown(0))
        {
            PlayMusic("bgm1", MusicType.BGM);
        }
        if (Input.GetMouseButtonDown(1))
        {
            PlayMusic("bgm2", MusicType.BGM);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayMusic("woo", MusicType.HumanSound);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayMusic("ohh", MusicType.HumanSound);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayMusic("ooo", MusicType.HumanSound);
        }
        */
    }

    public void PlayMusic(string musicName, MusicType musicType)
    {
        switch (musicType)
        {
            case MusicType.BGM:
                if (bkMusicDict.Count <= 0) { return; }
                AudioClip bkclip = bkMusicDict[musicName].audioClip;
                if(bkclip != null && bgmPlayer.clip != bkclip)
                {
                    bgmPlayer.clip = bkclip;
                    bgmPlayer.Play();
                }
                break;
            case MusicType.HumanSound:
                if (hmEffectDict.Count <= 0) { return; }
                AudioClip hmclip = hmEffectDict[musicName].audioClip;
                if (hmclip != null)
                {
                    humanSoundPlayer.clip = hmclip;
                }
                humanSoundPlayer.Play();
                break;
        }
    }
}

public enum MusicType
{
    BGM,
    HumanSound
}
