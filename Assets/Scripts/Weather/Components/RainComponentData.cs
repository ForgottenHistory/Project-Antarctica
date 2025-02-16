using UnityEngine;

[System.Serializable]
public class RainComponentData : IWeatherComponentData
{
    public bool enabled;
    public float intensity;
    public float dropSize;
    public float fallSpeed;
    public float emissionRadius;
    public float emissionHeight;
    public Color rainColor;
    public float rainAlpha;
    public float soundVolume;

    public void CopyFrom(IWeatherComponentData other)
    {
        if (other is RainComponentData data)
        {
            enabled = data.enabled;
            intensity = data.intensity;
            dropSize = data.dropSize;
            fallSpeed = data.fallSpeed;
            emissionRadius = data.emissionRadius;
            emissionHeight = data.emissionHeight;
            rainColor = data.rainColor;
            rainAlpha = data.rainAlpha;
            soundVolume = data.soundVolume;
        }
    }

    public IWeatherComponentData Lerp(IWeatherComponentData to, float t)
    {
        if (to is RainComponentData target)
        {
            return new RainComponentData
            {
                enabled = t < 0.5f ? enabled : target.enabled,
                intensity = Mathf.Lerp(intensity, target.intensity, t),
                dropSize = Mathf.Lerp(dropSize, target.dropSize, t),
                fallSpeed = Mathf.Lerp(fallSpeed, target.fallSpeed, t),
                emissionRadius = Mathf.Lerp(emissionRadius, target.emissionRadius, t),
                emissionHeight = Mathf.Lerp(emissionHeight, target.emissionHeight, t),
                rainColor = Color.Lerp(rainColor, target.rainColor, t),
                rainAlpha = Mathf.Lerp(rainAlpha, target.rainAlpha, t),
                soundVolume = Mathf.Lerp(soundVolume, target.soundVolume, t)
            };
        }
        return this;
    }
}