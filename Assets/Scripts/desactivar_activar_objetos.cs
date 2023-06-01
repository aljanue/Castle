using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class desactivar_activar_objetos : MonoBehaviour
{
    public Camera camaraExterior;
    public Camera camaraTrono;
    public Camera camaraUsuario;
    public Light luzAntorcha;

    void Update()
    {
        // Activar/desactivar la cámara exterior con la tecla 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camaraUsuario.enabled=false;
            camaraExterior.enabled = true;
            camaraTrono.enabled = false;
        }

        // Activar/desactivar la cámara del trono con la tecla 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camaraUsuario.enabled=false;
            camaraExterior.enabled = false;
            camaraTrono.enabled = true;
        }
        
        // Activar/desactivar la cámara del jugador con la tecla 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            camaraUsuario.enabled=true;
            camaraExterior.enabled = false;
            camaraTrono.enabled = false;
        }

        // Activar/desactivar la antorcha con la tecla L
        if (Input.GetKeyDown(KeyCode.L))
        {
            luzAntorcha.enabled = !luzAntorcha.enabled;
        }
    }
}

