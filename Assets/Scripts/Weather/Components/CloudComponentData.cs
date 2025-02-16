
using UnityEngine;

[System.Serializable]
public class CloudComponentData : IWeatherComponentData
{
    [Header("Basic Settings")]
    public bool enabled = true;
    public float opacity = 1f;
    public bool upperHemisphereOnly = true;

    [Header("Layer Settings")]
    public float altitude = 2000f;
    public float rotation = 0f;
    public Color tint = Color.white;
    public float exposureCompensation = 0f;

    [Header("Opacity Settings")]
    [Tooltip("Opacity multiplier for the red channel."), Range(0.0f, 1.0f)]
    public float opacityR = 1.0f;
    [Tooltip("Opacity multiplier for the green channel."), Range(0.0f, 1.0f)]
    public float opacityG = 0.0f;
    [Tooltip("Opacity multiplier for the blue channel."), Range(0.0f, 1.0f)]
    public float opacityB = 0.0f;
    [Tooltip("Opacity multiplier for the alpha channel."), Range(0.0f, 1.0f)]
    public float opacityA = 0.0f;

    [Header("Wind Settings")]
    public float windOrientation = 100f;
    public float windSpeed = 100f;

    [Header("Raymarching Settings")]
    public bool enableRaymarching = true;
    public int numPrimarySteps = 6;
    public float raymarchingDensity = 0.5f;
    public float ambientDimmer = 1f;

    [Header("Shadow Settings")]
    public bool enableShadows = true;
    public float shadowMultiplier = 1f;
    public Color shadowTint = Color.black;
    public float shadowResolution = 500f;

    public void CopyFrom(IWeatherComponentData other)
    {
        if (other is CloudComponentData data)
        {
            enabled = data.enabled;
            opacity = data.opacity;
            upperHemisphereOnly = data.upperHemisphereOnly;
            altitude = data.altitude;
            rotation = data.rotation;
            tint = data.tint;
            exposureCompensation = data.exposureCompensation;
            opacityR = data.opacityR;
            opacityG = data.opacityG;
            opacityB = data.opacityB;
            opacityA = data.opacityA;
            windOrientation = data.windOrientation;
            windSpeed = data.windSpeed;
            enableRaymarching = data.enableRaymarching;
            numPrimarySteps = data.numPrimarySteps;
            raymarchingDensity = data.raymarchingDensity;
            ambientDimmer = data.ambientDimmer;
            enableShadows = data.enableShadows;
            shadowMultiplier = data.shadowMultiplier;
            shadowTint = data.shadowTint;
            shadowResolution = data.shadowResolution;
        }
    }

    public IWeatherComponentData Lerp(IWeatherComponentData to, float t)
    {
        if (to is CloudComponentData target)
        {
            return new CloudComponentData
            {
                enabled = t < 0.5f ? enabled : target.enabled,
                opacity = Mathf.Lerp(opacity, target.opacity, t),
                upperHemisphereOnly = t < 0.5f ? upperHemisphereOnly : target.upperHemisphereOnly,
                altitude = Mathf.Lerp(altitude, target.altitude, t),
                rotation = Mathf.Lerp(rotation, target.rotation, t),
                tint = Color.Lerp(tint, target.tint, t),
                exposureCompensation = Mathf.Lerp(exposureCompensation, target.exposureCompensation, t),
                opacityR = Mathf.Lerp(opacityR, target.opacityR, t),
                opacityG = Mathf.Lerp(opacityG, target.opacityG, t),
                opacityB = Mathf.Lerp(opacityB, target.opacityB, t),
                opacityA = Mathf.Lerp(opacityA, target.opacityA, t),
                windOrientation = Mathf.Lerp(windOrientation, target.windOrientation, t),
                windSpeed = Mathf.Lerp(windSpeed, target.windSpeed, t),
                enableRaymarching = t < 0.5f ? enableRaymarching : target.enableRaymarching,
                numPrimarySteps = Mathf.RoundToInt(Mathf.Lerp(numPrimarySteps, target.numPrimarySteps, t)),
                raymarchingDensity = Mathf.Lerp(raymarchingDensity, target.raymarchingDensity, t),
                ambientDimmer = Mathf.Lerp(ambientDimmer, target.ambientDimmer, t),
                enableShadows = t < 0.5f ? enableShadows : target.enableShadows,
                shadowMultiplier = Mathf.Lerp(shadowMultiplier, target.shadowMultiplier, t),
                shadowTint = Color.Lerp(shadowTint, target.shadowTint, t),
                shadowResolution = Mathf.Lerp(shadowResolution, target.shadowResolution, t)
            };
        }
        return this;
    }
}