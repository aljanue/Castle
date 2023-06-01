using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraslacionPuerta : MonoBehaviour
{
    public float v = 1;
    private Vector3 pos_inicial, pos_inicialbarra;
    private Quaternion pos_inicialmanivela;
    private double umbral_max;
    private double umbral_barra;
    private bool cerrarPuerta, barraPuesta, puertaAbajo;
    private float rot_positiva = 0, rot_negativa = 0;
    public GameObject puerta;
    public GameObject manivela;
    public GameObject barra;
    public GameObject cuerda;
    public GameObject peso;
    public Material materialCuerda;

    void Start()
    {
        //Almacenamos posiciones iniciales
        pos_inicial = puerta.transform.position; 
        pos_inicialmanivela = manivela.transform.rotation;
        pos_inicialbarra = barra.transform.position;
        //Almacenamos posiciones finales
        umbral_max = pos_inicial.y + 3.8;
        umbral_barra = pos_inicialbarra.x-3;
        //Inicializamos variables booleanas
        cerrarPuerta = false;
        barraPuesta = true;
        puertaAbajo = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Creamos patrones de movimiento
        Vector3 traslacion_vec = Vector3.zero;
        Vector3 traslacion_vecbarra = Vector3.zero;

        traslacion_vec += (Vector3.up * Time.deltaTime * v);
        traslacion_vecbarra += (Vector3.right * Time.deltaTime * v);
        rot_positiva += (360.0f / v) * Time.deltaTime;
        rot_negativa -= (360.0f / v) * Time.deltaTime;

        //Vemos si la puerta está bajada
        if(puerta.transform.position.y > pos_inicial.y)
            puertaAbajo = false;
        else
            puertaAbajo=true;

        //Vemos si la barra está puesta
        if(barra.transform.position.x < umbral_barra)
            barraPuesta = false;
        else
            barraPuesta = true;


        //Desplazar barra (al pulsar la tecla O, siempre que no se haya llegado al tope y que la puerta esté abajo)
        if(Input.GetKey(KeyCode.O) && barra.transform.position.x >= umbral_barra && puertaAbajo){
            barra.transform.position -= traslacion_vecbarra;
        }  
         
        //Colocar barra (al pulsar la tecla P, siempre que no se haya llegado al tope y que la puerta esté abajo)
        if(Input.GetKey(KeyCode.P) && barra.transform.position.x <= pos_inicialbarra.x && puertaAbajo){
            barra.transform.position += traslacion_vecbarra;
        }
     
        //Subir puerta (al pulsar la tecla U, siempre que no se haya llegado al tope y se haya quitado la barra)
        if(Input.GetKey(KeyCode.U) && puerta.transform.position.y <= umbral_max && !barraPuesta){
            puerta.transform.position += traslacion_vec;
            manivela.transform.rotation = Quaternion.AngleAxis(rot_positiva, new Vector3(0,0,1));
            peso.transform.position -=traslacion_vec;
            cuerda.transform.localScale +=traslacion_vec/2;
            cuerda.transform.localPosition -=traslacion_vec/2;
        }  
            
        
        //Comprobación si ha llegado la puerta al umbral máximo al soltar la tecla
        if(Input.GetKeyUp(KeyCode.U) || Input.GetKeyUp(KeyCode.I))
        {
            if (puerta.transform.position.y < umbral_max)
                cerrarPuerta = true;
            else
                cerrarPuerta = false;
        }

        //Bajar puerta (al pulsar la tecla I, siempre que no hayamos llegado a la posición inicial)
        if(Input.GetKey(KeyCode.I) && puerta.transform.position.y > pos_inicial.y && !barraPuesta){
            puerta.transform.position -= traslacion_vec;
            manivela.transform.rotation = Quaternion.AngleAxis(rot_negativa, new Vector3(0,0,1));
            peso.transform.position +=traslacion_vec;
            cuerda.transform.localScale -=traslacion_vec/2;
            cuerda.transform.localPosition +=traslacion_vec/2;
            cerrarPuerta = true; // si se decide volver a cerrar una vez hemos llegado arriba, cae sola
        }
            
        
        //Cerrar puerta si no se ha alcanzado el umbral maximo de apertura
        if (cerrarPuerta && puerta.transform.position.y > pos_inicial.y && !barraPuesta){
            puerta.transform.position -= traslacion_vec;
            manivela.transform.rotation = Quaternion.AngleAxis(rot_negativa, new Vector3(0,0,1));
            peso.transform.position +=traslacion_vec;
            cuerda.transform.localScale -=traslacion_vec/2;
            cuerda.transform.localPosition +=traslacion_vec/2;
            //Si mientras baja la puerta, se presiona la tecla de subida, hacemos que vuelva a subir
             if(Input.GetKey(KeyCode.U) && puerta.transform.position.y <= umbral_max){
                puerta.transform.position += traslacion_vec;
                manivela.transform.rotation = Quaternion.AngleAxis(rot_positiva, new Vector3(0,0,1));
                peso.transform.position -=traslacion_vec;
                cuerda.transform.localScale +=traslacion_vec/2;
                cuerda.transform.localPosition -=traslacion_vec/2;
            }  
            // Si al caer la puerta, su posición excede la posición inicial, la corregimos para seguir con la apertura y cierre de la puerta
            if (puerta.transform.position.y <= pos_inicial.y)
            {
                puerta.transform.position = pos_inicial;
                cerrarPuerta = false;
                
            }
        }
    }
}
