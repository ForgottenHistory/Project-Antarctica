using System;
using System.Collections.Generic;

public class WeatherStateComponentData
{
    private Dictionary<Type, IWeatherComponentData> componentData = new Dictionary<Type, IWeatherComponentData>();

    public void SetComponentData(Type componentType, IWeatherComponentData data)
    {
        componentData[componentType] = data;
    }

    public IWeatherComponentData GetComponentData(Type componentType)
    {
        componentData.TryGetValue(componentType, out var data);
        return data;
    }

    public static WeatherStateComponentData CreateFromState(WeatherStateSO state)
    {
        var componentData = new WeatherStateComponentData();

        // Create Fog Component Data
        var fogData = new FogComponentData
        {
            enabled = state.fog.enableVolumetricFog,
            meanFreePath = state.fog.meanFreePath,
            baseHeight = state.fog.baseHeight,
            maximumHeight = state.fog.maximumHeight,
            albedo = state.fog.albedo,
            mipFogNear = state.fog.mipFogNear,
            mipFogFar = state.fog.mipFogFar,
            mipFogMaxMip = state.fog.mipFogMaxMip,
            globalLightProbeDimmer = state.fog.globalLightProbeDimmer,
            depthExtent = state.fog.depthExtent,
            sliceDistributionUniformity = state.fog.sliceDistributionUniformity,
            volumetricFogBudget = state.fog.volumetricFogBudget,
            resolutionDepthRatio = state.fog.resolutionDepthRatio,
            directionalLightsOnly = state.fog.directionalLightsOnly,
            anisotropy = state.fog.anisotropy,
            multiScattering = state.fog.multiScattering
        };
        componentData.SetComponentData(typeof(FogComponent), fogData);

        // Create Cloud Component Data
        var cloudData = new CloudComponentData
        {
            enabled = state.clouds.enableClouds,
            opacity = state.clouds.opacity,
            upperHemisphereOnly = state.clouds.upperHemisphereOnly,
            altitude = state.clouds.altitude,
            rotation = state.clouds.rotation,
            tint = state.clouds.cloudTint,
            exposureCompensation = state.clouds.exposureCompensation,
            opacityR = state.clouds.opacityR,
            opacityG = state.clouds.opacityG,
            opacityB = state.clouds.opacityB,
            opacityA = state.clouds.opacityA,
            windOrientation = state.clouds.windOrientation,
            windSpeed = state.clouds.windSpeed,
            enableRaymarching = state.clouds.enableRaymarching,
            numPrimarySteps = state.clouds.numPrimarySteps,
            raymarchingDensity = state.clouds.raymarchingDensity,
            ambientDimmer = state.clouds.ambientDimmer,
            enableShadows = state.clouds.enableShadows,
            shadowMultiplier = state.clouds.shadowMultiplier,
            shadowTint = state.clouds.shadowTint,
            shadowResolution = state.clouds.shadowResolution
        };
        componentData.SetComponentData(typeof(CloudComponent), cloudData);

        var rainData = new RainComponentData
        {
            enabled = state.rain.enableRain,
            intensity = state.rain.intensity,
            dropSize = state.rain.dropSize,
            fallSpeed = state.rain.fallSpeed,
            emissionRadius = state.rain.emissionRadius,
            emissionHeight = state.rain.emissionHeight,
            rainColor = state.rain.rainColor,
            rainAlpha = state.rain.rainAlpha,
            soundVolume = state.rain.soundVolume
        };
        componentData.SetComponentData(typeof(RainComponent), rainData);

        return componentData;
    }

    public static WeatherStateComponentData Lerp(WeatherStateComponentData from, WeatherStateComponentData to, float t)
    {
        var result = new WeatherStateComponentData();

        // Get all component types from both states
        var componentTypes = new HashSet<Type>();
        foreach (var type in from.componentData.Keys) componentTypes.Add(type);
        foreach (var type in to.componentData.Keys) componentTypes.Add(type);

        // Interpolate each component
        foreach (var type in componentTypes)
        {
            var fromData = from.GetComponentData(type);
            var toData = to.GetComponentData(type);

            if (fromData != null && toData != null)
            {
                var interpolated = fromData.Lerp(toData, t);
                result.SetComponentData(type, interpolated);
            }
            else if (fromData != null)
            {
                result.SetComponentData(type, fromData);
            }
            else if (toData != null)
            {
                result.SetComponentData(type, toData);
            }
        }

        return result;
    }
}