using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagoonMetallCounter : MonoBehaviour
{
    Collider collider;

    List<Collider> pices;

    [SerializeField] winSenario winSenario;

    private void Awake()
    {
        pices = new List<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (pices.Contains(other))
            return;

        pices.Add(other);

        winSenario.DidIWin(pices.Count);
    }

    private void OnTriggerExit(Collider other)
    {
        if (pices.Contains(other))
            pices.Remove(other);
    }
    private void OnCollisionEnter(Collision collision)
    {

    }
    private void OnCollisionExit(Collision collision)
    {

    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(10, 600, 200, 30), "Pics : " + pices.Count.ToString());
    }
}
