using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuerdas : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Hacemos que miren constantemente al objeto
        transform.LookAt(target);
        //Calculamos la longitud que recorren
        float longitud = (target.position-transform.position).magnitude;
        Vector3 v =new Vector3();
        v.x=1;
        v.y=1;
        v.z=longitud;
        //Escalamos dicha longitud
        transform.localScale = v;

    }
}
