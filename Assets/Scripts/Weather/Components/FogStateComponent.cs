using UnityEngine;

[System.Serializable]
public class FogStateComponent
{
    [Header("Basic Settings")]
    public bool enableVolumetricFog = true;
    
    [Header("Fog Attenuation")]
    public float meanFreePath = 1000f;
    public float baseHeight = 0f;
    public float maximumHeight = 100f;
    public float maxDistanceInMeters = 500f;
    
    [Header("Fog Color")]
    public Color albedo = Color.white;
    public float mipFogNear = 0f;
    public float mipFogFar = 1000f;
    public float mipFogMaxMip = 0.5f;
    
    [Header("Volumetric Fog")]
    public float globalLightProbeDimmer = 1f;
    public float depthExtent = 64f;
    public float sliceDistributionUniformity = 0.75f;
    [Range(0, 1)] public float volumetricFogBudget = 0.5f;
    [Range(0, 1)] public float resolutionDepthRatio = 0.5f;
    public bool directionalLightsOnly = false;
    public float anisotropy = 0f;
    public float multiScattering = 0f;
}
