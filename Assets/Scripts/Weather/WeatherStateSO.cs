using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition; // Add this for HDRP specifics

[CreateAssetMenu(fileName = "New Weather State", menuName = "Antarctic/Weather State")]
public class WeatherStateSO : ScriptableObject
{
    [Header("Basic Settings")]
    public string stateName;
    public float transitionDuration = 60f;
    
    [Header("Fog Settings")]
    public bool enableVolumetricFog = true;
    
    [Header("Fog Attenuation")]
    public float meanFreePath = 1000f;    // Correct - VolumetricFog property
    public float baseHeight = 0f;         // Correct - VolumetricFog property
    public float maximumHeight = 100f;    // Changed from maxHeight to maximumHeight
    public float maxDistanceInMeters = 500f; // Changed from maxFogDistance to match HDRP naming
    
    [Header("Fog Color")]
    public Color albedo = Color.white;    // Changed from tint to albedo, and from float to Color
    public float mipFogNear = 0f;        // Correct
    public float mipFogFar = 1000f;      // Correct
    public float mipFogMaxMip = 0.5f;    // Correct
    
    [Header("Volumetric Fog")]
    public float globalLightProbeDimmer = 1f;  // Changed from globalDimmer
    public float depthExtent = 64f;           // Changed from volumetricFogDistance
    public float sliceDistributionUniformity = 0.75f;  // Correct
    [Range(0, 1)] public float volumetricFogBudget = 0.5f;  // Correct
    [Range(0, 1)] public float resolutionDepthRatio = 0.5f; // Correct
    public bool directionalLightsOnly = false;  // Correct
    public float anisotropy = 0f;              // Correct
    public float multiScattering = 0f;         // Changed from multipleScatteringIntensity
    
    [Header("Cloud Settings")]
    public bool enableClouds = true;
    public float opacity = 1f;                 // Changed from cloudOpacity
    public bool upperHemisphereOnly = true;
    public float altitude = 2000f;             // Changed from cloudAltitude
    public float rotation = 0f;                // Changed from cloudRotation
    public Color cloudTint = Color.white;      // Correct
    public float exposureCompensation = 0f;    // Changed from cloudExposureCompensation
    
    [Header("Cloud Wind")]
    public float windOrientation = 100f;       // Correct
    public float windSpeed = 100f;             // Correct
    
    [Header("Cloud Raymarching")]
    public bool enableRaymarching = true;
    public int numPrimarySteps = 6;            // Changed from raymarchingSteps
    public float raymarchingDensity = 0.5f;    // Correct
    public float ambientDimmer = 1f;           // Changed from ambientProbeDimmer
    
    [Header("Cloud Shadows")]
    public bool enableShadows = true;          // Changed from enableCloudShadows
    public float shadowMultiplier = 1f;        // Correct
    public Color shadowTint = Color.black;     // Correct
    public float shadowResolution = 500f;      // Changed from shadowSize
    
    [Header("Time-Based Probability")]
    [Tooltip("Probability multiplier during daytime (0-1)")]
    [Range(0, 1)] public float dayTimeProbability = 1f;
    [Tooltip("Probability multiplier during nighttime (0-1)")]
    [Range(0, 1)] public float nightTimeProbability = 1f;
    
    [Header("Duration Settings")]
    public float minDuration = 300f;
    public float maxDuration = 1200f;
}