using UnityEngine;

[CreateAssetMenu(fileName = "New Sun Mode", menuName = "Antarctic/Sun Mode")]
public class SunModeSO : ScriptableObject
{
    [Header("Location Settings")]
    [Tooltip("Descriptive name for this location (e.g. Stockholm, Sweden)")]
    public string locationName = "Default";
    [Tooltip("Additional details about the location")]
    public string locationDescription = "";
    
    [Header("Behavior Type")]
    [SerializeField] private SunBehaviorType behaviorType = SunBehaviorType.Standard;
    
    [Header("Standard Mode Settings")]
    [Tooltip("Latitude in degrees (-90 to 90). Used only in Standard mode")]
    [Range(-90f, 90f)] 
    public float latitude = 0f;
    
    [Header("Antarctic/Custom Mode Settings")]
    [Tooltip("Maximum sun height in degrees. Used in Antarctic and Custom modes")]
    [SerializeField] private float maxSunHeight = 47.0f;
    [Tooltip("Minimum sun height in degrees. Used in Antarctic and Custom modes")]
    [SerializeField] private float minSunHeight = -15.0f;
    
    public enum SunBehaviorType
    {
        Standard,   // Regular day/night cycle based on latitude
        Antarctic,  // Special polar day/night behavior
        Custom      // Custom behavior defined by min/max heights
    }

    public SunBehaviorType BehaviorType => behaviorType;
    
    private void OnValidate()
    {
        // Ensure heights are in valid range
        maxSunHeight = Mathf.Clamp(maxSunHeight, -90f, 90f);
        minSunHeight = Mathf.Clamp(minSunHeight, -90f, 90f);
        
        // Ensure max is greater than min
        if (minSunHeight > maxSunHeight)
        {
            float temp = maxSunHeight;
            maxSunHeight = minSunHeight;
            minSunHeight = temp;
        }
        
        // Set appropriate defaults based on mode
        switch (behaviorType)
        {
            case SunBehaviorType.Standard:
                // Standard mode uses latitude to calculate heights
                break;
                
            case SunBehaviorType.Antarctic:
                // Antarctic mode uses typical polar values
                if (maxSunHeight == 90f && minSunHeight == -90f)
                {
                    maxSunHeight = 47.0f;
                    minSunHeight = -15.0f;
                }
                break;
                
            case SunBehaviorType.Custom:
                // Custom mode allows any valid values
                break;
        }
    }

    // Your existing calculation methods remain the same
    public float CalculateSunHeight(float timeOfDay, int month)
    {
        switch (behaviorType)
        {
            case SunBehaviorType.Antarctic:
                return CalculateAntarcticSunHeight(timeOfDay, month);
            case SunBehaviorType.Standard:
                return CalculateStandardSunHeight(timeOfDay, month);
            case SunBehaviorType.Custom:
                return CalculateCustomSunHeight(timeOfDay, month);
            default:
                return 0f;
        }
    }
    
    private float CalculateAntarcticSunHeight(float timeOfDay, int month)
    {
        // Calculate seasonal influence (sine wave over the year)
        // Southern hemisphere, so peak is in December (month 12)
        float seasonalFactor = Mathf.Sin((month - 3f) * Mathf.PI / 6f);
        
        // Calculate current maximum height based on season
        float currentMaxHeight = Mathf.Lerp(minSunHeight, maxSunHeight, (seasonalFactor + 1f) / 2f);
        
        // Calculate time-based angle
        float timeAngle = (timeOfDay - 12f) * 15f; // 15 degrees per hour
        
        if (currentMaxHeight > 0)
        {
            // During polar day (summer)
            return currentMaxHeight * Mathf.Sin(timeAngle * Mathf.Deg2Rad);
        }
        
        // During polar night (winter)
        return currentMaxHeight;
    }
    
    private float CalculateStandardSunHeight(float timeOfDay, int month)
    {
        // Calculate declination angle based on month (approximate)
        float declination = -23.45f * Mathf.Cos((360f/365f) * ((month * 30.44f) + 10f) * Mathf.Deg2Rad);
        
        // Calculate hour angle
        float hourAngle = (timeOfDay - 12f) * 15f;
        
        // Calculate sun height using solar elevation formula
        float latRad = latitude * Mathf.Deg2Rad;
        float declRad = declination * Mathf.Deg2Rad;
        float hourRad = hourAngle * Mathf.Deg2Rad;
        
        float sinAltitude = Mathf.Sin(latRad) * Mathf.Sin(declRad) + 
                           Mathf.Cos(latRad) * Mathf.Cos(declRad) * Mathf.Cos(hourRad);
        
        return Mathf.Asin(sinAltitude) * Mathf.Rad2Deg;
    }
    
    private float CalculateCustomSunHeight(float timeOfDay, int month)
    {
        // Simple lerp between min and max height based on time of day and month
        float monthFactor = Mathf.Sin((month - 3f) * Mathf.PI / 6f);
        float currentMaxHeight = Mathf.Lerp(minSunHeight, maxSunHeight, (monthFactor + 1f) / 2f);
        
        // Time-based factor (peaks at noon)
        float timeFactor = Mathf.Sin((timeOfDay / 24f) * Mathf.PI);
        
        return currentMaxHeight * timeFactor;
    }

    // Getter methods for heights
    public float GetMaxSunHeight() => maxSunHeight;
    public float GetMinSunHeight() => minSunHeight;
}