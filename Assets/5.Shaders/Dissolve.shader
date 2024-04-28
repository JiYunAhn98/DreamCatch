Shader "Custom/Dissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex("Noise Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Cut("Alpha Cut", Range(0,1)) = 0
        _OutThikness("OutThikness", Range(1,1.15)) = 1.15
        [HDR]_OutColor("Out Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadow alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NoiseTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Cut;
        float _OutThikness;
        float4 _OutColor;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            fixed4 noiseTex = tex2D(_NoiseTex, IN.uv_NoiseTex);

            float alpha;
            if (noiseTex.r >= _Cut) alpha = c.a;
            else alpha = 0;

            float outline;
            if (noiseTex.r >= _Cut * _OutThikness) outline = 0;
            else outline = 1;

            o.Emission = outline * _OutColor.rgb;
            o.Alpha = alpha;

            //o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
