using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FogInterpolator : WeatherInterpolator
{
    private Fog fog;

    public FogInterpolator(VolumeProfile profile) : base(profile) { }

    protected override bool Initialize(VolumeProfile profile)
    {
        return profile.TryGet(out fog);
    }

    public override void Interpolate(WeatherStateSO from, WeatherStateSO to, float t)
    {
        if (!IsValid || from == null || to == null) return;

        // Basic fog settings
        fog.active = to.enableVolumetricFog;  // Changed from enableVolumetricFog.value
        
        // Fog attenuation
        fog.meanFreePath.value = Mathf.Lerp(from.meanFreePath, to.meanFreePath, t);
        fog.baseHeight.value = Mathf.Lerp(from.baseHeight, to.baseHeight, t);
        fog.maximumHeight.value = Mathf.Lerp(from.maximumHeight, to.maximumHeight, t);
        
        // Fog color and mip settings
        fog.albedo.value = Color.Lerp(from.albedo, to.albedo, t);  // Changed from tint to albedo
        fog.mipFogNear.value = Mathf.Lerp(from.mipFogNear, to.mipFogNear, t);
        fog.mipFogFar.value = Mathf.Lerp(from.mipFogFar, to.mipFogFar, t);
        fog.mipFogMaxMip.value = Mathf.Lerp(from.mipFogMaxMip, to.mipFogMaxMip, t);
        
        // Volumetric fog settings
        /**
        fog.globalLightProbeDimmer.value = Mathf.Lerp(from.globalLightProbeDimmer, to.globalLightProbeDimmer, t);
        fog.depthExtent.value = Mathf.Lerp(from.depthExtent, to.depthExtent, t);
        fog.sliceDistributionUniformity.value = Mathf.Lerp(from.sliceDistributionUniformity, to.sliceDistributionUniformity, t);
        fog.volumetricFogBudget = Mathf.Lerp(from.volumetricFogBudget, to.volumetricFogBudget, t);
        fog.resolutionDepthRatio = Mathf.Lerp(from.resolutionDepthRatio, to.resolutionDepthRatio, t);
        fog.directionalLightsOnly.value = to.directionalLightsOnly;
        fog.anisotropy.value = Mathf.Lerp(from.anisotropy, to.anisotropy, t);
        */
    }

    public override void ApplyImmediate(WeatherStateSO state)
    {
        if (!IsValid || state == null) return;
        Interpolate(state, state, 1f);
    }

    public void UpdateTimeOfDay(WeatherStateSO currentState, float dayNightFactor)
    {
        if (!IsValid || currentState == null) return;

        fog.meanFreePath.value = Mathf.Lerp(
            currentState.meanFreePath * 0.5f,  // Reduced visibility at night
            currentState.meanFreePath,
            dayNightFactor
        );
        
        fog.maximumHeight.value = Mathf.Lerp(
            currentState.maximumHeight * 0.75f,
            currentState.maximumHeight,
            dayNightFactor
        );
    }
}