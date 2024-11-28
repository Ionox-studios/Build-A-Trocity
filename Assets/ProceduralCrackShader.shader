Shader "Custom/ProceduralCrackShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _CrackWidth ("Crack Width", Range(0.0, 0.1)) = 0.02
        _DamageLevel ("Damage Amount", Range(0, 1)) = 0  // Added damage control
        _CrackScale ("Crack Scale", Range(1, 50)) = 10
        _CrackSlope ("Crack Slope", Range(1, 100)) = 50
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
            float _DamageLevel;
            float _CrackWidth;
            float _CrackScale;
            float _CrackSlope;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 hash2(float2 p)
            {
                return frac(sin(float2(dot(p,float2(127.1,311.7)),
                                     dot(p,float2(269.5,183.3))))*43758.5453);
            }

            float3 voronoiB(float2 x)
            {
                float2 n = floor(x);
                float2 f = frac(x);

                float2 mg, mr;
                float md = 8.0;

                for(int j = -1; j <= 1; j++)
                {
                    for(int i = -1; i <= 1; i++)
                    {
                        float2 g = float2(float(i),float(j));
                        float2 o = hash2(n + g);
                        float2 r = g + o - f;
                        float d = dot(r,r);

                        if(d < md)
                        {
                            md = d;
                            mr = r;
                            mg = g;
                        }
                    }
                }

                md = 8.0;
                for(int j = -2; j <= 2; j++)
                {
                    for(int i = -2; i <= 2; i++)
                    {
                        float2 g = mg + float2(float(i),float(j));
                        float2 o = hash2(n + g);
                        float2 r = g + o - f;

                        if(dot(mr-r,mr-r) > 0.00001)
                        {
                            md = min(md, dot(0.5*(mr+r), normalize(r-mr)));
                        }
                    }
                }
                return float3(md, mr+n);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float2 uv = i.uv * _DamageLevel*100.0;
                float3 v = voronoiB(uv);
                
                float crack = min(1.0, _CrackSlope * pow(max(0.0, v.x - (_CrackWidth )), 1.0));


                //crack = 1.0 - crack;
                
                col.rgb *= crack;
                
                return col;
            }
            ENDCG
        }
    }
}