Shader "ImageEffect/DreamCatchEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor("Shadow Col", Color) = (0,0,0,0)
        _ShadowThreshold("Shadow Threshold", Float) = 0.5
    }
    SubShader
    {
        // No culling or depth
        // Z값을 사용하지 않고 항상 위에 나오게 하는 방법
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _ShadowThreshold;
            float4 _ShadowColor;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                half4 tex = col;
                // 명도를 유지한 흑백의 이미지를 얻음, 흑백이미지는 0~1 까지의 값을 갖으므로 0.5와 비교해서 0.5보다 크면 밝음. 0.5보다 작으면 어두움
                float luminance = (col.r * 0.29 + col.g * 0.59 + col.b * 0.12);

                if (luminance > _ShadowThreshold)
                {
                    col.rgb = 1;
                }
                else
                {
                    col.rgb = 0;
                }
                // just invert the colors = 색반전 코드
                // col.rgb = 1 - col.rgb;
                //return col;

                // 어두운 부분은 지금 어두운 부분(col) 그대로 쓰고, 흰색에는 원래 컬러 이미지(tex)를 가져온다
                col.rgb = lerp(_ShadowColor.rgb, tex.rgb, col.r);
                return col;
            }

            ENDCG
        }
    }
}
