using UnityEngine;

public class OLD_CraneControls : MonoBehaviour
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
        Vector3 c_Move
        {
            get;
        }
    }
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
        OLD_CraneControls owner;
        public CraneInput(OLD_CraneControls owner)
        {
            this.owner = owner;
            curveCabinControl = new ControlCurve();
            curveBoomControl = new ControlCurve();
            curveHookControl = new ControlCurve();
            curveMoveControl = new ControlCurve();
        }

        public Quaternion c_RotateCabin => RotateCabin();
        public Quaternion c_RotateBoom => RotateBoom();
        public float c_Hook => HookControl();
        public Vector3 c_Move => MoveCrane();

        public bool c_magnet => MagnetControl();
        private bool magnetControl;

        public ControlCurve curveCabinControl;
        public ControlCurve curveBoomControl;
        public ControlCurve curveHookControl;
        public ControlCurve curveMoveControl;
        
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

        Vector3 MoveCrane()
        {
            float input = curveMoveControl.Update(
                owner.craneCurve,
                Input.GetAxis("Vertical"),
                owner.craneMoveSpeed,
                owner.craneDrag,
                Time.deltaTime
                );
            return owner.craneAxis * input * Time.deltaTime;
        }

        bool MagnetControl()
        {
            if (Input.GetButtonDown("A"))
                magnetControl = true;
            if (Input.GetButtonDown("Z"))
                magnetControl = false;
            return magnetControl;

        }
    }
    CraneInput craneInput;

    [Header("Input")]
    [Header("TowerMove")]
    public AnimationCurve craneCurve;
    public float craneMoveSpeed = 5;
    public float craneDrag = 0.02f;
    Vector3 craneAxis = Vector3.forward;
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

    [Header("HookFunctions")]
    public SpawnIron magnet;

    class Wire
    {
        OLD_CraneControls owner;
        float[] wireLength;
        Vector3 scale;
        Transform[] wire;

        public Wire(OLD_CraneControls owner)
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


        public void ConnectLast(Vector3 jib, Vector3 hookPos)
        {
            Vector3 dir = hookPos - jib;
            wire[wire.Length - 1].position = hookPos;
            wire[wire.Length - 1].up = dir.normalized;
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
    SoftJointLimit wireJointLimit;

    ConfigurableJoint[] joints;
    // Start is called before the first frame update
    void Start()
    {
        craneInput = new CraneInput(this);
        wireMessure = new Wire(this);
        hook.transform.position = jib.position - Vector3.up * wireMessure.GetLastLenght();
        hook.gameObject.GetComponent<Rigidbody>().sleepThreshold = 0;
        wireJointLimit = new SoftJointLimit();
        wireJointLimit.limit = wireMessure.GetLastLenght();
        wireJoint.linearLimit = wireJointLimit;

    }

    // Update is called once per frame
    void Update()
    {
        tower.position += craneInput.c_Move;
        cabin.rotation *= craneInput.c_RotateCabin;
        boom.rotation *= craneInput.c_RotateBoom;
        wireLength += craneInput.c_Hook;
        wireMessure.UpdatePathLength();
        wireJointLimit.limit = wireMessure.GetLastLenght();
        wireJoint.linearLimit = wireJointLimit;

        wireMessure.ConnectLast(jib.position, hook.position);

        magnet.magnetOn = craneInput.c_magnet;
    }
    /*
    private void OnGUI()
    {
        GUI.TextField(new Rect(0, 0, 200, 20), "RAW : " + craneInput.curveCabinControl.rawValue + "  EV : " + craneInput.curveCabinControl.result.ToString());
    }
    */
}
