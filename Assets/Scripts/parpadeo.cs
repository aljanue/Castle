using UnityEngine;
using System.Collections;

public class parpadeo : MonoBehaviour {

    //Intensidad de la iluminación estara siempre entre 0 y 1
    private const float intensidad_min = 0;
    private const float intensidad_max = 1;

    // Velocidad de parpadeo de la luz
    public float v = 1;

    //Declaración de un objeto tipo Light
    private Light antorcha;

    void Start () {
       antorcha = GetComponent<Light>();
    }

    void Update () {
        float val = Mathf.PerlinNoise(Time.time * v, Time.time * v);

        // Interpolar el valor val obtenido entre 0 y 1
        antorcha.intensity = Mathf.Lerp(intensidad_min, intensidad_max, val);
    }
}
