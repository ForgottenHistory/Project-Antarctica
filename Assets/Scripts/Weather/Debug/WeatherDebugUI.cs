using UnityEngine;
using System.Collections.Generic;

public class WeatherDebugUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeatherStateManager weatherManager;
    [SerializeField] private SunController sunController;
    
    [Header("UI Settings")]
    [SerializeField] private bool showUI = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote;
    [SerializeField] private float windowWidth = 300f;
    [SerializeField] private float windowHeight = 400f;
    
    private Vector2 scrollPosition;
    private bool showWeatherStates = true;
    private bool showTimeControls = true;
    private bool showCurrentState = true;
    private readonly Dictionary<string, bool> foldoutStates = new();
    
    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            showUI = !showUI;
        }
    }

    private void OnGUI()
    {
        if (!showUI) return;

        // Main debug window
        GUILayout.Window(0, new Rect(10, 10, windowWidth, windowHeight), DrawDebugWindow, "Weather System Debug");
    }

    private void DrawDebugWindow(int windowID)
    {
        if (weatherManager == null || sunController == null)
        {
            GUILayout.Label("Error: Missing required components!");
            return;
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        DrawTimeSection();
        DrawCurrentWeatherSection();
        DrawWeatherStatesSection();

        GUILayout.EndScrollView();
        
        // Make the window draggable
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    private void DrawTimeSection()
    {
        showTimeControls = DrawFoldout("Time Controls", showTimeControls);
        if (showTimeControls)
        {
            GUILayout.BeginVertical("box");
            
            // Display current time
            float currentTime = sunController.GetCurrentTime();
            GUILayout.Label($"Current Time: {FormatTime(currentTime)}");
            
            // Location info
            var locationSettings = sunController.GetCurrentLocationSettings();
            if (locationSettings != null)
            {
                GUILayout.Space(5);
                GUILayout.Label($"Location: {locationSettings.locationName}");
                if (!string.IsNullOrEmpty(locationSettings.locationDescription))
                {
                    GUILayout.Label($"Details: {locationSettings.locationDescription}");
                }
                if (locationSettings.BehaviorType == SunModeSO.SunBehaviorType.Standard)
                {
                    GUILayout.Label($"Latitude: {locationSettings.latitude:F1}°");
                }
            }
            
            GUILayout.Space(5);
            
            // Time controls
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("6:00 AM"))
                SetTime(6);
            if (GUILayout.Button("12:00 PM"))
                SetTime(12);
            if (GUILayout.Button("6:00 PM"))
                SetTime(18);
            if (GUILayout.Button("12:00 AM"))
                SetTime(0);
            GUILayout.EndHorizontal();

            // Month selection
            GUILayout.BeginHorizontal();
            GUILayout.Label("Month:");
            if (GUILayout.Button("<<"))
                ChangeMonth(-1);
            GUILayout.Label($"{GetMonthName(sunController.CurrentMonth)}");
            if (GUILayout.Button(">>"))
                ChangeMonth(1);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

    private void DrawCurrentWeatherSection()
    {
        showCurrentState = DrawFoldout("Current Weather", showCurrentState);
        if (showCurrentState)
        {
            GUILayout.BeginVertical("box");
            
            var currentState = weatherManager.CurrentState;
            var targetState = weatherManager.TargetState;
            
            // System Status
            bool randomEnabled = weatherManager.RandomWeatherEnabled;
            bool newRandomEnabled = GUILayout.Toggle(randomEnabled, "Random Weather Enabled");
            if (newRandomEnabled != randomEnabled)
            {
                weatherManager.SetRandomWeatherEnabled(newRandomEnabled);
            }

            if (randomEnabled)
            {
                GUILayout.Label($"Next weather change in: {weatherManager.CurrentWeatherTimer:F1}s");
                if (GUILayout.Button("Reset Timer"))
                {
                    weatherManager.ResetWeatherTimer();
                }
            }
            
            GUILayout.Space(10);
            
            // Current State
            if (currentState != null)
            {
                GUILayout.Label($"Current State: {currentState.stateName}");
            }
            
            // Transition Status
            if (targetState != null && targetState != currentState)
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical("box");
                GUILayout.Label("Transition in Progress:");
                GUILayout.Label($"Target State: {targetState.stateName}");
                GUILayout.Label($"Progress: {(weatherManager.TransitionProgress * 100):F0}%");
                
                if (GUILayout.Button("Cancel Transition"))
                {
                    weatherManager.CancelTransition();
                }
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
    }

    private void DrawWeatherStatesSection()
    {
        showWeatherStates = DrawFoldout("Available Weather States", showWeatherStates);
        if (showWeatherStates)
        {
            GUILayout.BeginVertical("box");
            
            foreach (var state in weatherManager.AvailableStates)
            {
                if (state == null) continue;

                string stateKey = $"state_{state.stateName}";
                foldoutStates[stateKey] = DrawFoldout(state.stateName, foldoutStates.GetValueOrDefault(stateKey));
                
                if (foldoutStates[stateKey])
                {
                    GUILayout.BeginVertical("box");
                    
                    // State details
                    GUILayout.Label($"Day Probability: {state.dayTimeProbability:P0}");
                    GUILayout.Label($"Night Probability: {state.nightTimeProbability:P0}");
                    
                    // Actions
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Set Target"))
                        weatherManager.SetTargetWeather(state);
                    if (GUILayout.Button("Force"))
                        weatherManager.ForceWeatherState(state);
                    GUILayout.EndHorizontal();
                    
                    GUILayout.EndVertical();
                }
            }
            
            GUILayout.EndVertical();
        }
    }

    private bool DrawFoldout(string title, bool state)
    {
        GUILayout.BeginHorizontal();
        state = GUILayout.Toggle(state, (state ? "▼ " : "► ") + title, "label");
        GUILayout.EndHorizontal();
        return state;
    }

    private void SetTime(float hour)
    {
        sunController.SetTime(hour);
    }

    private void ChangeMonth(int delta)
    {
        int newMonth = sunController.CurrentMonth + delta;
        if (newMonth < 1) newMonth = 12;
        if (newMonth > 12) newMonth = 1;
        sunController.SetMonth(newMonth);
    }

    private string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time);
        int minutes = Mathf.FloorToInt((time - hours) * 60);
        string period = hours >= 12 ? "PM" : "AM";
        hours = hours % 12;
        if (hours == 0) hours = 12;
        return $"{hours:D2}:{minutes:D2} {period}";
    }

    private string GetMonthName(int month)
    {
        return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
    }
}
