Shader "Custom/Columnas"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Tessellation("Tessellation", Range(1,5)) = 1.0
        _EfectoOnda("EfectoOnda", Range(0,0.3)) = 0
        _Rotacion("Angulo", Range(0, 4)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow tessellate:tess

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;

        half _EfectoOnda;
        half _Tessellation;
        half _Rotacion;

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

    
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;


            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }

        //Función para manipular la rotación de la columna
        float3 rotate(half3 v, half angle){
            float3x3 mat;
            mat[0] = float3(cos(angle),0,sin(angle));
            mat[1] = float3(0,1,0);
            mat[2] = float3(-sin(angle),0,cos(angle));

            return mul(mat,v);
        }

        // Función para producir la deformación de la columna vertice a vertice
        void vert(inout appdata_full v) {
            
            // Declaración de las variables amplitud, frecuencia y angulo
            float amplitud = _EfectoOnda*0.2;
            float freq = 10.0;
            float transformacion = v.vertex.y*freq;

            float angulo = 0.0;

            v.vertex.x += amplitud * sin(transformacion);

            angulo = _Rotacion * v.vertex.y;
            v.vertex.xyz = rotate(v.vertex.xyz, angulo);
            v.normal.xyz = rotate(v.normal.xyz, angulo);
        }

        float tess(){
            return _Tessellation;
        }

        ENDCG
    }
    FallBack "Diffuse"
}