using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CraneContols))]
[RequireComponent(typeof(CraneSounds))]
public class CraneMaster : MonoBehaviour
{
    
    public CraneContols input;
    public CraneSounds sounds;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<CraneContols>();
        input.Init();
        sounds = GetComponent<CraneSounds>();
        sounds.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        input.Tick(Time.deltaTime);
    }
}