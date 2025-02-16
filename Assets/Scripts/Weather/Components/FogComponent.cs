using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FogComponent : IWeatherComponent
{
    private VolumeProfile profile;
    private Fog fog;
    private bool isInitialized;

    public void Initialize(VolumeProfile profile)
    {
        this.profile = profile;  // Store the profile reference
        isInitialized = profile.TryGet(out fog);
        if (!isInitialized)
        {
            Debug.LogError("Failed to initialize FogComponent: Fog not found in VolumeProfile");
        }
    }

    public void Interpolate(IWeatherComponentData from, IWeatherComponentData to, float t)
    {
        if (!isInitialized) return;
        if (from is FogComponentData fromData && to is FogComponentData toData)
        {
            var interpolated = (FogComponentData)fromData.Lerp(toData, t);
            ApplyImmediate(interpolated);
        }
    }

    public void ApplyImmediate(IWeatherComponentData data)
    {
        if (!isInitialized || !(data is FogComponentData fogData)) return;

        // Basic fog settings
        fog.active = fogData.enabled;

        // Fog attenuation
        fog.meanFreePath.value = fogData.meanFreePath;
        fog.baseHeight.value = fogData.baseHeight;
        fog.maximumHeight.value = fogData.maximumHeight;
        fog.maxFogDistance.value = fogData.maxDistanceInMeters;

        // Fog color and mip settings
        fog.albedo.value = fogData.albedo;
        fog.mipFogNear.value = fogData.mipFogNear;
        fog.mipFogFar.value = fogData.mipFogFar;
        fog.mipFogMaxMip.value = fogData.mipFogMaxMip;

        // Volumetric fog settings
        fog.globalLightProbeDimmer.value = fogData.globalLightProbeDimmer;
        fog.depthExtent.value = fogData.depthExtent;
        fog.sliceDistributionUniformity.value = fogData.sliceDistributionUniformity;
        // These might need to be accessed differently depending on your HDRP version
        // fog.volumetricFogBudget.value = fogData.volumetricFogBudget;
        // fog.resolutionDepthRatio.value = fogData.resolutionDepthRatio;
        fog.directionalLightsOnly.value = fogData.directionalLightsOnly;
        fog.anisotropy.value = fogData.anisotropy;
        fog.globalLightProbeDimmer.value = fogData.multiScattering;
    }

    public IWeatherComponentData CreateSnapshot()
    {
        if (!isInitialized) return new FogComponentData();

        return new FogComponentData
        {
            enabled = fog.active,
            meanFreePath = fog.meanFreePath.value,
            baseHeight = fog.baseHeight.value,
            maximumHeight = fog.maximumHeight.value,
            maxDistanceInMeters = fog.maxFogDistance.value,
            albedo = fog.albedo.value,
            mipFogNear = fog.mipFogNear.value,
            mipFogFar = fog.mipFogFar.value,
            mipFogMaxMip = fog.mipFogMaxMip.value,
            globalLightProbeDimmer = fog.globalLightProbeDimmer.value,
            depthExtent = fog.depthExtent.value,
            sliceDistributionUniformity = fog.sliceDistributionUniformity.value,
            // volumetricFogBudget = fog.volumetricFogBudget.value,
            // resolutionDepthRatio = fog.resolutionDepthRatio.value,
            directionalLightsOnly = fog.directionalLightsOnly.value,
            anisotropy = fog.anisotropy.value,
            multiScattering = fog.globalLightProbeDimmer.value
        };
    }

    public void UpdateTimeOfDay(IWeatherComponentData data, float dayNightFactor)
    {
        if (!isInitialized || !(data is FogComponentData fogData)) return;

        // Adjust fog density based on time of day
        fog.meanFreePath.value = Mathf.Lerp(
            fogData.meanFreePath * 0.5f,  // Reduced visibility at night
            fogData.meanFreePath,
            dayNightFactor
        );
        
        fog.maximumHeight.value = Mathf.Lerp(
            fogData.maximumHeight * 0.75f,
            fogData.maximumHeight,
            dayNightFactor
        );
    }
}