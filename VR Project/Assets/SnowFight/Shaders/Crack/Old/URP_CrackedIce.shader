Shader "Custom/URP_CrackedIce"
{
    Properties
    {
        _MainTex ("Ice Texture", 2D) = "white" {}
        _CrackTex1 ("Crack Texture 1", 2D) = "white" {}
        _CrackTex2 ("Crack Texture 2", 2D) = "white" {}
        _Damage ("Damage", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Blend Off
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // Includi le librerie URP. Se il percorso non funziona, verifica che il tuo pacchetto URP sia aggiornato.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Dichiarazioni delle texture e dei sampler tramite macro URP:
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_CrackTex1);
            SAMPLER(sampler_CrackTex1);

            TEXTURE2D(_CrackTex2);
            SAMPLER(sampler_CrackTex2);

            float _Damage; // Valore compreso tra 0 e 1.
            float4 _MainTex_ST; // Per tiling/offset

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                // Trasforma la posizione dell'oggetto in clip space
                OUT.pos = TransformObjectToHClip(IN.vertex);
                // Applica tiling e offset usando la macro TRANSFORM_TEX definita in Core.hlsl
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                // Campiona le texture utilizzando le macro SAMPLE_TEXTURE2D
                half4 baseColor   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half4 crackColor1 = SAMPLE_TEXTURE2D(_CrackTex1, sampler_CrackTex1, uv);
                half4 crackColor2 = SAMPLE_TEXTURE2D(_CrackTex2, sampler_CrackTex2, uv);

                // Calcola due fattori di blending con smoothstep per ottenere transizioni morbide.
                float blend1 = smoothstep(0.2, 0.5, _Damage);
                float blend2 = smoothstep(0.5, 0.8, _Damage);

                // Effettua il blending: prima tra baseColor e crackColor1, poi tra il risultato e crackColor2.
                half4 blended = lerp(baseColor, crackColor1, blend1);
                blended = lerp(blended, crackColor2, blend2);

                return blended;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
