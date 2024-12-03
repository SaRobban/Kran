using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CraneParts))]
[RequireComponent(typeof(CraneContols))]
[RequireComponent (typeof(MoveCrane))]
[RequireComponent(typeof(CraneWire))]
[RequireComponent(typeof(CraneWinch))]
[RequireComponent(typeof(CraneSounds))]
public class CraneMaster : MonoBehaviour
{

    public CraneParts parts;
    public CraneContols input;
    public MoveCrane move;
    public CraneWire wire;
    public CraneWinch winch;
    public CraneSounds sounds;
    // Start is called before the first frame update
    void Start()
    {
        parts = GetComponent<CraneParts>();
        input = GetComponent<CraneContols>();
        input.Init();
        move = GetComponent<MoveCrane>();
        move.Init(this);
        wire = GetComponent<CraneWire>();
        wire.Init();
        winch = GetComponent<CraneWinch>();
        winch.Init(this);
        sounds = GetComponent<CraneSounds>();
        sounds.Init(this);
}

    // Update is called once per frame
    void Update()
    {
        input.Tick(Time.deltaTime);
        move.Tick(Time.deltaTime);
        wire.Tick(Time.deltaTime);
        winch.Tick(Time.deltaTime);
    }
}