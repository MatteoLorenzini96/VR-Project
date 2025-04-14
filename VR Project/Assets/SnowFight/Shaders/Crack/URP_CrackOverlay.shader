Shader "Custom/URP_CrackOverlay_UVDistortion"
{
    Properties
    {
        _CrackTex1 ("Crack Texture 1", 2D) = "white" {}
        _CrackTex2 ("Crack Texture 2", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0,0.1)) = 0.02
        _Damage ("Damage", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Name "CrackOverlay"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CrackTex1);
            SAMPLER(sampler_CrackTex1);
            TEXTURE2D(_CrackTex2);
            SAMPLER(sampler_CrackTex2);
            float _Damage;
            float _DistortionStrength;
            float4 _CrackTex1_ST;
            float4 _CrackTex2_ST;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv1 = TRANSFORM_TEX(IN.uv, _CrackTex1);
                OUT.uv2 = TRANSFORM_TEX(IN.uv, _CrackTex2);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Campiona la prima texture delle crepe
                half4 crack1 = SAMPLE_TEXTURE2D(_CrackTex1, sampler_CrackTex1, IN.uv1);

                // Calcola la luminanza della texture delle crepe
                float luminance = dot(crack1.rgb, float3(0.299, 0.587, 0.114));

                // Calcola l'offset delle UV basato sulla luminanza
                float2 offset = (_DistortionStrength * luminance) * float2(1.0, 1.0);

                // Applica l'offset alle UV
                float2 distortedUV1 = IN.uv1 + offset;
                float2 distortedUV2 = IN.uv2 + offset;

                // Campiona le texture delle crepe con le UV distorte
                half4 distortedCrack1 = SAMPLE_TEXTURE2D(_CrackTex1, sampler_CrackTex1, distortedUV1);
                half4 distortedCrack2 = SAMPLE_TEXTURE2D(_CrackTex2, sampler_CrackTex2, distortedUV2);

                // Calcola i fattori di blending
                float blend1 = smoothstep(0.2, 0.5, _Damage);
                float blend2 = smoothstep(0.5, 0.8, _Damage);

                // Effettua il blending delle texture distorte
                half4 blended = lerp(half4(0,0,0,0), distortedCrack1, blend1);
                blended = lerp(blended, distortedCrack2, blend2);

                return blended;
            }
            ENDHLSL
        }
    }
}
