Shader "Meincraft/Water Shader"
{
    Properties
    {
        _MainTex ("Texture", 2DArray) = "white" {}
        _WaterHeight ("Water Height", Float) = 0.9
        _Alpha ("Alpha", Float) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
            };

            UNITY_DECLARE_TEX2DARRAY(_MainTex);
            
            float _WaterHeight;
            float _Alpha;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.uv.xyz);
                color *= i.color;
                UNITY_APPLY_FOG(i.fogCoord, color);
                color.a = _Alpha;
                return color;
            }
            ENDCG
        }
    }
}
