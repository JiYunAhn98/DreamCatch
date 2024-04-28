Shader "ImageEffect/Chromatic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RGBSplit("RGB Split", float) = 0
        _ChromaticPower("Power", float) = 3
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _RGBSplit;
            float _ChromaticPower;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col;// = tex2D(_MainTex, i.uv);

                float2 expandTexCoord = i.uv * 2 - 1;
                float radialGradient = dot(expandTexCoord, expandTexCoord*3);
                radialGradient = pow(radialGradient, _ChromaticPower);

                half colR = tex2D(_MainTex, i.uv + float2(_RGBSplit, _RGBSplit) * 0.1 * radialGradient).r;
                half colG = tex2D(_MainTex, i.uv).g;
                half colB = tex2D(_MainTex, i.uv + float2(_RGBSplit, _RGBSplit) * 0.1 * radialGradient).b;
                
                col.rgb = float3(colR, colG, colB);
                return col;
            }
            ENDCG
        }
    }
}
