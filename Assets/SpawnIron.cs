using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnIron : MonoBehaviour
{
    public Animator anim;

    public CraneMaster craneMaster;
    public int numberOfPices = 1000;
    public Collider magnet;
    public float range = 1;
    public GameObject ironCube;
    public IronCubeCode[] irons;

    public bool magnetOn;


    public Collider col;
    // Start is called before the first frame update
    void Awake()
    {
        col = magnet.GetComponent<Collider>();
                    Debug.Log(col.bounds.center);

        irons = new IronCubeCode[numberOfPices];
        for (int i = 0; i < irons.Length; i++)
        {
            GameObject cube = Instantiate(ironCube, transform.position * 1 + Random.insideUnitSphere * 1, Quaternion.identity);
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            IronCubeCode code = cube.GetComponent<IronCubeCode>();
            code.Setup(rb, this);
            irons[i] = code;
        }

        craneMaster.input.A_OnMagnet += Magnet;
    }
    private void OnDestroy()
    {
        craneMaster.input.A_OnMagnet -= Magnet;
    }
    public void MagnetAktiv(bool on)
    {
        magnetOn = on;
    }

    void Magnet()
    {
        if (magnetOn == false)
        {
            anim.SetBool("Open", false);

            Vector3 point = magnet.transform.position;
            float aqrRange = range * range;
            for (int i = 0; i < irons.Length; i++)
            {
                /*
                if (col.bounds.Contains(irons[i].transform.position))
                {
                    irons[i].rb.isKinematic = true;
                    irons[i].transform.parent = magnet.transform;
                }
                */
                Vector3 dir = point - irons[i].transform.position;
                float sqrdist = dir.sqrMagnitude;

                if (sqrdist < range * range)
                {
                    irons[i].rb.isKinematic = true;
                    irons[i].transform.parent = magnet.transform;
                }
            }
            magnetOn = true;
            return;
        }
        if (magnetOn == true)
        {
            anim.SetBool("Open", true);
            foreach (IronCubeCode cube in irons)
            {
                cube.transform.parent = null;
                cube.rb.isKinematic = false;
                magnetOn = false;
            }
        }
    }

}
