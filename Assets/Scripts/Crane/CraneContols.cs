using UnityEngine;

public class CraneContols : MonoBehaviour
{
    public enum Gears { Move, Rotate, Rilt, Wire };
    [SerializeField] private Gears gear = Gears.Move;
    public System.Action<Gears> A_GearSwitch;
    private class CraneForceSimulator
    {
        public class ControlCurve
        {
            public float rawValue = 0;
            public float result;
            /// <summary>
            /// I Know this is messy but it allows for realtime edit in editor
            /// </summary>
            public float Update(AnimationCurve curve, float gas, float brake, float max, float drag, float delta)
            {
                float slowDown = drag + brake;
                rawValue *= (1 - slowDown * delta);
                rawValue += gas * delta;
                rawValue = Mathf.Clamp(rawValue, -1, 1);
                if (rawValue < 0)
                    result = curve.Evaluate(Mathf.Abs(rawValue)) * -max;
                else
                    result = curve.Evaluate(rawValue) * max;

                return result;
            }
        }
        CraneContols owner;
        public CraneForceSimulator(CraneContols owner)
        {
            this.owner = owner;
            curveCabinControl = new ControlCurve();
            curveBoomControl = new ControlCurve();
            curveHookControl = new ControlCurve();
            curveMoveControl = new ControlCurve();
        }

        public Quaternion c_RotateCabin;
        public Quaternion c_TiltBoom;
        public float c_Winch;
        public Vector3 c_Move;
        public bool c_magnet;
        private bool magnetControl;

        private ControlCurve curveCabinControl;
        private ControlCurve curveBoomControl;
        private ControlCurve curveHookControl;
        private ControlCurve curveMoveControl;

        public void Update(float deltaTime)
        {
            float gas = Input.GetAxis("Horizontal");
            float yAxis = Input.GetAxis("Vertical");
            float brake = Mathf.Clamp(yAxis, -1, 0) * -1;
            bool shift = Input.GetKeyDown(KeyCode.UpArrow);

            UpdateValues(gas, brake, shift, deltaTime);
        }

        private void UpdateValues(float gas, float brake, bool shift, float deltaTime)
        {
            if (shift)
                Shift();

            float movePower = 0;
            float towerPower = 0;
            float boomPower = 0;
            float winchPower = 0;

            switch (owner.gear)
            {
                case CraneContols.Gears.Move:
                    movePower = gas;
                    break;

                case CraneContols.Gears.Rotate:
                    towerPower = gas;
                    break;

                case CraneContols.Gears.Rilt:
                    boomPower = gas;
                    break;

                case CraneContols.Gears.Wire:
                    winchPower = gas;
                    break;

                default:
                    break;
            }

            c_Move = MoveCrane(movePower, brake, deltaTime);
            c_RotateCabin = RotateCrane(towerPower, brake, deltaTime);
            c_TiltBoom = TiltBoom(boomPower, brake, deltaTime);
            c_Winch = WindWinch(winchPower, brake, deltaTime);
            c_magnet = MagnetControl();
        }

        void Shift()
        {
            switch (owner.gear)
            {
                case CraneContols.Gears.Move:
                    owner.gear = CraneContols.Gears.Rotate;
                    break;

                case CraneContols.Gears.Rotate:
                    owner.gear = CraneContols.Gears.Rilt;
                    break;

                case CraneContols.Gears.Rilt:
                    owner.gear = CraneContols.Gears.Wire;
                    break;

                case CraneContols.Gears.Wire:
                    owner.gear = CraneContols.Gears.Move;
                    break;

                default:
                    owner.gear = CraneContols.Gears.Move;
                    break;
            }
            owner.A_GearSwitch?.Invoke(owner.gear);
        }

        Vector3 MoveCrane(float gas, float brake, float deltaTime)
        {
            float input = curveMoveControl.Update(
                owner.craneCurve,
                gas,
                brake,
                owner.craneMoveSpeed,
                owner.craneDrag,
                deltaTime
                );
            return owner.craneAxis * input * deltaTime ;
        }
        Quaternion RotateCrane(float gas, float brake, float deltaTime)
        {
            float rotation = curveCabinControl.Update(
                owner.cabinControlCurve,
                gas ,
                brake,
                owner.cabinMaxSpeed,
                owner.cabinDrag,
                deltaTime
                );
            return Quaternion.Euler(0, rotation *deltaTime, 0);
        }
        Quaternion TiltBoom(float gas, float brake, float deltaTime)
        {
            float rotation = curveBoomControl.Update(
                owner.boomControlCurve,
                gas,
                brake,
                owner.boomMaxSpeed,
                owner.boomDrag,
                deltaTime
                );
            return Quaternion.Euler(rotation * deltaTime, 0, 0);
        }
        float WindWinch(float gas, float brake, float deltaTime)
        {
            float raiseHook = curveHookControl.Update(
                    owner.HookControlCurve,
                    gas,
                    brake,
                    owner.hookMaxSpeed,
                    owner.hookDrag,
                    deltaTime
                    );

            return raiseHook * deltaTime;
        }



        bool MagnetControl()
        {
            if (Input.GetButtonDown("A"))
            {
                magnetControl = true;
                owner.A_OnMagnet?.Invoke();
            }
            if (Input.GetButtonDown("Z"))
                magnetControl = false;
            return magnetControl;

        }
    }
    private CraneForceSimulator craneInput;

    [Header("Input")]
    [Header("TowerMove")]
    [SerializeField] private AnimationCurve craneCurve;
    [SerializeField] float craneMoveSpeed = 5;
    [SerializeField] float craneDrag = 0.02f;
    Vector3 craneAxis = Vector3.forward;
    [Header("Cabin")]
    [SerializeField] private AnimationCurve cabinControlCurve;
    [SerializeField] private float cabinMaxSpeed = 20;
    [SerializeField] private float cabinDrag = 0.25f;
    [Header("Boom")]
    [SerializeField] private AnimationCurve boomControlCurve;
    [SerializeField] private float boomMaxSpeed = 10;
    [SerializeField] private float boomDrag = 2;
    [Header("Hook")]
    [SerializeField] private AnimationCurve HookControlCurve;
    [SerializeField] private float hookMaxSpeed = 10;
    [SerializeField] private float hookDrag;


    public Quaternion c_RotateCabin => craneInput.c_RotateCabin;
    public Quaternion c_TiltBoom => craneInput.c_TiltBoom;
    public float c_Winch => craneInput.c_Winch;
    public Vector3 c_Move => craneInput.c_Move;
    public bool c_magnet => craneInput.c_magnet;
    public bool magnetControl => craneInput.c_magnet;

    public System.Action A_OnMagnet;

    public void Init()
    {
        craneInput = new CraneForceSimulator(this);
    }

    public void Tick(float deltaTime)
    {
        craneInput.Update(deltaTime);
    }
    /*
    public void OnGUI()
    {
        GUI.TextField(new Rect(10, 10, 200, 200), "Input : \n" + c_Move + "\n" + c_RotateCabin + "\n" + c_TiltBoom + "\n" + c_Winch);
    }
    /*/
}
