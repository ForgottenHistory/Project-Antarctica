using UnityEngine;
using UnityEngine.Rendering;

public class RainComponent : IWeatherComponent
{
    private ParticleSystem rainSystem;
    private AudioSource rainAudio;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.ShapeModule shapeModule;
    private bool isInitialized;

    private const float BASE_EMISSION_RATE = 1000f; // Base rate for intensity of 1

    public void Initialize(VolumeProfile profile)
    {
        // Find rain system in scene
        var rainObject = GameObject.Find("RainSystem");
        if (rainObject == null)
        {
            Debug.LogError("RainSystem not found in scene!");
            return;
        }

        rainSystem = rainObject.GetComponent<ParticleSystem>();
        rainAudio = rainObject.GetComponent<AudioSource>();
        
        if (rainSystem == null)
        {
            Debug.LogError("ParticleSystem not found on RainSystem!");
            return;
        }

        // Cache modules for better performance
        mainModule = rainSystem.main;
        emissionModule = rainSystem.emission;
        shapeModule = rainSystem.shape;

        // Set up base particle system settings
        mainModule.loop = true;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
        
        shapeModule.shapeType = ParticleSystemShapeType.ConeVolume;
        shapeModule.alignToDirection = true;

        isInitialized = true;
    }

    public void Interpolate(IWeatherComponentData from, IWeatherComponentData to, float t)
    {
        if (!isInitialized) return;
        
        if (from is RainComponentData fromRain && to is RainComponentData toRain)
        {
            var lerped = (RainComponentData)fromRain.Lerp(toRain, t);
            ApplyImmediate(lerped);
        }
    }

    public void ApplyImmediate(IWeatherComponentData data)
    {
        if (!isInitialized || !(data is RainComponentData rainData)) return;

        // Handle enabling/disabling
        if (rainData.enabled && !rainSystem.isPlaying)
            rainSystem.Play();
        else if (!rainData.enabled && rainSystem.isPlaying)
            rainSystem.Stop();

        // Update particle system properties
        mainModule.startSize = rainData.dropSize;
        mainModule.startSpeed = -rainData.fallSpeed; // Negative for downward motion
        mainModule.startColor = new Color(
            rainData.rainColor.r,
            rainData.rainColor.g,
            rainData.rainColor.b,
            rainData.rainAlpha
        );

        // Update emission
        var rate = new ParticleSystem.MinMaxCurve(rainData.intensity * BASE_EMISSION_RATE);
        emissionModule.rateOverTime = rate;

        // Update shape
        shapeModule.radius = rainData.emissionRadius;
        shapeModule.length = rainData.emissionHeight;

        // Update audio
        if (rainAudio != null)
        {
            rainAudio.volume = rainData.intensity * rainData.soundVolume;
            
            if (rainData.enabled && !rainAudio.isPlaying)
                rainAudio.Play();
            else if (!rainData.enabled && rainAudio.isPlaying)
                rainAudio.Stop();
        }
    }

    public IWeatherComponentData CreateSnapshot()
    {
        if (!isInitialized) return new RainComponentData();

        return new RainComponentData
        {
            enabled = rainSystem.isPlaying,
            intensity = emissionModule.rateOverTime.constant / BASE_EMISSION_RATE,
            dropSize = mainModule.startSize.constant,
            fallSpeed = -mainModule.startSpeed.constant,
            emissionRadius = shapeModule.radius,
            emissionHeight = shapeModule.length,
            rainColor = mainModule.startColor.color,
            rainAlpha = mainModule.startColor.color.a,
            soundVolume = rainAudio != null ? rainAudio.volume : 0f
        };
    }
}