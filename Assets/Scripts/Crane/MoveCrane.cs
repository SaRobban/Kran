using UnityEngine;

public class MoveCrane : MonoBehaviour
{
    CraneMaster master;
    CraneParts parts => master.parts;
    CraneContols contols => master.input;
    
    public void Init(CraneMaster master)
    {
        this.master = master;
    }
   
    // Update is called once per frame
    public void Tick(float deltaTime)
    {
        parts.Tower.transform.position += contols.c_Move;
        parts.Cabin.transform.rotation *= contols.c_RotateCabin;
        parts.Boom.transform.rotation *= contols.c_TiltBoom;
       
    }
}
