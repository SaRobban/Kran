using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        float lengthToHook = 3;

        parts.Hook.transform.position = parts.Jib.position - Vector3.up * lengthToHook;
        parts.Hook.gameObject.GetComponent<Rigidbody>().sleepThreshold = 0;
        wireJointLimit = new SoftJointLimit();
        wireJointLimit.limit = lengthToHook;
        parts.WireJoint.linearLimit = wireJointLimit;
    }
    // Update is called once per frame
    public void Tick(float deltaTime)
    {
        if (Mathf.Abs(master.input.c_Winch) < Mathf.Epsilon)
            return;

        Rigidbody hook = parts.WireJoint.connectedBody;
        parts.WireJoint.connectedBody = null;
        wireJointLimit.limit += master.input.c_Winch;
        parts.WireJoint.linearLimit = wireJointLimit;

        Vector3 dir = hook.position- parts.Jib.position;
        hook.transform.position += dir.normalized * master.input.c_Winch;

        parts.WireJoint.connectedBody = hook;
    }
}
