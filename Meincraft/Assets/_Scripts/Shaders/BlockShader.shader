Shader "Meincraft/Block Shader"
{
    Properties
    {
        _MainTex ("Texture", 2DArray) = "white" {}
        _BlockBreakingAtlas ("Texture", 2DArray) = "white" {}
        _AlphaClipping("AlphaClipping", Float) = 0.5
        _BreakProgress ("Break Progress", Range(0, 1)) = 0
        _TargetBlockPosition ("Target Block Position", Vector) = (0, 0, 0, 0)
        _OutlineThickness ("Outline Thickness", Range(0.001, 10)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float3 worldPos : TEXCOORD2;
            };

            UNITY_DECLARE_TEX2DARRAY(_MainTex);
            UNITY_DECLARE_TEX2DARRAY(_BlockBreakingAtlas);
            
            float _AlphaClipping;
            
            float4 _TargetBlockPosition;
            float _BreakProgress;
            float _OutlineThickness;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texCol = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.uv.xyz);
                fixed4 color = texCol * i.color;
                float3 posDiff = i.worldPos - _TargetBlockPosition;
                
                float3 isInsideBlock = step(0, posDiff) * step(posDiff, 1);
                float insideBlock = isInsideBlock.x * isInsideBlock.y * isInsideBlock.z;

                if (insideBlock > 0)
                {
                    if(_BreakProgress > 0)
                    {
                        int breakStage = (_BreakProgress * 10);
                        fixed4 blockBreakingCol = UNITY_SAMPLE_TEX2DARRAY(_BlockBreakingAtlas, float3(i.uv.xy, breakStage));
                        color.rgb *= blockBreakingCol.a;
                    }

                    float3 blockMin = _TargetBlockPosition;
                    float3 blockMax = _TargetBlockPosition + float3(1, 1, 1);

                    float edgeX = step(blockMin.x + _OutlineThickness, i.worldPos.x) - step(blockMax.x - _OutlineThickness, i.worldPos.x);
                    float edgeY = step(blockMin.y + _OutlineThickness, i.worldPos.y) - step(blockMax.y - _OutlineThickness, i.worldPos.y);
                    float edgeZ = step(blockMin.z + _OutlineThickness, i.worldPos.z) - step(blockMax.z - _OutlineThickness, i.worldPos.z);

                    float edgeFactorX = max(edgeX, min(edgeY, edgeZ));
                    float edgeFactorY = max(edgeY, min(edgeX, edgeZ));
                    float edgeFactorZ = max(edgeZ, min(edgeX, edgeY));

                    color = lerp(color, fixed4(0, 0, 0, 1), 1-edgeFactorX);
                    color = lerp(color, fixed4(0, 0, 0, 1), 1-edgeFactorY);
                    color = lerp(color, fixed4(0, 0, 0, 1), 1-edgeFactorZ);
                }
                
                clip(color.a - _AlphaClipping);
                
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }
            ENDCG
        }
    }
}
