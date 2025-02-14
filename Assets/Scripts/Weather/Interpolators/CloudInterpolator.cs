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

    public override void Interpolate(WeatherStateSO from, WeatherStateSO to, float t)
    {
        if (!IsValid || from == null || to == null) return;

        // Basic cloud settings
        cloudLayer.opacity.value = Mathf.Lerp(
            from.enableClouds ? from.opacity : 0f,
            to.enableClouds ? to.opacity : 0f,
            t
        );

        cloudLayer.upperHemisphereOnly.value = to.upperHemisphereOnly;

        // Layer A settings (main cloud layer)
        cloudLayer.layerA.altitude.value = Mathf.Lerp(from.altitude, to.altitude, t);
        cloudLayer.layerA.rotation.value = Mathf.Lerp(from.rotation, to.rotation, t);
        cloudLayer.layerA.tint.value = Color.Lerp(from.cloudTint, to.cloudTint, t);
        cloudLayer.layerA.exposure.value = Mathf.Lerp(from.exposureCompensation, to.exposureCompensation, t);

        // Wind settings
        cloudLayer.layerA.scrollOrientation.Override(new WindParameter.WindParamaterValue {
            mode = WindParameter.WindOverrideMode.Custom,
            customValue = Mathf.Lerp(from.windOrientation, to.windOrientation, t)
        });

        cloudLayer.layerA.scrollSpeed.Override(new WindParameter.WindParamaterValue {
            mode = WindParameter.WindOverrideMode.Custom,
            customValue = Mathf.Lerp(from.windSpeed, to.windSpeed, t)
        });
        
        // Raymarching settings
        cloudLayer.layerA.lighting.value = to.enableRaymarching;
        cloudLayer.layerA.steps.value = Mathf.RoundToInt(Mathf.Lerp(from.numPrimarySteps, to.numPrimarySteps, t));
        cloudLayer.layerA.thickness.value = Mathf.Lerp(from.raymarchingDensity, to.raymarchingDensity, t);
        cloudLayer.layerA.ambientProbeDimmer.value = Mathf.Lerp(from.ambientDimmer, to.ambientDimmer, t);

        // Shadow settings
        cloudLayer.layerA.castShadows.value = to.enableShadows;
        cloudLayer.shadowMultiplier.value = Mathf.Lerp(from.shadowMultiplier, to.shadowMultiplier, t);
        cloudLayer.shadowTint.value = Color.Lerp(from.shadowTint, to.shadowTint, t);
        cloudLayer.shadowSize.value = Mathf.Lerp(from.shadowResolution, to.shadowResolution, t);
    }

    public override void ApplyImmediate(WeatherStateSO state)
    {
        if (!IsValid || state == null) return;
        Interpolate(state, state, 1f);
    }
}