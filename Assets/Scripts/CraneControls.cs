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

    class Joint
    {
        public Transform anchor;
        public Rigidbody rb;
        float ropeLenght = 3;
        float damping = 1;
        public Joint(Transform parent, Rigidbody rb, float ropeLenght)
        {
            this.anchor = parent;
            this.rb = rb;
            this.ropeLenght = ropeLenght;
        }

        public void Upd(float ropeLength, float deltaTime)
        {
            Vector3 dir = rb.transform.position - anchor.position;
            float dist = dir.magnitude;

            if (dist > ropeLength)
            {
                dir = dir.normalized;

                Plane plane = new Plane(dir, anchor.position);
                Vector3 planeVelocity = plane.ClosestPointOnPlane(rb.position + rb.velocity) - anchor.position;

                rb.position = anchor.position + dir * ropeLenght;
                rb.velocity = planeVelocity * (1 - damping * deltaTime);
                //   Debug.DrawRay(rb.position, planeVelocity, Color.green);
            }
        }

        public Vector3 GetPoint()
        {
            return rb.transform.position;
        }
    }
    private Joint joint;
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
            for (int b = 1; b < wireLength.Length - 1; b++)
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

        public float GetLastLenght()
        {
            return wire[wire.Length - 1].localScale.y;
        }

    }
    Wire wireMessure;
    // Start is called before the first frame update
    void Start()
    {
        craneInput = new CraneInput(this);
        wireMessure = new Wire(this);

        GameObject go = new GameObject("HookDummy");
        go.transform.position = jib.position - Vector3.up * wireMessure.GetLastLenght();
        Rigidbody rb = go.AddComponent<Rigidbody>();
        joint = new Joint(jib, rb, wireMessure.GetLastLenght());
    }

    // Update is called once per frame
    void Update()
    {
        cabin.rotation *= craneInput.c_RotateCabin;
        boom.rotation *= craneInput.c_RotateBoom;
        wireMessure.UpdatePathLength();
//    }

//    private void FixedUpdate()
//    {
        joint.Upd(wireMessure.GetLastLenght(), Time.fixedDeltaTime);
        hook.position = joint.GetPoint();
    }
}
