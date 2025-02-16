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
            enabled = state.enableVolumetricFog,
            meanFreePath = state.meanFreePath,
            baseHeight = state.baseHeight,
            maximumHeight = state.maximumHeight,
            albedo = state.albedo,
            mipFogNear = state.mipFogNear,
            mipFogFar = state.mipFogFar,
            mipFogMaxMip = state.mipFogMaxMip,
            globalLightProbeDimmer = state.globalLightProbeDimmer,
            depthExtent = state.depthExtent,
            sliceDistributionUniformity = state.sliceDistributionUniformity,
            volumetricFogBudget = state.volumetricFogBudget,
            resolutionDepthRatio = state.resolutionDepthRatio,
            directionalLightsOnly = state.directionalLightsOnly,
            anisotropy = state.anisotropy,
            multiScattering = state.multiScattering
        };
        componentData.SetComponentData(typeof(FogComponent), fogData);

        // Create Cloud Component Data
        var cloudData = new CloudComponentData
        {
            enabled = state.enableClouds,
            opacity = state.opacity,
            upperHemisphereOnly = state.upperHemisphereOnly,
            altitude = state.altitude,
            rotation = state.rotation,
            tint = state.cloudTint,
            exposureCompensation = state.exposureCompensation,
            opacityR = state.opacityR,
            opacityG = state.opacityG,
            opacityB = state.opacityB,
            opacityA = state.opacityA,
            windOrientation = state.windOrientation,
            windSpeed = state.windSpeed,
            enableRaymarching = state.enableRaymarching,
            numPrimarySteps = state.numPrimarySteps,
            raymarchingDensity = state.raymarchingDensity,
            ambientDimmer = state.ambientDimmer,
            enableShadows = state.enableShadows,
            shadowMultiplier = state.shadowMultiplier,
            shadowTint = state.shadowTint,
            shadowResolution = state.shadowResolution
        };
        componentData.SetComponentData(typeof(CloudComponent), cloudData);

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