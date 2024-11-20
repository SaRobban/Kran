using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneControls : MonoBehaviour
{
    [SerializeField] private GameObject wirePreFab;
    [Header("Parts")]
    [SerializeField] private Transform tower;
    [SerializeField] private Transform cabin;
    [SerializeField] private Transform boom;
    [SerializeField] private Transform jib;
    [SerializeField] private Transform hook;
    [SerializeField] private Transform[] wirePath;

    [SerializeField] private float towerSpeed = 40;
    [SerializeField] private float boomSpeed = 40;
    [SerializeField] private float hookSpeed = 40;

    [SerializeField] private float wireLength = 50;
    class CraneInput
    {
        CraneControls owner;
        public CraneInput(CraneControls owner)
        {
            this.owner = owner;
        }

        public Quaternion c_RotateCabin => RotateCabin();
        public Quaternion c_RotateBoom => RotateBoom();
        public float c_hook => Input.mouseScrollDelta.y;
        public float c_move => HookControl();

        float HookControl()
        {
            if (Input.GetButtonDown("Fire1"))
                return 1;
            if (Input.GetButtonDown("Fire2"))
                return -1;
            return 0;
        }

        Quaternion RotateCabin()
        {
            return Quaternion.Euler(0, Input.GetAxis("Horizontal") * owner.towerSpeed * Time.deltaTime, 0);
        }

        Quaternion RotateBoom()
        {
            return Quaternion.Euler(Input.GetAxis("Vertical") * owner.boomSpeed * Time.deltaTime, 0, 0);
        }

    }
    CraneInput craneInput;

    class Wire
    {
        CraneControls owner;
        float[] wireLength;
        Vector3 scale;
        Transform[] wire;

        public Wire(CraneControls owner)
        {
            this.owner = owner;
            wireLength = new float[owner.wirePath.Length];
            wire = new Transform[wireLength.Length - 1];
            

            scale = Vector3.one;

            float c = 0;
            int a = 0;
            for (int b = 1; b < wireLength.Length; b++)
            {
                Vector3 dir = owner.wirePath[a].position - owner.wirePath[b].position;
                wire[a] = Instantiate(owner.wirePreFab, owner.wirePath[a].position, Quaternion.identity).transform;
                wire[a].up = dir.normalized;
                scale.y = dir.magnitude;
                c += scale.y;
                wire[a].localScale = scale;
                a = b;
            }
            Debug.Log(wire.Length);
            owner.wireLength = c;
        }

        public void UpdatePathLength()
        {
            float c = owner.wireLength;
            int a = 0;
            for (int b = 1; b < wireLength.Length-1; b++)
            {
                wire[a].position = owner.wirePath[a].position;
                Vector3 dir = owner.wirePath[a].position - owner.wirePath[b].position;
                wire[a].up = dir.normalized;
                float mag = dir.magnitude;
                scale.y = mag;
                wire[a].localScale = scale;
                c -= mag;
                a = b;
            }
            scale.y = c;
            wire[wire.Length - 1].position = owner.wirePath[wire.Length - 1].position;
            wire[wire.Length - 1].up = Vector3.up;
            wire[wire.Length - 1].localScale = scale;
        }


    }
    Wire wireMessure;
    // Start is called before the first frame update
    void Start()
    {
        craneInput = new CraneInput(this);
        wireMessure = new Wire(this);
    }

    // Update is called once per frame
    void Update()
    {
        cabin.rotation *= craneInput.c_RotateCabin;
        boom.rotation *= craneInput.c_RotateBoom;
        wireMessure.UpdatePathLength();
    }
}
