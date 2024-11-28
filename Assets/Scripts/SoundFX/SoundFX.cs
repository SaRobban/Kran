using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public AudioSource defaultSource;
    public AudioClip[] gearBoxSounds;

    private void Awake()
    {
        defaultSource = GetComponent<AudioSource>();
        SoundManager.Init(this);
    }
    public void PlaySound(AudioSource source = null)
    {

    }
}


public static class SoundManager
{
    static SoundFX soundFX;
    public static void Init(SoundFX soundPool)
    {
        soundFX = soundPool;
    }

    public static void PlayGearBox(AudioSource source)
    {
        int radomSound = (int)Random.Range(0, soundFX.gearBoxSounds.Length);
        source.PlayOneShot(soundFX.gearBoxSounds[radomSound]);
    }
}