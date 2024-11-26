using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnIron : MonoBehaviour
{
    public Collider magnet;
    public float range = 4;
    public GameObject ironCube;
    public IronCubeCode[] irons;

    public bool magnetOn;
    // Start is called before the first frame update
    void Awake()
    {
        irons = new IronCubeCode[100];
        for (int i = 0; i < irons.Length; i++)
        {
            GameObject cube = Instantiate(ironCube, transform.position * 1 + Random.insideUnitSphere * 1, Quaternion.identity);
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            IronCubeCode code = cube.GetComponent<IronCubeCode>();
            code.Setup(rb, this);
            irons[i] = code;
        }
    }

    public void MagnetAktiv(bool on)
    {
        magnetOn = on;
    }

    // Update is called once per frame
    void Update()
    {
        magnet.transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))*Time.deltaTime;

        Vector3 point = magnet.transform.position;
        float aqrRange = range * range;
        if (magnetOn)
        {
            for (int i = 0; i < irons.Length; i++)
            {
                if (irons[i].rb.isKinematic)
                    continue;

                Vector3 dir = point - irons[i].transform.position;
                float dist = -dir.sqrMagnitude + aqrRange;
                if (dist > 0)
                {
                    float force = dist / aqrRange;
                    irons[i].rb.AddForce(dir.normalized * 18 * force, ForceMode.Acceleration);
                }
            }
        }
        if (!magnetOn) { 
            foreach (IronCubeCode cube in irons)
            {
                cube.transform.parent = null;
                cube.rb.isKinematic = false;
            }
        }
    }
}
