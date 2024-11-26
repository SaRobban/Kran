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
    private void OnCollisionEnter(Collision collision)
    {
        if (!spawnIron.magnetOn)
            return;

        if (collision.collider == spawnIron.magnet)
        {
            rb.isKinematic = true;
            transform.parent = spawnIron.magnet.transform;
        }

        if (collision.collider.TryGetComponent(out Rigidbody otherRB))
        {
            if (otherRB.isKinematic == true)
            {
                rb.isKinematic = true;
                transform.parent = otherRB.transform;
            }
        }
    }
}
