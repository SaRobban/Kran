using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class JointTest : MonoBehaviour
{

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

        public Vector3 Forward(Vector3 pos)
        {
            Vector3 dir = pos - anchor.position;
            float dist = dir.magnitude;

            if (dist > ropeLenght)
            {
                dir = dir.normalized;
                anchor.position = anchor.position + dir * ropeLenght;
            }
            return anchor.position;
        }
        public Vector3 Rewerse(Vector3 pos)
        {
            Vector3 dir = pos- anchor.position;
            float dist = dir.magnitude;

            if (dist > ropeLenght)
            {
                dir = dir.normalized;
                anchor.position = anchor.position + dir * ropeLenght;
            }
            return anchor.position;
        }



        public void Upd(float deltaTime)
        {
            Vector3 dir = rb.transform.position - anchor.position;
            float dist = dir.magnitude;

            if (dist > ropeLenght)
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

    Joint[] joints;
    int length = 10;

    public Transform jib;

    private void Start()
    {
        joints = new Joint[length];
        Transform parent = jib;
        for (int i = 0; i < length; i++)
        {
            GameObject go = new GameObject("Joint_" + i);
            go.transform.position = jib.transform.position - Vector3.up * i;
            go.transform.rotation = Quaternion.identity;
            Debug.DrawRay(go.transform.position, Vector3.right, Color.blue, 10);



            Rigidbody rb = go.AddComponent<Rigidbody>();
            if (i == length - 1)
                rb.mass *= 100;

            Joint joint = new Joint(parent, rb, 1.5f);
            joints[i] = joint;
            parent = rb.transform;
        }
    }

    private void FixedUpdate()
    {
        Vector3 start = jib.position;
        Vector3 end = joints[joints.Length - 1].anchor.position;

        for(int i = 0; i < 4; i++)
        {
            FK(start);
            IK(end);
        }

        Vector3 dline = joints[0].anchor.position;
        for(int i = 1; i<joints.Length; i++)
        {
            Vector3 dline2 = joints[i].anchor.position;
            Debug.DrawLine(dline, dline2);
            dline2 = dline;
        }
        
        joints[0].Upd(Time.fixedDeltaTime);
        for (int i = 1; i < joints.Length; i++)
        {
            joints[i].Upd(Time.fixedDeltaTime);
            Debug.DrawLine(joints[i-1].GetPoint(), joints[i].GetPoint(), Color.red);
        }
    }

    private void FK(Vector3 start)
    {
        for (int i = 0; i < joints.Length; i++)
        {
            start = joints[i].Forward(start);
        }
    }
    private void IK(Vector3 end)
    {
        joints[joints.Length-1].anchor.position = end;
        for(int i = joints.Length-1; i > 0; i--)
        {
            end = joints[i].Rewerse(end);
        }
    }
}
