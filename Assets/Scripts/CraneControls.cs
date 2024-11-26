using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneControls : MonoBehaviour
{
    interface I_CraneInput
    {
        Quaternion c_RotateCabin
        {
            get;
        }
        Quaternion c_RotateBoom
        {
            get;
        }
        float c_Hook
        {
            get;
        }
        float c_Move
        {
            get;
        }
    }


    /*
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
    */
    class CraneInput : I_CraneInput
    {
        public class ControlCurve
        {
            public float rawValue = 0;
            public float result;
            /// <summary>
            /// I Know this is messy but it allows for realtime edit in editor
            /// </summary>
            public float Update(AnimationCurve curve, float change, float max, float drag, float delta)
            {
                rawValue *= (1 - drag * delta);
                rawValue += change * delta;
                rawValue = Mathf.Clamp(rawValue, -1, 1);

                if (rawValue < 0)
                    result = curve.Evaluate(Mathf.Abs(rawValue)) * -max;
                else
                    result = curve.Evaluate(rawValue) * max;

                return result;
            }
        }

        CraneControls owner;
        public CraneInput(CraneControls owner)
        {
            this.owner = owner;
            curveCabinControl = new ControlCurve();
            curveBoomControl = new ControlCurve();
            curveHookControl = new ControlCurve();
        }

        public Quaternion c_RotateCabin => RotateCabin();
        public Quaternion c_RotateBoom => RotateBoom();
        public float c_Hook => HookControl();
        public float c_Move => MoveCrane();

        public ControlCurve curveCabinControl;
        public ControlCurve curveBoomControl;
        public ControlCurve curveHookControl;
        float HookControl()
        {
            int dir = 0;
            if (Input.GetButton("Fire1"))
            {
                dir = -1;
            }
            if (Input.GetButton("Fire2"))
            {
                dir = 1;
            }

            float raiseHook = curveHookControl.Update(
                    owner.HookControlCurve,
                    dir,
                    owner.hookMaxSpeed,
                    owner.hookDrag,
                    Time.deltaTime
                    );

            return raiseHook;
        }

        Quaternion RotateCabin()
        {
            float input = Input.GetAxis("Horizontal");

            float rotation = curveCabinControl.Update(
                owner.cabinControlCurve,
                input,
                owner.cabinMaxSpeed,
                owner.cabinDrag,
                Time.deltaTime
                );
            return Quaternion.Euler(0, rotation * Time.deltaTime, 0);
        }
        Quaternion RotateBoom()
        {
            float input = Input.GetAxis("Vertical");

            float rotation = curveBoomControl.Update(
                owner.boomControlCurve,
                input,
                owner.boomMaxSpeed,
                owner.boomDrag,
                Time.deltaTime
                );
            return Quaternion.Euler(rotation * Time.deltaTime, 0, 0);
        }

        float MoveCrane()
        {
            return 0;
        }
    }
    CraneInput craneInput;

    [Header("Input")]
    [Header("Cabin")]
    public AnimationCurve cabinControlCurve;
    public float cabinMaxSpeed = 20;
    public float cabinDrag = 0.25f;
    [Header("Boom")]
    public AnimationCurve boomControlCurve;
    public float boomMaxSpeed = 10;
    public float boomDrag = 2;
    [Header("Hook")]
    public AnimationCurve HookControlCurve;
    public float hookMaxSpeed = 10;
    public float hookDrag;

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

    public ConfigurableJoint wireJoint;

    ConfigurableJoint[] joints;
    // Start is called before the first frame update
    void Start()
    {
        craneInput = new CraneInput(this);
        wireMessure = new Wire(this);
        hook.transform.position = jib.position - Vector3.up * wireMessure.GetLastLenght();
    }

    void SetupJoints()
    {

        int numberOfSegments = 7;

        joints = new ConfigurableJoint[numberOfSegments];

        Vector3 startPos = jib.transform.position;
        Vector3 goalPos = hook.transform.position;

        Vector3 dir = goalPos - startPos;
        float fullDist = dir.magnitude;
        float segmentDist = fullDist / numberOfSegments;
        dir = dir.normalized;

        Rigidbody lastBody = jib.GetComponent<Rigidbody>();
        for (int i = 0; i < numberOfSegments; i++)
        {
            Vector3 nextPos = startPos + dir * (i + 1) * segmentDist;
            Rigidbody segmentRB = CreateRopeSegmentBodyAt(nextPos);
            joints[i] = CreateJoint(lastBody, segmentRB);
            //   joints[i].anchor = dir * segmentDist*-0.5f;

            lastBody = segmentRB;
        }
        // lastBody.isKinematic = true;
        // lastBody.transform.parent = jib;

        Debug.Log(joints.Length + " long and");
    }

    Rigidbody CreateRopeSegmentBodyAt(Vector3 pos)
    {
        Transform t = new GameObject("Segment").transform;
        t.position = pos;
        Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
        return rb;
    }

    ConfigurableJoint CreateJoint(Rigidbody rbSource, Rigidbody rbAnchor)
    {
        ConfigurableJoint joint = rbSource.gameObject.AddComponent<ConfigurableJoint>();
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.connectedBody = rbAnchor;
        return joint;
    }

    // Update is called once per frame
    void Update()
    {
        cabin.rotation *= craneInput.c_RotateCabin;
        boom.rotation *= craneInput.c_RotateBoom;
        wireLength += craneInput.c_Hook;
        wireMessure.UpdatePathLength();
        wireJoint.connectedAnchor = Vector3.up* wireMessure.GetLastLenght();
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(0, 0, 200, 20), "RAW : " + craneInput.curveCabinControl.rawValue + "  EV : " + craneInput.curveCabinControl.result.ToString());
    }
}
