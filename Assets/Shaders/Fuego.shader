Shader "Custom/Fuego"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Noise("Noise", 2D) = "white"{}
        _FireSpeed("Fire speed", float) = 1.0
    }
    SubShader
    {
        // "RenderType"="Transparent" "Queue"="Transparent" 
        Tags { "RenderType"="Transparent" "Queue"="Transparent"  }
        LOD 200
        Cull Off
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Noise;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Noise;
        };

        half _Glossiness;
        half _Metallic;
        half _FireSpeed;

        fixed4 _Color;
        fixed4 _Color2;

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

            //Pasos 4 y 5
            float2 uvNoise = IN.uv_Noise;
            uvNoise.y += _Time*_FireSpeed;
            fixed4 noise =tex2D (_Noise, uvNoise);
            // o.Albedo = noise.xyz;

            //Paso 6
            half2 uv;
            
            uv.x = IN.uv_MainTex.x;
            uv.y = (noise.y + IN.uv_MainTex.y) *0.5;

            // o.Albedo = float3(uv.x, uv.y, 0);
            //Paso 7
            fixed4 newC = tex2D(_MainTex, uv) * _Color;

            //Paso 8
            o.Emission = newC.rgb;
            o.Albedo = 0;
            o.Alpha = newC.a;
            

            // o.Albedo = c.rgb;
            
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            // o.Alpha = c.a;

            // fixed4 c2 = tex2D(_SecondTex, IN.uv_SecondTex) * _Color2;
           
        }
        ENDCG
    }
    FallBack "Diffuse"
}
