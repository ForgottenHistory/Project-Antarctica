using UnityEngine;
using UnityEngine.Rendering;

public abstract class WeatherInterpolator
{
    public bool IsValid { get; private set; }

    protected WeatherInterpolator(VolumeProfile profile)
    {
        IsValid = Initialize(profile);
    }

    protected abstract bool Initialize(VolumeProfile profile);
    public abstract void Interpolate(WeatherStateSO from, WeatherStateSO to, float t);
    public abstract void ApplyImmediate(WeatherStateSO state);
}