using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class CloudInterpolator : WeatherInterpolator
{
    private CloudLayer cloudLayer;

    public CloudInterpolator(VolumeProfile profile) : base(profile) { }

    protected override bool Initialize(VolumeProfile profile)
    {
        return profile.TryGet(out cloudLayer);
    }

    public void ApplySnapshot(WeatherStateSnapshot snapshot)
    {
        if (!IsValid || snapshot == null) return;

        // Basic cloud settings
        cloudLayer.opacity.value = snapshot.enableClouds ? snapshot.opacity : 0f;
        cloudLayer.upperHemisphereOnly.value = snapshot.upperHemisphereOnly;

        // Layer A settings (main cloud layer)
        cloudLayer.layerA.altitude.value = snapshot.altitude;
        cloudLayer.layerA.rotation.value = snapshot.rotation;
        cloudLayer.layerA.tint.value = snapshot.cloudTint;
        cloudLayer.layerA.exposure.value = snapshot.exposureCompensation;

        // Opacity settings
        cloudLayer.layerA.opacityR.value = snapshot.opacityChannels[0];
        cloudLayer.layerA.opacityG.value = snapshot.opacityChannels[1];
        cloudLayer.layerA.opacityB.value = snapshot.opacityChannels[2];
        cloudLayer.layerA.opacityA.value = snapshot.opacityChannels[3];
        
        // Wind settings
        cloudLayer.layerA.scrollOrientation.Override(new WindParameter.WindParamaterValue
        {
            mode = WindParameter.WindOverrideMode.Custom,
            customValue = snapshot.windOrientation
        });

        cloudLayer.layerA.scrollSpeed.Override(new WindParameter.WindParamaterValue
        {
            mode = WindParameter.WindOverrideMode.Custom,
            customValue = snapshot.windSpeed
        });

        // Raymarching settings
        cloudLayer.layerA.lighting.value = snapshot.enableRaymarching;
        cloudLayer.layerA.steps.value = snapshot.numPrimarySteps;
        cloudLayer.layerA.thickness.value = snapshot.raymarchingDensity;
        cloudLayer.layerA.ambientProbeDimmer.value = snapshot.ambientDimmer;

        // Shadow settings
        cloudLayer.layerA.castShadows.value = snapshot.enableShadows;
        cloudLayer.shadowMultiplier.value = snapshot.shadowMultiplier;
        cloudLayer.shadowTint.value = snapshot.shadowTint;
        cloudLayer.shadowSize.value = snapshot.shadowResolution;
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
}