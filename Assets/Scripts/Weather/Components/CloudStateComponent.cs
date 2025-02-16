using UnityEngine;

[System.Serializable]
public class CloudStateComponent
{
    [Header("Basic Settings")]
    public bool enableClouds = true;
    public float opacity = 1f;
    public bool upperHemisphereOnly = true;
    
    [Header("Layer Settings")]
    public float altitude = 2000f;
    public float rotation = 0f;
    public Color cloudTint = Color.white;
    public float exposureCompensation = 0f;
    
    [Header("Opacity Channels")]
    [Tooltip("Opacity multiplier for the red channel."), Range(0.0f,1.0f)]
    public float opacityR = 1.0f;
    [Tooltip("Opacity multiplier for the green channel."), Range(0.0f,1.0f)]
    public float opacityG = 0.0f;
    [Tooltip("Opacity multiplier for the blue channel."), Range(0.0f,1.0f)]
    public float opacityB = 0.0f;
    [Tooltip("Opacity multiplier for the alpha channel."), Range(0.0f,1.0f)]
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
}