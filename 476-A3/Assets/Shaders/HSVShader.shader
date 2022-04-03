// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/HSVShader"
{    
    Properties
    {
        _Hue("Hue", float) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _Hue;

            struct appdata
            {
                float4 vertex:POSITION;
                float2 uv:TEXCOORD0;
            };

            struct v2f
            {
                float4 clipPos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };

            float3 hue2rgb(float hue) {
                hue = frac(hue);
                float r = abs(hue * 6 - 3) - 1;
                float g = 2 - abs(hue * 6 - 2);
                float b = 2 - abs(hue * 6 - 4);
                float3 rgb = float3(r,g,b);
                rgb = saturate(rgb);
                return rgb;
            }

            float3 hsv2rgb(float3 hsv) {
                float3 rgb = hue2rgb(hsv.x);
                rgb = lerp(1, rgb, hsv.y);
                rgb *= hsv.z;
                return rgb;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.clipPos=UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 hsv = float3(_Hue, i.uv.x, i.uv.y);
                float3 rgb = hsv2rgb(hsv);
                return float4(rgb, 1);
            }
            ENDCG
        }
    }
}