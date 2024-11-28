Shader "Custom/HolySymbolGlow"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,0.5,0,1)
        _GlowSize ("Glow Size", Range(1,10)) = 2
        _Intensity ("Glow Intensity", Range(1,10)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend One One

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowSize;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture multiple times for blur effect
                float4 tex = tex2D(_MainTex, i.uv);
                
                // Simple blur by sampling nearby pixels
                float2 directions[8] = {
                    float2(1,0), float2(-1,0),
                    float2(0,1), float2(0,-1),
                    float2(0.707,0.707), float2(-0.707,0.707),
                    float2(0.707,-0.707), float2(-0.707,-0.707)
                };
                
                float glow = 0;
                for(int dir = 0; dir < 8; dir++)  // Changed 'i' to 'dir'
                {
                    float2 offset = directions[dir] * _GlowSize * 0.01;
                    glow += tex2D(_MainTex, i.uv + offset).a;
                }
                glow /= 8.0;
                
                // Combine original texture with glow
                float4 finalColor = tex + glow * _GlowColor * _Intensity;
                finalColor.a = saturate(finalColor.a);
                
                return finalColor;
            }
            ENDCG
        }
    }
}