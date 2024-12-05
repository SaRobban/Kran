using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronCubeCode : MonoBehaviour
{
    public Rigidbody rb;
    public SpawnIron spawnIron;

    // Start is called before the first frame update
    public void Setup(Rigidbody rb, SpawnIron mother)
    {
        this.rb = rb;
        this.spawnIron = mother;
    }
}
