// WeatherComponentManager.cs
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class WeatherComponentManager : MonoBehaviour
{
    private Dictionary<System.Type, IWeatherComponent> components = new Dictionary<System.Type, IWeatherComponent>();
    private VolumeProfile volumeProfile;

    public void Initialize(VolumeProfile profile)
    {
        this.volumeProfile = profile;
        RegisterComponent<FogComponent>();
        RegisterComponent<CloudComponent>();
    }

    private void RegisterComponent<T>() where T : IWeatherComponent, new()
    {
        var component = new T();
        component.Initialize(volumeProfile);
        components[typeof(T)] = component;
    }

    public T GetWeatherComponent<T>() where T : IWeatherComponent
    {
        if (components.TryGetValue(typeof(T), out var component))
        {
            return (T)component;
        }
        return default;
    }

    public void InterpolateAll(WeatherStateComponentData from, WeatherStateComponentData to, float t)
    {
        foreach (var component in components.Values)
        {
            var fromData = from.GetComponentData(component.GetType());
            var toData = to.GetComponentData(component.GetType());
            if (fromData != null && toData != null)
            {
                component.Interpolate(fromData, toData, t);
            }
        }
    }

    public void ApplyImmediate(WeatherStateComponentData state)
    {
        foreach (var component in components.Values)
        {
            var data = state.GetComponentData(component.GetType());
            if (data != null)
            {
                component.ApplyImmediate(data);
            }
        }
    }

    public WeatherStateComponentData CreateSnapshot()
    {
        var snapshot = new WeatherStateComponentData();
        foreach (var kvp in components)
        {
            var componentData = kvp.Value.CreateSnapshot();
            snapshot.SetComponentData(kvp.Key, componentData);
        }
        return snapshot;
    }
}
