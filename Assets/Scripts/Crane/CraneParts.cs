using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneParts : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Transform tower;
    [SerializeField] private Transform cabin;
    [SerializeField] private Transform boom;
    [SerializeField] private Transform jib;
    [SerializeField] private Transform hook;

    [Header("Special Parts")]
    [SerializeField] private ConfigurableJoint wireJoint;

    public Transform Tower { get { return tower; } }
    public Transform Cabin { get { return cabin; } }
    public Transform Boom { get { return boom; } }
    public Transform Jib { get { return jib; } }
    public Transform Hook { get { return hook; } }

    public ConfigurableJoint WireJoint { get { return wireJoint; } }
}
