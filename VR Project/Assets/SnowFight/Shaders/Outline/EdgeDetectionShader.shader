Shader "Hidden/Edge Detection"
{
    Properties
    {
        _OutlineThickness ("Outline Thickness", Float) = 1
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _UseDynamicOutline ("Use Dynamic Outline", Range(0, 1)) = 0  
        _DynamicDarkeningFactor ("Dynamic Darkening Factor", Range(0, 1)) = 0.5
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.2
        _NoiseScale ("Noise Scale", Float) = 20.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            Name "EDGE DETECTION OUTLINE"
            
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            float _OutlineThickness;
            float4 _OutlineColor;
            float _UseDynamicOutline;
            float _DynamicDarkeningFactor;
            float _NoiseIntensity;
            float _NoiseScale;

            #pragma vertex Vert
            #pragma fragment frag

            // Kernel di edge detection (Roberts Cross) per vettori.
            float RobertsCross(float3 samples[4])
            {
                float3 diff1 = samples[1] - samples[2];
                float3 diff2 = samples[0] - samples[3];
                return sqrt(dot(diff1, diff1) + dot(diff2, diff2));
            }

            // Kernel di edge detection per valori singoli.
            float RobertsCross(float samples[4])
            {
                float diff1 = samples[1] - samples[2];
                float diff2 = samples[0] - samples[3];
                return sqrt(diff1 * diff1 + diff2 * diff2);
            }
            
            // Helper per mappare le normali dallo spazio [-1,1] a [0,1].
            float3 SampleSceneNormalsRemapped(float2 uv)
            {
                return SampleSceneNormals(uv) * 0.5 + 0.5;
            }
            
            // Helper per il calcolo della luminanza.
            float SampleSceneLuminance(float2 uv)
            {
                float3 color = SampleSceneColor(uv);
                return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
            }

            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 uv = IN.texcoord;
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                
                // Calcola il raggio per sampling in base allo spessore definito
                float halfF = floor(_OutlineThickness * 0.5);
                float halfC = ceil(_OutlineThickness * 0.5);

                float2 uvs[4];
                uvs[0] = uv + texelSize * float2(halfF, halfC) * float2(-1, 1);
                uvs[1] = uv + texelSize * float2(halfC, halfC) * float2(1, 1);
                uvs[2] = uv + texelSize * float2(halfF, halfF) * float2(-1, -1);
                uvs[3] = uv + texelSize * float2(halfC, halfF) * float2(1, -1);
                
                float3 normalSamples[4];
                float depthSamples[4], luminanceSamples[4];
                for (int i = 0; i < 4; i++)
                {
                    depthSamples[i] = SampleSceneDepth(uvs[i]);
                    normalSamples[i] = SampleSceneNormalsRemapped(uvs[i]);
                    luminanceSamples[i] = SampleSceneLuminance(uvs[i]);
                }
                
                float edgeDepth = RobertsCross(depthSamples);
                float edgeNormal = RobertsCross(normalSamples);
                float edgeLuminance = RobertsCross(luminanceSamples);
                
                float depthThreshold = 1.0 / 200.0;
                edgeDepth = edgeDepth > depthThreshold ? 1.0 : 0.0;
                
                float normalThreshold = 1.0 / 4.0;
                edgeNormal = edgeNormal > normalThreshold ? 1.0 : 0.0;
                
                float luminanceThreshold = 1.0 / 0.5;
                edgeLuminance = edgeLuminance > luminanceThreshold ? 1.0 : 0.0;
                
                float edge = max(edgeDepth, max(edgeNormal, edgeLuminance));
                float noise = frac(sin(dot(uv * _NoiseScale, float2(12.9898, 78.233))) * 43758.5453);
                edge *= step(_NoiseIntensity, noise);

                // Scelta dinamica del colore: se _UseDynamicOutline > 0.5, campiona e media i colori nei dintorni,
                // altrimenti usa il colore fisso.
                float3 finalOutlineColor;
                if (_UseDynamicOutline > 0.5)
                {
                    // Media di 5 sample: centrale + 4 nei compagni orizzontale e verticale.
                    float3 sum = SampleSceneColor(uv);
                    sum += SampleSceneColor(uv + texelSize * float2(1, 0));
                    sum += SampleSceneColor(uv + texelSize * float2(-1, 0));
                    sum += SampleSceneColor(uv + texelSize * float2(0, 1));
                    sum += SampleSceneColor(uv + texelSize * float2(0, -1));
                    float3 avgColor = sum / 5.0;
                    finalOutlineColor = avgColor * _DynamicDarkeningFactor;
                }
                else
                {
                    finalOutlineColor = _OutlineColor.rgb;
                }
                
                return edge * float4(finalOutlineColor, 1.0);
            }
            ENDHLSL
        }
    }
}
