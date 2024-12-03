using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneWire : MonoBehaviour
{
    class Wire
    {
        CraneWire owner;
        Vector3 scale;
        Transform[] wireSegments;
        public Wire(CraneWire owner)
        {
            this.owner = owner;
            wireSegments = new Transform[owner.wirePath.Length - 1];
            
            scale = Vector3.one;

            float c = 0;
            int a = 0;
            for (int b = 1; b < owner.wirePath.Length; b++)
            {
                Vector3 dir = owner.wirePath[a].position - owner.wirePath[b].position;
                wireSegments[a] = Instantiate(owner.wirePrefab, owner.wirePath[a].position, Quaternion.identity).transform;
                wireSegments[a].up = dir.normalized;
                float segmentLength = dir.magnitude;
                scale.y = segmentLength;
                c += segmentLength;
                wireSegments[a].localScale = scale;
                a = b;
            }
            owner.totalLenght = c;
        }

        public void UpdatePathLength()
        {
            int a = 0;

            for(int b = 1; b < owner.wirePath.Length; b++)
            {
                wireSegments[a].position = owner.wirePath[a].position;
                Vector3 dir = owner.wirePath[a].position - owner.wirePath[b].position;
                wireSegments[a].up = dir.normalized;
                float mag = dir.magnitude;
                scale.y = mag;
                wireSegments[a].localScale = scale;
                a = b;
            }
        }

        public float GetLastLenght()
        {
            return wireSegments[wireSegments.Length - 1].localScale.y;
        }
    }
    Wire wireMessure;

    public Transform[] wirePath;
    public GameObject wirePrefab;
    public float totalLenght;

    public void Init()
    {
       wireMessure = new Wire(this);
    }

    public void Tick(float dt)
    {
        wireMessure.UpdatePathLength();
    }
}
