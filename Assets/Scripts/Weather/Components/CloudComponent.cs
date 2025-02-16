using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class CloudComponent : IWeatherComponent
{
    private VolumeProfile profile;
    private CloudLayer cloudLayer;
    private bool isInitialized;

    public void Initialize(VolumeProfile profile)
    {
        this.profile = profile;
        isInitialized = profile.TryGet(out cloudLayer);
        if (!isInitialized)
        {
            Debug.LogError("Failed to initialize CloudComponent: CloudLayer not found in VolumeProfile");
        }
    }

    public void Interpolate(IWeatherComponentData from, IWeatherComponentData to, float t)
    {
        if (!isInitialized) return;
        if (from is CloudComponentData fromData && to is CloudComponentData toData)
        {
            var interpolated = (CloudComponentData)fromData.Lerp(toData, t);
            ApplyImmediate(interpolated);
        }
    }

    public void ApplyImmediate(IWeatherComponentData data)
    {
        if (!isInitialized || !(data is CloudComponentData cloudData)) 
        {
            Debug.LogWarning($"Cannot apply cloud data. Initialized: {isInitialized}");
            return;
        }

        // Re-get the component to ensure we have the current reference
        if (!profile.TryGet(out cloudLayer))
        {
            Debug.LogError("Lost reference to cloud component");
            return;
        }

        // Basic cloud settings
        cloudLayer.opacity.value = cloudData.enabled ? cloudData.opacity : 0f;
        cloudLayer.upperHemisphereOnly.value = cloudData.upperHemisphereOnly;

        // Layer A settings (main cloud layer)
        cloudLayer.layerA.altitude.value = cloudData.altitude;
        cloudLayer.layerA.rotation.value = cloudData.rotation;
        cloudLayer.layerA.tint.value = cloudData.tint;
        cloudLayer.layerA.exposure.value = cloudData.exposureCompensation;

        // Opacity settings
        cloudLayer.layerA.opacityR.value = cloudData.opacityR;
        cloudLayer.layerA.opacityG.value = cloudData.opacityG;
        cloudLayer.layerA.opacityB.value = cloudData.opacityB;
        cloudLayer.layerA.opacityA.value = cloudData.opacityA;

        // Wind settings
        cloudLayer.layerA.scrollOrientation.Override(new WindParameter.WindParamaterValue
        {
            mode = WindParameter.WindOverrideMode.Custom,
            customValue = cloudData.windOrientation
        });

        cloudLayer.layerA.scrollSpeed.Override(new WindParameter.WindParamaterValue
        {
            mode = WindParameter.WindOverrideMode.Custom,
            customValue = cloudData.windSpeed
        });

        // Raymarching settings
        cloudLayer.layerA.lighting.value = cloudData.enableRaymarching;
        cloudLayer.layerA.steps.value = cloudData.numPrimarySteps;
        cloudLayer.layerA.thickness.value = cloudData.raymarchingDensity;
        cloudLayer.layerA.ambientProbeDimmer.value = cloudData.ambientDimmer;

        // Shadow settings
        cloudLayer.layerA.castShadows.value = cloudData.enableShadows;
        cloudLayer.shadowMultiplier.value = cloudData.shadowMultiplier;
        cloudLayer.shadowTint.value = cloudData.shadowTint;
        cloudLayer.shadowSize.value = cloudData.shadowResolution;
    }

    public IWeatherComponentData CreateSnapshot()
    {
        if (!isInitialized) return new CloudComponentData();

        return new CloudComponentData
        {
            enabled = cloudLayer.opacity.value > 0f,
            opacity = cloudLayer.opacity.value,
            upperHemisphereOnly = cloudLayer.upperHemisphereOnly.value,
            altitude = cloudLayer.layerA.altitude.value,
            rotation = cloudLayer.layerA.rotation.value,
            tint = cloudLayer.layerA.tint.value,
            exposureCompensation = cloudLayer.layerA.exposure.value,
            opacityR = cloudLayer.layerA.opacityR.value,
            opacityG = cloudLayer.layerA.opacityG.value,
            opacityB = cloudLayer.layerA.opacityB.value,
            opacityA = cloudLayer.layerA.opacityA.value,
            windOrientation = cloudLayer.layerA.scrollOrientation.value.customValue,
            windSpeed = cloudLayer.layerA.scrollSpeed.value.customValue,
            enableRaymarching = cloudLayer.layerA.lighting.value,
            numPrimarySteps = cloudLayer.layerA.steps.value,
            raymarchingDensity = cloudLayer.layerA.thickness.value,
            ambientDimmer = cloudLayer.layerA.ambientProbeDimmer.value,
            enableShadows = cloudLayer.layerA.castShadows.value,
            shadowMultiplier = cloudLayer.shadowMultiplier.value,
            shadowTint = cloudLayer.shadowTint.value,
            shadowResolution = cloudLayer.shadowSize.value
        };
    }
}