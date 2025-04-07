using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class EdgeDetection : ScriptableRendererFeature
{
    private class EdgeDetectionPass : ScriptableRenderPass
    {
        private Material material;

        private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int NoiseIntensityProperty = Shader.PropertyToID("_NoiseIntensity");
        private static readonly int NoiseScaleProperty = Shader.PropertyToID("_NoiseScale");
        private static readonly int UseDynamicOutlineProperty = Shader.PropertyToID("_UseDynamicOutline");
        private static readonly int DynamicDarkeningFactorProperty = Shader.PropertyToID("_DynamicDarkeningFactor");

        public EdgeDetectionPass()
        {
            profilingSampler = new ProfilingSampler(nameof(EdgeDetectionPass));
        }

        public void Setup(ref EdgeDetectionSettings settings, ref Material edgeDetectionMaterial)
        {
            material = edgeDetectionMaterial;
            renderPassEvent = settings.renderPassEvent;

            material.SetFloat(OutlineThicknessProperty, settings.outlineThickness);
            material.SetColor(OutlineColorProperty, settings.outlineColor);
            material.SetFloat(NoiseIntensityProperty, settings.noiseIntensity);
            material.SetFloat(NoiseScaleProperty, settings.noiseScale);
            material.SetFloat(UseDynamicOutlineProperty, settings.useDynamicOutline);
            material.SetFloat(DynamicDarkeningFactorProperty, settings.dynamicDarkeningFactor);
        }

        private class PassData
        {
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();

            using var builder = renderGraph.AddRasterRenderPass<PassData>("Edge Detection", out _);

            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.UseAllGlobalTextures(true);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc((PassData _, RasterGraphContext context) =>
            {
                Blitter.BlitTexture(context.cmd, Vector2.one, material, 0);
            });
        }
    }

    [Serializable]
    public class EdgeDetectionSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        [Range(0, 15)] public int outlineThickness = 3;
        public Color outlineColor = Color.black;

        // 0 per outline fisso, 1 per outline dinamico
        [Range(0, 1)] public float useDynamicOutline = 0;
        // Fattore di scurimento per il colore campionato (0 = nero, 1 = stesso colore)
        [Range(0, 1)] public float dynamicDarkeningFactor = 0.5f;

        [Range(0, 1)] public float noiseIntensity = 0.2f;
        public float noiseScale = 20.0f;
    }

    [SerializeField] private EdgeDetectionSettings settings;
    private Material edgeDetectionMaterial;
    private EdgeDetectionPass edgeDetectionPass;

    /// <summary>
    /// Chiamato:
    /// - Quando la Scriptable Renderer Feature viene caricata per la prima volta.
    /// - Quando viene abilitata o disabilitata.
    /// - Quando viene modificata una proprieta' nell'Inspector.
    /// </summary>
    public override void Create()
    {
        edgeDetectionPass ??= new EdgeDetectionPass();
    }

    /// <summary>
    /// Chiamato ogni frame, una volta per ogni camera.
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Non eseguire il rendering per alcune viste.
        if (renderingData.cameraData.cameraType == CameraType.Preview ||
            renderingData.cameraData.cameraType == CameraType.Reflection ||
            UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
            return;

        if (edgeDetectionMaterial == null)
        {
            edgeDetectionMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Edge Detection"));
            if (edgeDetectionMaterial == null)
            {
                Debug.LogWarning("Not all required materials could be created. Edge Detection will not render.");
                return;
            }
        }

        edgeDetectionPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        edgeDetectionPass.requiresIntermediateTexture = true;
        edgeDetectionPass.Setup(ref settings, ref edgeDetectionMaterial);

        renderer.EnqueuePass(edgeDetectionPass);
    }

    /// <summary>
    /// Liberazione delle risorse allocate, come i materiali.
    /// </summary>
    override protected void Dispose(bool disposing)
    {
        edgeDetectionPass = null;
        CoreUtils.Destroy(edgeDetectionMaterial);
    }
}
