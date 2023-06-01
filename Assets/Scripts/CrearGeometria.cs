using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CrearGeometria : MonoBehaviour
{
    public enum formas 
    {
        Triangulo,
        Tira,
        Caja,
        Plano,
        Cubo,
        Cilindro,
        Esfera,
        Muro
    };
    public formas forma;
    public int particiones = 20;

    public List<Vector3> vertices;
    public List<Vector3> normals;
    public List<Vector2> textCoords;
    public List<int> triangles;

    // Variables públicas para la construcción de la caja
    public float TamX= 1;
    public float TamY= 1;
    public float TamZ = 1;
    
    // Variables públicas para la creación de un cilindro
    public float radio = 1;
    public float altura = 10;

    //Variables públicas para la creación de la almena
    public float altura_almena=0.3f;

    //Variables publicas para aplicar ruido Perlin
    public float escalaPerlin = 0.5f;
    public float amplitudPerlin = 0.1f;

    // Función generica para crear la geometría, se invoca desde el CustomInspector
    public void crear() 
    {
        Debug.Log("Crear geometría: " + forma);
        switch (forma) {
            // Triangulo
            case formas.Triangulo: 
                crearTriangulo(); 
                break;

            case formas.Tira:
                crearTira();
                break;

            case formas.Caja:
                crearCaja();
                break;

            case formas.Plano:
                crearPlano();
                break;

            case formas.Esfera:
                crearEsfera();
                break;

            case formas.Cilindro:
                crearCilindro();
                break;

            case formas.Muro:
                crearMuro();
                break;
            
            // ToDo: completar el resto de formas posibles
            // case formas.Cubo:
            // case formas.Cilindro:
        }
    }

    void crearTriangulo() 
    {
        // Crear los vertices
        vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 0, 1));
        vertices.Add(new Vector3(1, 0, 0));

        // Crear las normales de cada vertice
        normals = new List<Vector3>();
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));

        // Crear las coordenadas de textura de cada vertice
        textCoords = new List<Vector2>();
        textCoords.Add(new Vector2(0, 0));
        textCoords.Add(new Vector2(0, 1));
        textCoords.Add(new Vector2(1, 0));

        // Crear los triángulos, a partir de los indices de los vertices en el vector de vertices
        // Cuidado! orientación mano izquierda, vertices en sentido horario
        triangles = new List<int>();
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        // Crear una nueva malla de triángulos a partir de los datos anteriores
        crearMesh();
    }

    void crearTira() 
    {
        // Crear las listas para añadir vertices, normales, coordenadas de textura e indices de los triángulos
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        textCoords = new List<Vector2>();
        triangles = new List<int>();

        // Inicializar el contador de vertices
        int tamTira = particiones + 1;
        float tamParticion = 1.0f/particiones;

        /*
        Añadir los dos primeros vertices de la tira, después
        por cada iteración del bucle se añaden 2 vertices y
        se crean dos triangulos (T1 y T2) con los dos vertices 
        anteriores.

        1 - - 3 - - 5 - - 
        |T2 / |   / |
        |  /  |  /  |
        | / T1| /   |
        0 - - 2 - - 4 - - 

        */

        // - Vertice inferior
        vertices.Add(new Vector3(0,0,0));
        normals.Add(new Vector3(0,1,0));
        textCoords.Add(new Vector2(0,0));
        
        // - Vertice superior
        vertices.Add(new Vector3(0,0,tamParticion));
        normals.Add(new Vector3(0,1,0));
        textCoords.Add(new Vector2(0,1));

        for (int i=1; i<=particiones; i++) 
        {
            float X = tamParticion * i;
            
            // - Vertice inferior
            vertices.Add(new Vector3(X,0,0));
            normals.Add(new Vector3(0,1,0));
            textCoords.Add(new Vector2(X,0));

            // - Vertice superior
            vertices.Add(new Vector3(X,0,tamParticion));
            normals.Add(new Vector3(0,1,0));
            textCoords.Add(new Vector2(X,1));
            
            int currIndex = vertices.Count-1;

            // - Triangulo 1
            triangles.Add(currIndex);       //3
            triangles.Add(currIndex-1);     //2
            triangles.Add(currIndex-3);     //0

            // - Triangulo 2
            triangles.Add(currIndex);       //3
            triangles.Add(currIndex-3);     //0
            triangles.Add(currIndex-2);     //1
        }

        // Crear la mesh con los vectores calculados
        crearMesh();
    }

    void crearPlano() 
    {
        // Crear las listas para añadir vertices, normales, coordenadas de textura e indices de los triángulos
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        textCoords = new List<Vector2>();
        triangles = new List<int>();

        // Inicializar el contador de vertices
        int currIndex = 0;
        int tamTira = particiones + 1;

        // Calcular el tamaño de cada celda, 
        // al ser un cuadrado es igual en X y en Z
        float tamParticion = 1.0f/particiones;

        for (int i=0; i<=particiones; i++) {
            
            float Z = tamParticion * i;             // Calculo de la coordenada Z (filas)

            for (int j=0; j<=particiones; j++) {
                
                float X = tamParticion * j;          // Calculo de la coordenada X (columna)
                
                vertices.Add(new Vector3(X,0,Z));
                normals.Add(new Vector3(0,1,0));     // En el plano XZ el vector normal es el vector Y
                textCoords.Add(new Vector2(X,Z));

                if ((i>0) && (j>0)) {

                    // A partir de la primera fila y la primera columna, 
                    // cada vertice da lugar a 2 nuevos triángulos (sentido horario)
                    /*
                            currIndex - 1  -->              O --- X   <-- currIndex
                                                            | 1 / |
                                                            |  /  |
                                                            | / 2 |
                            currIndex - tamTira - 1  -->    O --- O   <-- currIndex - tamTira
                    
                    */

                    // Triángulo 1
                    triangles.Add(currIndex);
                    triangles.Add(currIndex - tamTira - 1);
                    triangles.Add(currIndex - 1);
                    
                    // Triángulo 2
                    triangles.Add(currIndex);
                    triangles.Add(currIndex - tamTira);
                    triangles.Add(currIndex - tamTira - 1);
                }
                
                // Incrementar el contador de vertices
                currIndex ++;
            }
        }

        // Crear una nueva malla de triángulos a partir de los datos anteriores
        crearMesh();
    }

    void crearEsfera() 
    {

        // Ecuación de la esfera por gajos
        // [U,V] en el rango [0,1]
        // x = radio * cos(2*PI*U) * sin(PI*V)
        // y = radio * cos(PI*V)
        // z = radio * sin(2*PI*U) * sin(PI*V)

        float radio = 1;

        // Crear las listas para añadir vertices, normales, coordenadas de textura e indices de los triángulos
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        textCoords = new List<Vector2>();
        triangles = new List<int>();

        float paso = 1.0f / particiones;

        int currIndex = 0;
        int tamTira = particiones + 1;

        for (int i=0; i<=particiones; i++) {
            float u = paso * i;
            for (int j=0; j<=particiones; j++) {
                float v = paso * j;

                Vector3 aux = new Vector3(
                    Mathf.Cos(2*Mathf.PI*u) * Mathf.Sin(Mathf.PI * v),
                    Mathf.Cos(Mathf.PI*v),
                    Mathf.Sin(2*Mathf.PI*u) * Mathf.Sin(Mathf.PI * v)
                );

                vertices.Add(radio * aux);
                normals.Add(aux.normalized);    // Para una esfera centrada en 0, la normal coincide con las coordenadas del vertice
                textCoords.Add(new Vector2(u,1.0f-v));

                if (i>0 && j>0) {
                    triangles.Add(currIndex);
                    triangles.Add(currIndex - tamTira);
                    triangles.Add(currIndex - tamTira - 1);

                    triangles.Add(currIndex);
                    triangles.Add(currIndex - tamTira - 1);
                    triangles.Add(currIndex - 1);
                }

                // Incrementar el contador de vertices
                currIndex ++;
            }
        }

        // Crear una nueva malla de triángulos a partir de los datos anteriores
        crearMesh();
    }

    // Función para crear la mesh a partir de los vectores calculados en las funciones anteriores
    public void crearMesh() {

        // Crear la mesh con toda la información
        Mesh m = new Mesh();
        m.vertices = vertices.ToArray();
        m.normals = normals.ToArray();
        m.uv = textCoords.ToArray();
        m.triangles = triangles.ToArray();

        // Llamada obligatoria para recalcular la información 
        // de la malla a partir de los vectores asignados
        m.RecalculateBounds();  

        // Asignar la malla al componente MeshFilter del Gameobject
        GetComponent<MeshFilter>().sharedMesh = m;
    }

    //crea caja de dimensiones diferentes
    public void crearCaja()
    {
        // Crear las listas para añadir vertices, normales, coordenadas de textura e indices de los triángulos
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        textCoords = new List<Vector2>();
        triangles = new List<int>();

        //DIBUJAR CARA DERECHA
        vertices.Add(new Vector3(TamX, 0, -TamZ));
        normals.Add(new Vector3(1, 0, 0));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(TamX, TamY, -TamZ));
        normals.Add(new Vector3(1, 0, 0));
        textCoords.Add(new Vector2(0, TamY));

        vertices.Add(new Vector3(TamX, TamY, 0));
        normals.Add(new Vector3(1, 0, 0));
        textCoords.Add(new Vector2(TamZ, TamY));

        vertices.Add(new Vector3(TamX, 0, 0));
        normals.Add(new Vector3(1, 0, 0));
        textCoords.Add(new Vector2(TamZ, 0));

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(3);

        //DIBUJAR CARA IZQUIERDA
        vertices.Add(new Vector3(0, 0, 0));
        normals.Add(new Vector3(-1, 0, 0));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(0, TamY, 0));
        normals.Add(new Vector3(-1, 0, 0));
        textCoords.Add(new Vector2(0, TamY));

        vertices.Add(new Vector3(0, TamY, -TamZ));
        normals.Add(new Vector3(-1, 0, 0));
        textCoords.Add(new Vector2(TamZ, TamY));

        vertices.Add(new Vector3(0, 0, -TamZ));
        normals.Add(new Vector3(-1, 0, 0));
        textCoords.Add(new Vector2(TamZ, 0));

        triangles.Add(4);
        triangles.Add(5);
        triangles.Add(6);
        triangles.Add(4);
        triangles.Add(6);
        triangles.Add(7);

        //DIBUJAR CARA FRONTAL
        vertices.Add(new Vector3(0, 0, -TamZ));
        normals.Add(new Vector3(0, 0, -1));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(0, TamY, -TamZ));
        normals.Add(new Vector3(0, 0, -1));
        textCoords.Add(new Vector2(0, TamY));

        vertices.Add(new Vector3(TamX, TamY, -TamZ));
        normals.Add(new Vector3(0, 0, -1));
        textCoords.Add(new Vector2(TamX, TamY));

        vertices.Add(new Vector3(TamX, 0, -TamZ));
        normals.Add(new Vector3(0, 0, -1));
        textCoords.Add(new Vector2(TamX, 0));

        triangles.Add(8);
        triangles.Add(9);
        triangles.Add(10);
        triangles.Add(8);
        triangles.Add(10);
        triangles.Add(11);

        //DIBUJAR CARA TRASERA
        vertices.Add(new Vector3(TamX, 0, 0));
        normals.Add(new Vector3(0, 0, 1));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(TamX, TamY, 0));
        normals.Add(new Vector3(0, 0, 1));
        textCoords.Add(new Vector2(0, TamY));

        vertices.Add(new Vector3(0, TamY, 0));
        normals.Add(new Vector3(0, 0, 1));
        textCoords.Add(new Vector2(TamX, TamY));

        vertices.Add(new Vector3(0, 0, 0));
        normals.Add(new Vector3(0, 0, 1));
        textCoords.Add(new Vector2(TamX, 0));

        triangles.Add(12);
        triangles.Add(13);
        triangles.Add(14);
        triangles.Add(12);
        triangles.Add(14);
        triangles.Add(15);

        //DIBUJAR CARA SUPERIOR
        vertices.Add(new Vector3(0, TamY, -TamZ));
        normals.Add(new Vector3(0, 1, 0));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(0, TamY, 0));
        normals.Add(new Vector3(0, 1, 0));
        textCoords.Add(new Vector2(0, TamZ));

        vertices.Add(new Vector3(TamX, TamY, 0));
        normals.Add(new Vector3(0, 1, 0));
        textCoords.Add(new Vector2(TamX, TamZ));

        vertices.Add(new Vector3(TamX, TamY, -TamZ));
        normals.Add(new Vector3(0, 1, 0));
        textCoords.Add(new Vector2(TamX, 0));

        triangles.Add(16);
        triangles.Add(17);
        triangles.Add(18);
        triangles.Add(16);
        triangles.Add(18);
        triangles.Add(19);

        //DIBUJAR CARA INFERIOR
        vertices.Add(new Vector3(0, 0, 0));
        normals.Add(new Vector3(0, -1, 0));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(0, 0, -TamZ));
        normals.Add(new Vector3(0, -1, 0));
        textCoords.Add(new Vector2(0, TamZ));

        vertices.Add(new Vector3(TamX, 0, -TamZ));
        normals.Add(new Vector3(0, -1, 0));
        textCoords.Add(new Vector2(TamX, TamZ));

        vertices.Add(new Vector3(TamX, 0, 0));
        normals.Add(new Vector3(0, -1, 0));
        textCoords.Add(new Vector2(TamX, 0));

        triangles.Add(20);
        triangles.Add(21);
        triangles.Add(22);
        triangles.Add(20);
        triangles.Add(22);
        triangles.Add(23);

        crearMesh();
    } 

    /*Crear un cilindro dados el radio, la altura y el numero de particiones*/
    public void crearCilindro(){
        
        // Crear las listas para añadir vertices, normales, coordenadas de textura e indices de los triángulos   
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        textCoords = new List<Vector2>();
        triangles = new List<int>();

        //Tamaño del lado exterior de un triangulo
        float paso = 1.0f/ particiones;
        float angulo = 0f;
        
        int currIndex = 0;
        int tamTira = particiones + 1;

        //Crear la tapa superior del cilindro
        for (int i = 0; i < particiones; i++) {
            
            // Vertice central de la tapa superior
            vertices.Add(new Vector3(0, altura, 0));
            normals.Add(Vector3.up);
            textCoords.Add(new Vector2(0.5f, 0.5f));

            // Vertice 1 de los bordes de la tapa superior
            Vector3 vertice = new Vector3(
                radio * Mathf.Cos(angulo),
                altura,
                radio * Mathf.Sin(angulo)
            );

            // Añadir a las listas
            vertices.Add(vertice);
            normals.Add(Vector3.up);
            textCoords.Add(new Vector2(0.5f+(Mathf.Cos(angulo)*0.5f), 0.5f+(Mathf.Sin(angulo)*0.5f)));

            // Incrementamos el angulo
            angulo += 2*Mathf.PI * paso;

            // Vertice 2 de los bordes de la tapa superior
            Vector3 vertice2 = new Vector3(
                radio * Mathf.Cos(angulo),
                altura,
                radio * Mathf.Sin(angulo)
            );

            // Añadir a las listas
            vertices.Add(vertice2);
            normals.Add(Vector3.up);
            textCoords.Add(new Vector2(0.5f+(Mathf.Cos(angulo)*0.5f), 0.5f+(Mathf.Sin(angulo)*0.5f)));
            
            // Añadir el triangulo a las lista
            triangles.Add(currIndex);
            triangles.Add(currIndex+2);
            triangles.Add(currIndex+1);

            // Cada triangulo 3 vertices, por eso incrementamos de 3 en 3
            currIndex = currIndex+3;
        }

        // Crear la tapa inferior del cilindro
        for (int i = 0; i < particiones; i++) {
            
            // Vertice central de la tapa inferior
            vertices.Add(new Vector3(0, 0, 0));
            normals.Add(-Vector3.up);
            textCoords.Add(new Vector2(0.5f, 0.5f));

            // Vertice 1 de los bordes de la tapa inferior
            Vector3 vertice = new Vector3(
                radio * Mathf.Cos(angulo),
                0,
                radio * Mathf.Sin(angulo)
            );

            // Añadir a las listas
            vertices.Add(vertice);
            normals.Add(-Vector3.up);
            textCoords.Add(new Vector2(0.5f+(Mathf.Cos(angulo)*0.5f), 0.5f+(Mathf.Sin(angulo)*0.5f)));

            // Incrementar el angulo
            angulo += 2*Mathf.PI * paso;

            // Vertice 2 de los bordes de la tapa inferior
            Vector3 vertice2 = new Vector3(
                radio * Mathf.Cos(angulo),
                0,
                radio * Mathf.Sin(angulo)
            );

            // Añadir a las listas
            vertices.Add(vertice2);
            normals.Add(-Vector3.up);
            textCoords.Add(new Vector2(0.5f+(Mathf.Cos(angulo)*0.5f), 0.5f+(Mathf.Sin(angulo)*0.5f)));
            
            // Añadir el triangulo a las lista
            triangles.Add(currIndex);
            triangles.Add(currIndex+1);
            triangles.Add(currIndex+2);
            
            // Cada triangulo 3 vertices, por eso incrementamos de 3 en 3
            currIndex = currIndex+3;
        }

        // Crear el cuerpo del cilindro
        for (int i=0; i<=particiones; i++) {
            float u = paso * i; // u [0,1]
            for (int j=0; j<=particiones; j++) {
                float v = paso * j; // v [0,1]

                Vector3 aux = new Vector3(
                    radio*Mathf.Cos((2*Mathf.PI)*u),
                    altura*v,
                    radio* Mathf.Sin((2*Mathf.PI)*u)
                );

                // Añadir a las listas
                vertices.Add(aux);
                normals.Add(aux.normalized);    // Para una esfera centrada en 0, la normal coincide con las coordenadas del vertice
                textCoords.Add(new Vector2(u,v));

                if (i>0 && j>0) {
                    triangles.Add(currIndex);
                    triangles.Add(currIndex - tamTira - 1);
                    triangles.Add(currIndex - tamTira);
                    

                    triangles.Add(currIndex);
                    triangles.Add(currIndex - 1);
                    triangles.Add(currIndex - tamTira - 1);
                }

                // Incrementar el contador de vertices
                currIndex ++;
            }
        }

        // Crear una nueva malla de triángulos a partir de los datos anteriores
        crearMesh();
    }

    //crea muro de almenas
    void crearMuro()
    {
        // Crear las listas para añadir vertices, normales, coordenadas de textura e indices de los triángulos
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        textCoords = new List<Vector2>();
        triangles = new List<int>();

        //contador que guarda los triangulos de forma consecutiva para evitar que 
        //los reemplace en la siguiente iteración del bucle
        int cont = 0;

        //tamaño horizontal de las almenas con respecto a las particiones y el tamaño del muro
        float x_almena = TamX/particiones;

        Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 tam = new Vector3(0.0f, 0.0f, 0.0f);

        //bucle para dibujar las partes del muro sin almenas
        for(float i = x_almena; i <=TamX; i+=x_almena*2)
        {
            offset.x = i;
            tam.x = x_almena; tam.y= TamY; tam.z = TamZ;
            crearCajaMuro(offset, tam, cont);
            cont+=24;
        }

        //bucle para dibujar las partes del muro con almenas
        for(float j = 0; j <=TamX; j+=x_almena*2)
        {
            
            offset.x = j;
            tam.x = x_almena; tam.y= TamY+altura_almena; tam.z = TamZ;
            crearCajaMuro(offset, tam, cont); 
            cont+=24;
        }

        //bucle para añadir ruido perlin a los vértices
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i] + ruidoPerlinVertice(vertices[i]);
        }
        
        crearMesh();
    }

    //Crea una particion del muro de almenas
    void crearCajaMuro(Vector3 offset, Vector3 tam, int cont)
    {
        //añadimos el offset para desplazar las cajas

        //DIBUJAR CARA DERECHA 
        vertices.Add(new Vector3(tam.x+offset.x, 0+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(1, 0, 0));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(tam.x+offset.x, tam.y+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(1, 0, 0));
        textCoords.Add(new Vector2(0, tam.y));

        vertices.Add(new Vector3(tam.x+offset.x, tam.y+offset.y, 0+offset.z));
        normals.Add(new Vector3(1, 0, 0));
        textCoords.Add(new Vector2(tam.z, tam.y));

        vertices.Add(new Vector3(tam.x+offset.x, 0+offset.y, 0+offset.z));
        normals.Add(new Vector3(1, 0, 0));
        textCoords.Add(new Vector2(tam.z, 0));

        triangles.Add(0+cont);
        triangles.Add(1+cont);
        triangles.Add(2+cont);
        triangles.Add(0+cont);
        triangles.Add(2+cont);
        triangles.Add(3+cont);

        //DIBUJAR CARA IZQUIERDA
        vertices.Add(new Vector3(0+offset.x, 0+offset.y, 0+offset.z));
        normals.Add(new Vector3(-1, 0, 0));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(0+offset.x, tam.y+offset.y, 0+offset.z));
        normals.Add(new Vector3(-1, 0, 0));
        textCoords.Add(new Vector2(0, tam.y));

        vertices.Add(new Vector3(0+offset.x, tam.y+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(-1, 0, 0));
        textCoords.Add(new Vector2(tam.z, tam.y));

        vertices.Add(new Vector3(0+offset.x, 0+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(-1, 0, 0));
        textCoords.Add(new Vector2(tam.z, 0));

        triangles.Add(4+cont);
        triangles.Add(5+cont);
        triangles.Add(6+cont);
        triangles.Add(4+cont);
        triangles.Add(6+cont);
        triangles.Add(7+cont);

        //DIBUJAR CARA FRONTAL
        vertices.Add(new Vector3(0+offset.x, 0+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(0, 0, -1));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(0+offset.x, tam.y+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(0, 0, -1));
        textCoords.Add(new Vector2(0, tam.y));

        vertices.Add(new Vector3(tam.x+offset.x, tam.y+offset.y, -tam.z+offset.y));
        normals.Add(new Vector3(0, 0, -1));
        textCoords.Add(new Vector2(tam.x, tam.y));

        vertices.Add(new Vector3(tam.x+offset.x, 0+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(0, 0, -1));
        textCoords.Add(new Vector2(tam.x, 0));

        triangles.Add(8+cont);
        triangles.Add(9+cont);
        triangles.Add(10+cont);
        triangles.Add(8+cont);
        triangles.Add(10+cont);
        triangles.Add(11+cont);

        //DIBUJAR CARA TRASERA
        vertices.Add(new Vector3(tam.x+offset.x, 0+offset.y, 0+offset.z));
        normals.Add(new Vector3(0, 0, 1));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(tam.x+offset.x, tam.y+offset.y, 0+offset.z));
        normals.Add(new Vector3(0, 0, 1));
        textCoords.Add(new Vector2(0, tam.y));

        vertices.Add(new Vector3(0+offset.x, tam.y+offset.y, 0+offset.z));
        normals.Add(new Vector3(0, 0, 1));
        textCoords.Add(new Vector2(tam.x, tam.y));

        vertices.Add(new Vector3(0+offset.x, 0+offset.y, 0+offset.z));
        normals.Add(new Vector3(0, 0, 1));
        textCoords.Add(new Vector2(tam.x, 0));

        triangles.Add(12+cont);
        triangles.Add(13+cont);
        triangles.Add(14+cont);
        triangles.Add(12+cont);
        triangles.Add(14+cont);
        triangles.Add(15+cont);

        //DIBUJAR CARA SUPERIOR
        vertices.Add(new Vector3(0+offset.x, tam.y+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(0, 1, 0));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(0+offset.x, tam.y+offset.y, 0+offset.z));
        normals.Add(new Vector3(0, 1, 0));
        textCoords.Add(new Vector2(0, tam.z));

        vertices.Add(new Vector3(tam.x+offset.x, tam.y+offset.y, 0+offset.z));
        normals.Add(new Vector3(0, 1, 0));
        textCoords.Add(new Vector2(tam.x, tam.z));

        vertices.Add(new Vector3(tam.x+offset.x, tam.y+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(0, 1, 0));
        textCoords.Add(new Vector2(tam.x, 0));

        triangles.Add(16+cont);
        triangles.Add(17+cont);
        triangles.Add(18+cont);
        triangles.Add(16+cont);
        triangles.Add(18+cont);
        triangles.Add(19+cont);

        //DIBUJAR CARA INFERIOR
        vertices.Add(new Vector3(0+offset.x, 0+offset.y, 0+offset.z));
        normals.Add(new Vector3(0, -1, 0));
        textCoords.Add(new Vector2(0, 0));

        vertices.Add(new Vector3(0+offset.x, 0+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(0, -1, 0));
        textCoords.Add(new Vector2(0, tam.z));

        vertices.Add(new Vector3(tam.x+offset.x, 0+offset.y, -tam.z+offset.z));
        normals.Add(new Vector3(0, -1, 0));
        textCoords.Add(new Vector2(tam.x, tam.z));

        vertices.Add(new Vector3(tam.x+offset.x, 0+offset.y, 0+offset.z));
        normals.Add(new Vector3(0, -1, 0));
        textCoords.Add(new Vector2(tam.x, 0));

        triangles.Add(20+cont);
        triangles.Add(21+cont);
        triangles.Add(22+cont);
        triangles.Add(20+cont);
        triangles.Add(22+cont);
        triangles.Add(23+cont);

        
    }

    Vector3 ruidoPerlinVertice(Vector3 v)
    {   
        float ruidoPerlin = Mathf.PerlinNoise(v.x * escalaPerlin, v.z * escalaPerlin) * amplitudPerlin;
        return new Vector3(0, ruidoPerlin, 0);
    }
}
