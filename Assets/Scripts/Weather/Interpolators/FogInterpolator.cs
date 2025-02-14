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

    public void ApplySnapshot(WeatherStateSnapshot snapshot)
    {
        if (!IsValid || snapshot == null) return;

        // Basic fog settings
        fog.active = snapshot.enableVolumetricFog;
        
        // Fog attenuation
        fog.meanFreePath.value = snapshot.meanFreePath;
        fog.baseHeight.value = snapshot.baseHeight;
        fog.maximumHeight.value = snapshot.maximumHeight;
        
        // Fog color and mip settings
        fog.albedo.value = snapshot.albedo;
        fog.mipFogNear.value = snapshot.mipFogNear;
        fog.mipFogFar.value = snapshot.mipFogFar;
        fog.mipFogMaxMip.value = snapshot.mipFogMaxMip;

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

    public override void Interpolate(WeatherStateSO from, WeatherStateSO to, float t)
    {
        if (!IsValid || from == null || to == null) return;
        
        var snapshot = WeatherStateSnapshot.Lerp(
            WeatherStateSnapshot.CreateFromState(from),
            WeatherStateSnapshot.CreateFromState(to),
            t
        );
        
        ApplySnapshot(snapshot);
    }

    public override void ApplyImmediate(WeatherStateSO state)
    {
        if (!IsValid || state == null) return;
        ApplySnapshot(WeatherStateSnapshot.CreateFromState(state));
    }

    public void UpdateTimeOfDay(WeatherStateSnapshot snapshot, float dayNightFactor)
    {
        if (!IsValid || snapshot == null) return;

        fog.meanFreePath.value = Mathf.Lerp(
            snapshot.meanFreePath * 0.5f,  // Reduced visibility at night
            snapshot.meanFreePath,
            dayNightFactor
        );
        
        fog.maximumHeight.value = Mathf.Lerp(
            snapshot.maximumHeight * 0.75f,
            snapshot.maximumHeight,
            dayNightFactor
        );
    }
}
