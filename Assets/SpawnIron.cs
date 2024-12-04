using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnIron : MonoBehaviour
{
    public CraneMaster craneMaster;
    public int numberOfPices = 1000;
    public Collider magnet;
    public float range = 4;
    public GameObject ironCube;
    public IronCubeCode[] irons;

    public bool magnetOn;
    public float kinematicInsideRange = 1;

    // Start is called before the first frame update
    void Awake()
    {
        irons = new IronCubeCode[numberOfPices];
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
        magnet.transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime;

        Vector3 point = magnet.transform.position;
        float aqrRange = range * range;
        //TODO : Toggle
        if (craneMaster.input.c_magnet)
        {
            for (int i = 0; i < irons.Length; i++)
            {
                if (irons[i].rb.isKinematic)
                    continue;

                Vector3 dir = point - irons[i].transform.position;
                float sqrdist = dir.sqrMagnitude;
                float magForce = aqrRange - sqrdist;
                if (magForce > 0)
                {
                    float force = magForce / aqrRange;
                    irons[i].rb.AddForce(dir.normalized * 18 * force, ForceMode.Acceleration);
                }


                if (sqrdist < kinematicInsideRange)
                {
                    if (Vector3.Dot(dir, irons[i].rb.velocity) < 0)
                    {
                        irons[i].rb.isKinematic = true;
                        irons[i].transform.parent = magnet.transform;
                    }
                }

            }
            magnetOn = true;
        }
        if (!craneMaster.input.c_magnet && magnetOn == true)
        {
            foreach (IronCubeCode cube in irons)
            {
                cube.transform.parent = null;
                cube.rb.isKinematic = false;
                magnetOn = false;
            }
        }
    }
    /*
    private void OnGUI()
    {
        GUI.TextField(new Rect(10, 300,200,100), "Magnet : "+ craneMaster.input.c_magnet);
    }
    /*/
}
