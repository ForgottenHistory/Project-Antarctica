using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable]
public class FogComponentData : IWeatherComponentData
{
    [Header("Basic Settings")]
    public bool enabled = true;

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

    public void CopyFrom(IWeatherComponentData other)
    {
        if (other is FogComponentData data)
        {
            enabled = data.enabled;
            meanFreePath = data.meanFreePath;
            baseHeight = data.baseHeight;
            maximumHeight = data.maximumHeight;
            maxDistanceInMeters = data.maxDistanceInMeters;
            albedo = data.albedo;
            mipFogNear = data.mipFogNear;
            mipFogFar = data.mipFogFar;
            mipFogMaxMip = data.mipFogMaxMip;
            globalLightProbeDimmer = data.globalLightProbeDimmer;
            depthExtent = data.depthExtent;
            sliceDistributionUniformity = data.sliceDistributionUniformity;
            volumetricFogBudget = data.volumetricFogBudget;
            resolutionDepthRatio = data.resolutionDepthRatio;
            directionalLightsOnly = data.directionalLightsOnly;
            anisotropy = data.anisotropy;
            multiScattering = data.multiScattering;
        }
    }

    public IWeatherComponentData Lerp(IWeatherComponentData to, float t)
    {
        if (to is FogComponentData target)
        {
            return new FogComponentData
            {
                enabled = t < 0.5f ? enabled : target.enabled,
                meanFreePath = Mathf.Lerp(meanFreePath, target.meanFreePath, t),
                baseHeight = Mathf.Lerp(baseHeight, target.baseHeight, t),
                maximumHeight = Mathf.Lerp(maximumHeight, target.maximumHeight, t),
                maxDistanceInMeters = Mathf.Lerp(maxDistanceInMeters, target.maxDistanceInMeters, t),
                albedo = Color.Lerp(albedo, target.albedo, t),
                mipFogNear = Mathf.Lerp(mipFogNear, target.mipFogNear, t),
                mipFogFar = Mathf.Lerp(mipFogFar, target.mipFogFar, t),
                mipFogMaxMip = Mathf.Lerp(mipFogMaxMip, target.mipFogMaxMip, t),
                globalLightProbeDimmer = Mathf.Lerp(globalLightProbeDimmer, target.globalLightProbeDimmer, t),
                depthExtent = Mathf.Lerp(depthExtent, target.depthExtent, t),
                sliceDistributionUniformity = Mathf.Lerp(sliceDistributionUniformity, target.sliceDistributionUniformity, t),
                volumetricFogBudget = Mathf.Lerp(volumetricFogBudget, target.volumetricFogBudget, t),
                resolutionDepthRatio = Mathf.Lerp(resolutionDepthRatio, target.resolutionDepthRatio, t),
                directionalLightsOnly = t < 0.5f ? directionalLightsOnly : target.directionalLightsOnly,
                anisotropy = Mathf.Lerp(anisotropy, target.anisotropy, t),
                multiScattering = Mathf.Lerp(multiScattering, target.multiScattering, t)
            };
        }
        return this;
    }
}
