using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaExterior : MonoBehaviour
{
    public float v = 0.3f;
    private Quaternion pos_inicial;
    private Quaternion pos_inicialmanivela;
    private double umbral_max;
    private bool caerPuerta;
    private float rotacion;
    public GameObject puerta;
    public GameObject manivela;
    public Material materialCuerda;
     
    void Start()
    {
        pos_inicial = puerta.transform.rotation; //Almacenamos la posicion inicial de la puerta
        pos_inicialmanivela = manivela.transform.localRotation;
        umbral_max = pos_inicial.x + Quaternion.AngleAxis(-15, Vector3.right).x;

        caerPuerta = false;
    }

    // Update is called once per frame
    void Update()
    {
     
        //Tirar puerta (al pulsar la tecla J, siempre que no se haya llegado al tope)
        if(Input.GetKey(KeyCode.J) && puerta.transform.rotation.x >= Quaternion.AngleAxis(-85, new Vector3(1,0,0)).x){
            rotacion -= (360.0f / v) * Time.deltaTime;
            puerta.transform.rotation = Quaternion.AngleAxis(rotacion, new Vector3(1,0,0));
            manivela.transform.localRotation = Quaternion.AngleAxis(rotacion, new Vector3(0,0,1));
        }  
           
        
        //Comprobación si ha llegado la puerta al umbral máximo al soltar la tecla
        if(Input.GetKeyUp(KeyCode.J) || Input.GetKeyUp(KeyCode.K))
        {
            if (puerta.transform.rotation.x <= umbral_max)
                caerPuerta = true;
            else
                caerPuerta = false;
        }

        //Recoger puerta (al pulsar la tecla K, siempre que no hayamos llegado a la posición inicial)
        if(Input.GetKey(KeyCode.K) && puerta.transform.rotation.x <= pos_inicial.x){
            rotacion += (360.0f / v) * Time.deltaTime;
            puerta.transform.rotation = Quaternion.AngleAxis(rotacion, new Vector3(1,0,0));
            manivela.transform.localRotation = Quaternion.AngleAxis(rotacion, new Vector3(0,0,1));
            caerPuerta = true; // si se decide volver a tirar una vez hemos llegado arriba, cae sola
        }
            
        
        //Tirar la puerta automáticamente si ha alcanzado el umbral maximo de apertura
        if (caerPuerta && puerta.transform.rotation.x < umbral_max){
            rotacion -= (360.0f / v) * Time.deltaTime;
            puerta.transform.rotation = Quaternion.AngleAxis(rotacion, new Vector3(1,0,0));
            manivela.transform.localRotation = Quaternion.AngleAxis(rotacion, new Vector3(0,0,1));
             
            if(Input.GetKey(KeyCode.J) && puerta.transform.rotation.x >= Quaternion.AngleAxis(-85, new Vector3(1,0,0)).x){
                rotacion -= (360.0f / v) * Time.deltaTime;
                puerta.transform.rotation = Quaternion.AngleAxis(rotacion, new Vector3(1,0,0));
                manivela.transform.localRotation = Quaternion.AngleAxis(rotacion, new Vector3(0,0,1));
            }
            if(Input.GetKey(KeyCode.K) && puerta.transform.rotation.x <= pos_inicial.x){
            rotacion += (360.0f / v) * Time.deltaTime;
            puerta.transform.rotation = Quaternion.AngleAxis(rotacion, new Vector3(1,0,0));
            manivela.transform.localRotation = Quaternion.AngleAxis(rotacion, new Vector3(0,0,1));
            caerPuerta = true; // si se decide volver a cerrar una vez hemos llegado arriba, cae sola
        }
            // Si al caer la puerta, su posición excede la posición inicial, la corregimos para seguir con la apertura y cierre de la puerta
            if (puerta.transform.rotation.x <= Quaternion.AngleAxis(-85, new Vector3(1,0,0)).x)
            {
                puerta.transform.rotation = Quaternion.AngleAxis(-85, new Vector3(1,0,0));
                caerPuerta = false;
                
            }
        }
     }
}
