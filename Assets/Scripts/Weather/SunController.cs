using UnityEngine;
using System;

public class SunController : MonoBehaviour, ISunController
{
    [Header("Time Settings")]
    [SerializeField] private float dayLengthInMinutes = 24f;
    [SerializeField] private float startTimeOfDay = 12f; // 24-hour format
    
    [Header("Location Settings")]
    [SerializeField] private SunModeSO locationSettings;
    [SerializeField] [Range(1, 12)] private int currentMonth = 6; // 1 = January, 6 = June, etc.

    private float timeElapsed;
    private float currentTimeOfDay;
    public int CurrentMonth => currentMonth;

    private void OnValidate()
    {
        if (locationSettings == null)
        {
            Debug.LogWarning("No location settings assigned to SunController!");
        }
    }

    private void Start()
    {
        currentTimeOfDay = startTimeOfDay;
        UpdateSunPosition();
    }

    private void Update()
    {
        // Update time
        timeElapsed += Time.deltaTime;
        currentTimeOfDay += (24f * Time.deltaTime) / (dayLengthInMinutes * 60f);
        
        // Reset day when it reaches 24
        if (currentTimeOfDay >= 24f)
        {
            currentTimeOfDay = 0f;
        }

        UpdateSunPosition();
    }

    public void SetTime(float hour)
    {
        currentTimeOfDay = Mathf.Clamp(hour, 0f, 24f);
        UpdateSunPosition();
    }

    private void UpdateSunPosition()
    {
        if (locationSettings == null) return;

        // Calculate sun height based on location settings
        float xRotation = locationSettings.CalculateSunHeight(currentTimeOfDay, currentMonth);
        
        // Calculate azimuth (horizontal rotation)
        float timeAngle = (currentTimeOfDay - 12f) * 15f; // 15 degrees per hour
        
        // Apply rotation
        transform.rotation = Quaternion.Euler(xRotation, timeAngle, 0f);
    }

    // Public method to set the current month (1-12)
    public void SetMonth(int month)
    {
        currentMonth = Mathf.Clamp(month, 1, 12);
        UpdateSunPosition();
    }

    // Public method to get current time in 24-hour format
    public float GetCurrentTime()
    {
        return currentTimeOfDay;
    }
    
    // Method to change location settings at runtime
    public void SetLocationSettings(SunModeSO newSettings)
    {
        if (newSettings == null) return;
        locationSettings = newSettings;
        UpdateSunPosition();
    }
    
    public SunModeSO GetCurrentLocationSettings()
    {
        return locationSettings;
    }
}