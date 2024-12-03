using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CraneWinch : MonoBehaviour
{
    CraneMaster master;
    CraneWire wire => master.wire;
    CraneParts parts => master.parts;
    SoftJointLimit wireJointLimit;
    // Start is called before the first frame update
    public void Init(CraneMaster master)
    {
        this.master = master;

        parts.Hook.transform.position = parts.Jib.position - Vector3.up * wire.totalLenght;
        parts.Hook.gameObject.GetComponent<Rigidbody>().sleepThreshold = 0;
        wireJointLimit = new SoftJointLimit();
        wireJointLimit.limit = wire.totalLenght;
        parts.WireJoint.linearLimit = wireJointLimit;
    }
    // Update is called once per frame
    public void Tick(float deltaTime)
    {
        wireJointLimit.limit += master.input.c_Winch;
        parts.WireJoint.linearLimit = wireJointLimit;
    }
}
