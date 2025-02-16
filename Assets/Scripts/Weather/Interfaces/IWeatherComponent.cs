using UnityEngine.Rendering;

public interface IWeatherComponent
{
    void Initialize(VolumeProfile profile);
    void Interpolate(IWeatherComponentData from, IWeatherComponentData to, float t);
    void ApplyImmediate(IWeatherComponentData data);
    IWeatherComponentData CreateSnapshot();
}

public interface IWeatherComponentData
{
    void CopyFrom(IWeatherComponentData other);
    IWeatherComponentData Lerp(IWeatherComponentData to, float t);
}