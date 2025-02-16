using UnityEngine;

[System.Serializable]
public class RainStateComponent
{
    [Header("Basic Settings")]
    public bool enableRain = false;
    
    [Header("Rain Properties")]
    [Range(0f, 1f)]
    public float intensity = 0f;
    public float dropSize = 1f;
    [Min(0.1f)]
    public float fallSpeed = 7f;

    [Header("Emission Area")]
    public float emissionRadius = 30f;
    public float emissionHeight = 20f;

    [Header("Visual Settings")]
    public Color rainColor = Color.white;
    [Range(0f, 1f)]
    public float rainAlpha = 0.5f;
    
    [Header("Sound Settings")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;
}