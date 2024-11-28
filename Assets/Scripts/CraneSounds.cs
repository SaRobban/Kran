using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneSounds : MonoBehaviour
{

    public CraneMaster master;
    public AudioSource audioSource;
    
    public void Init(CraneMaster master)
    {
        this.master = master;
        audioSource = GetComponent<AudioSource>();

        master.input.A_GearSwitch += PlayGearBox;
    }

    void PlayGearBox(CraneContols.Gears gear)
    {
        SoundManager.PlayGearBox(audioSource);
    }

}
