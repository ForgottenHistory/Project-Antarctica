using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class WeatherStateSnapshot
{
    // Fog Settings
    public bool enableVolumetricFog;
    public float meanFreePath;
    public float baseHeight;
    public float maximumHeight;
    public Color albedo;
    public float mipFogNear;
    public float mipFogFar;
    public float mipFogMaxMip;

    // Cloud Settings
    public bool enableClouds;
    public float opacity;
    public bool upperHemisphereOnly;
    public float altitude;
    public float rotation;
    public Color cloudTint;
    public float exposureCompensation;
    public float[] opacityChannels = new float[4]; // R,G,B,A
    public float windOrientation;
    public float windSpeed;
    public bool enableRaymarching;
    public int numPrimarySteps;
    public float raymarchingDensity;
    public float ambientDimmer;
    public bool enableShadows;
    public float shadowMultiplier;
    public Color shadowTint;
    public float shadowResolution;

    public static WeatherStateSnapshot CreateFromState(WeatherStateSO state)
    {
        var snapshot = new WeatherStateSnapshot
        {
            // Fog settings
            enableVolumetricFog = state.enableVolumetricFog,
            meanFreePath = state.meanFreePath,
            baseHeight = state.baseHeight,
            maximumHeight = state.maximumHeight,
            albedo = state.albedo,
            mipFogNear = state.mipFogNear,
            mipFogFar = state.mipFogFar,
            mipFogMaxMip = state.mipFogMaxMip,

            // Cloud settings
            enableClouds = state.enableClouds,
            opacity = state.opacity,
            upperHemisphereOnly = state.upperHemisphereOnly,
            altitude = state.altitude,
            rotation = state.rotation,
            cloudTint = state.cloudTint,
            exposureCompensation = state.exposureCompensation,
            opacityChannels = new[] { state.opacityR, state.opacityG, state.opacityB, state.opacityA },
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
        return snapshot;
    }

    public static WeatherStateSnapshot Lerp(WeatherStateSnapshot from, WeatherStateSnapshot to, float t)
    {
        var result = new WeatherStateSnapshot
        {
            // Fog settings
            enableVolumetricFog = t < 0.5f ? from.enableVolumetricFog : to.enableVolumetricFog,
            meanFreePath = Mathf.Lerp(from.meanFreePath, to.meanFreePath, t),
            baseHeight = Mathf.Lerp(from.baseHeight, to.baseHeight, t),
            maximumHeight = Mathf.Lerp(from.maximumHeight, to.maximumHeight, t),
            albedo = Color.Lerp(from.albedo, to.albedo, t),
            mipFogNear = Mathf.Lerp(from.mipFogNear, to.mipFogNear, t),
            mipFogFar = Mathf.Lerp(from.mipFogFar, to.mipFogFar, t),
            mipFogMaxMip = Mathf.Lerp(from.mipFogMaxMip, to.mipFogMaxMip, t),

            // Cloud settings
            enableClouds = t < 0.5f ? from.enableClouds : to.enableClouds,
            opacity = Mathf.Lerp(from.opacity, to.opacity, t),
            upperHemisphereOnly = t < 0.5f ? from.upperHemisphereOnly : to.upperHemisphereOnly,
            altitude = Mathf.Lerp(from.altitude, to.altitude, t),
            rotation = Mathf.Lerp(from.rotation, to.rotation, t),
            cloudTint = Color.Lerp(from.cloudTint, to.cloudTint, t),
            exposureCompensation = Mathf.Lerp(from.exposureCompensation, to.exposureCompensation, t),
            opacityChannels = new float[4],
            windOrientation = Mathf.Lerp(from.windOrientation, to.windOrientation, t),
            windSpeed = Mathf.Lerp(from.windSpeed, to.windSpeed, t),
            enableRaymarching = t < 0.5f ? from.enableRaymarching : to.enableRaymarching,
            numPrimarySteps = Mathf.RoundToInt(Mathf.Lerp(from.numPrimarySteps, to.numPrimarySteps, t)),
            raymarchingDensity = Mathf.Lerp(from.raymarchingDensity, to.raymarchingDensity, t),
            ambientDimmer = Mathf.Lerp(from.ambientDimmer, to.ambientDimmer, t),
            enableShadows = t < 0.5f ? from.enableShadows : to.enableShadows,
            shadowMultiplier = Mathf.Lerp(from.shadowMultiplier, to.shadowMultiplier, t),
            shadowTint = Color.Lerp(from.shadowTint, to.shadowTint, t),
            shadowResolution = Mathf.Lerp(from.shadowResolution, to.shadowResolution, t)
        };

        // Interpolate opacity channels
        for (int i = 0; i < 4; i++)
        {
            result.opacityChannels[i] = Mathf.Lerp(from.opacityChannels[i], to.opacityChannels[i], t);
        }

        return result;
    }
}