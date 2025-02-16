using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(WeatherTransitionHandler))]
[RequireComponent(typeof(WeatherComponentManager))]
public class WeatherStateManager : MonoBehaviour
{
    // Public properties for debug and external access
    public WeatherStateSO CurrentState => currentState;
    public WeatherStateSO TargetState => targetState;
    public float TransitionProgress => targetState != null ? transitionTimer / targetState.transitionDuration : 0f;
    public bool RandomWeatherEnabled => enableRandomWeather;
    public IReadOnlyList<WeatherStateSO> AvailableStates => availableWeatherStates;
    public float CurrentWeatherTimer => weatherTimer;

    [Header("References")]
    [SerializeField] private VolumeProfile volumeProfile;
    [SerializeField] private SunController sunController;

    [Header("Weather States")]
    [SerializeField] private List<WeatherStateSO> availableWeatherStates;
    [SerializeField] private WeatherStateSO defaultState;

    [Header("System Settings")]
    [SerializeField] private bool enableRandomWeather = true;
    [SerializeField] private float weatherCheckInterval = 60f;

    private WeatherTransitionHandler transitionHandler;
    private WeatherComponentManager componentManager;
    private WeatherStateComponentData currentSnapshot;
    
    public WeatherChangeEvent onWeatherStateChanged;
    public WeatherChangeEvent onWeatherStateStartTransition;
    public TimeOfDayEvent onTimeOfDayChanged;

    private WeatherStateSO currentState;
    private WeatherStateSO targetState;
    private float transitionTimer;
    private float weatherTimer;
    private float lastTimeCheck;
    private float lastTimeOfDay = -1f;

    private void Start()
    {
        InitializeComponents();
        InitializeEvents();
        SetInitialState();
    }

    private void InitializeComponents()
    {
        transitionHandler = GetComponent<WeatherTransitionHandler>();
        componentManager = GetComponent<WeatherComponentManager>();
        componentManager.Initialize(volumeProfile);

        ValidateWeatherStates();
    }

    private void ValidateWeatherStates()
    {
        if (availableWeatherStates == null || availableWeatherStates.Count == 0)
        {
            Debug.LogWarning("No weather states configured!");
            enabled = false;
            return;
        }

        availableWeatherStates = availableWeatherStates.Where(x => x != null).ToList();

        if (defaultState == null)
        {
            defaultState = availableWeatherStates[0];
            Debug.LogWarning("No default weather state set. Using first available state.");
        }
    }

    private void InitializeEvents()
    {
        onWeatherStateChanged ??= new WeatherChangeEvent();
        onWeatherStateStartTransition ??= new WeatherChangeEvent();
        onTimeOfDayChanged ??= new TimeOfDayEvent();
    }

    private void SetInitialState()
    {
        currentState = defaultState;
        targetState = defaultState;
        currentSnapshot = WeatherStateComponentData.CreateFromState(defaultState);
        ApplyWeatherState(defaultState);
        weatherTimer = Random.Range(defaultState.minDuration, defaultState.maxDuration);
    }

    private void Update()
    {
        if (!enabled) return;

        UpdateTimeOfDay();
        if (enableRandomWeather)
        {
            UpdateWeatherSystem();
        }
    }

    private void UpdateTimeOfDay()
    {
        if (sunController == null) return;

        float timeOfDay = sunController.GetCurrentTime();

        if (Mathf.Abs(timeOfDay - lastTimeOfDay) >= 0.1f)
        {
            onTimeOfDayChanged.Invoke(timeOfDay);
            lastTimeOfDay = timeOfDay;
            UpdateWeatherForTimeOfDay(timeOfDay);
        }
    }

    private void UpdateWeatherForTimeOfDay(float timeOfDay)
    {
        if (currentSnapshot == null) return;
        float dayNightFactor = GetDayNightFactor(timeOfDay);
        
        var fogComponent = componentManager.GetWeatherComponent<FogComponent>();
        if (fogComponent != null)
        {
            var fogData = currentSnapshot.GetComponentData(typeof(FogComponent));
            fogComponent.UpdateTimeOfDay(fogData, dayNightFactor);
        }
    }

    private void UpdateWeatherSystem()
    {
        weatherTimer -= Time.deltaTime;

        if (weatherTimer <= 0)
        {
            SelectNewWeatherState();
        }

        if (transitionHandler.IsTransitioning)
        {
            currentSnapshot = transitionHandler.UpdateTransition(Time.deltaTime);
        }
    }

    private void SelectNewWeatherState()
    {
        float timeOfDay = sunController.GetCurrentTime();
        bool isDay = IsDay(timeOfDay);

        var weightedStates = availableWeatherStates
            .Select(state => new
            {
                State = state,
                Weight = isDay ? state.dayTimeProbability : state.nightTimeProbability
            })
            .Where(x => x.Weight > 0 && x.State != targetState)
            .ToList();

        if (weightedStates.Count == 0) return;

        float totalWeight = weightedStates.Sum(x => x.Weight);
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var weightedState in weightedStates)
        {
            currentWeight += weightedState.Weight;
            if (randomValue <= currentWeight)
            {
                SetTargetWeather(weightedState.State);
                break;
            }
        }

        weatherTimer = Random.Range(targetState.minDuration, targetState.maxDuration);
    }

    private void ApplyWeatherState(WeatherStateSO state)
    {
        if (state == null) return;

        var componentData = WeatherStateComponentData.CreateFromState(state);
        componentManager.ApplyImmediate(componentData);
        currentSnapshot = componentData;
    }

    private float GetDayNightFactor(float timeOfDay)
    {
        if (timeOfDay > 12f)
            timeOfDay = 24f - timeOfDay;

        return Mathf.Clamp01((timeOfDay - 3f) / 6f);
    }

    private bool IsDay(float timeOfDay)
    {
        return timeOfDay >= 6f && timeOfDay <= 18f;
    }

    // Public control methods
    public void SetTargetWeather(WeatherStateSO newState)
    {
        if (newState == null) return;

        var oldState = targetState;
        targetState = newState;

        if (transitionHandler.IsTransitioning)
        {
            transitionHandler.InterruptTransition(newState, newState.transitionDuration);
        }
        else
        {
            transitionHandler.StartTransition(currentSnapshot, newState, newState.transitionDuration);
        }

        onWeatherStateStartTransition.Invoke(oldState, targetState);
    }

    public void ForceWeatherState(WeatherStateSO state)
    {
        if (state == null) return;

        var oldState = currentState;
        currentState = state;
        targetState = state;

        ApplyWeatherState(state);
        onWeatherStateChanged.Invoke(oldState, state);
    }

    public void SetRandomWeatherEnabled(bool enabled)
    {
        enableRandomWeather = enabled;
        if (enabled)
        {
            weatherTimer = Mathf.Min(weatherTimer, weatherCheckInterval);
        }
    }

    public void ResetWeatherTimer()
    {
        if (targetState != null)
        {
            weatherTimer = Random.Range(targetState.minDuration, targetState.maxDuration);
        }
    }

    public void CancelTransition()
    {
        if (currentState != targetState)
        {
            targetState = currentState;
            ApplyWeatherState(currentState);
            transitionHandler.StartTransition(currentSnapshot, currentState, 0f);
        }
    }
}