Shader "Custom/Rio"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Amplitud("Amplitud de las olas", Range(0,1)) = 1.0
        _WaterSpeed("Velocidad del agua", Range(0,10)) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Amplitud;
        half _WaterSpeed;

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 new_uv = IN.uv_MainTex;
            
            new_uv.x += (_WaterSpeed)*_Time.x;
            new_uv.y +=  _Amplitud * sin(new_uv.x*20 + (_Time.y * _WaterSpeed)) + 0.5;

            fixed4 c = tex2D (_MainTex, new_uv) * _Color;

            // Albedo comes from a texture tinted by color
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }

        void vert(inout appdata_full v)
        {
            // Transformar la posición del vértice al espacio del mundo
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            // Generar una onda en el eje y usando las coordenadas de textura
            float offsetY = _Amplitud * sin(v.texcoord.y  + _Time.y * _WaterSpeed);
            float offsetX = _Amplitud/2 * sin(offsetY  + _Time.y * _WaterSpeed*2);
            
            // Deformar la posición del vértice en el eje y
            worldPos.y += offsetY/2;
            worldPos.x += offsetX;

            // Transformar la posición deformada al espacio del objeto
            v.vertex = mul(unity_WorldToObject, worldPos);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
