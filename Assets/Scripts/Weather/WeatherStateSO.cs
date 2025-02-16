// WeatherStateSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Weather State", menuName = "Antarctic/Weather State")]
public class WeatherStateSO : ScriptableObject
{
    [Header("Basic Settings")]
    public string stateName;
    public float transitionDuration = 60f;
    
    [Header("Components")]
    public FogStateComponent fog;
    public CloudStateComponent clouds;
    public RainStateComponent rain;

    [Header("Time-Based Probability")]
    [Tooltip("Probability multiplier during daytime (0-1)")]
    [Range(0, 1)] public float dayTimeProbability = 1f;
    [Tooltip("Probability multiplier during nighttime (0-1)")]
    [Range(0, 1)] public float nightTimeProbability = 1f;
    
    [Header("Duration Settings")]
    public float minDuration = 300f;
    public float maxDuration = 1200f;
}
