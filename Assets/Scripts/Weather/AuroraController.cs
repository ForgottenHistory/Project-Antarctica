using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AuroraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SunController sunController;
    
    [Header("Aurora Settings")]
    [SerializeField] [Range(0f, 1f)] private float dayOpacity = 0.0f;
    [SerializeField] [Range(0f, 1f)] private float nightOpacity = 1.0f;
    [SerializeField] [Range(0f, 2f)] private float transitionDuration = 1f;
    
    [Header("Sun Angle Settings")]
    [SerializeField] private float darkStartAngle = 195f; // Start of dark period
    [SerializeField] private float darkEndAngle = 345f;   // End of dark period
    [SerializeField] [Range(0f, 10f)] private float transitionAngleRange = 5f;
    
    private Material auroraMaterial;
    private Color currentColor;
    private float currentOpacity;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    private void Start()
    {
        if (sunController == null)
        {
            sunController = FindFirstObjectByType<SunController>();
            if (sunController == null)
            {
                Debug.LogError("No SunController found in the scene!");
                enabled = false;
                return;
            }
        }

        auroraMaterial = GetComponent<MeshRenderer>().material;
        currentColor = auroraMaterial.GetColor(ColorProperty);
        currentOpacity = currentColor.a;
    }

    private void Update()
    {
        float targetOpacity = CalculateTargetOpacity();
        
        // Smoothly transition the opacity
        currentOpacity = Mathf.Lerp(currentOpacity, targetOpacity, Time.deltaTime / transitionDuration);
        
        // Update the material color
        currentColor.a = currentOpacity;
        auroraMaterial.SetColor(ColorProperty, currentColor);
    }

    private float CalculateTargetOpacity()
    {
        float sunAngle = sunController.transform.rotation.eulerAngles.y;
        
        // Check if we're in the dark period (between darkStartAngle and darkEndAngle)
        bool isInDarkPeriod = sunAngle >= darkStartAngle && sunAngle <= darkEndAngle;
        
        if (isInDarkPeriod)
        {
            // Calculate transition near start of dark period
            if (Mathf.Abs(sunAngle - darkStartAngle) <= transitionAngleRange)
            {
                float t = (Mathf.Abs(sunAngle - darkStartAngle) / transitionAngleRange);
                return Mathf.Lerp(dayOpacity, nightOpacity, t);
            }
            
            // Calculate transition near end of dark period
            if (Mathf.Abs(sunAngle - darkEndAngle) <= transitionAngleRange)
            {
                float t = (Mathf.Abs(sunAngle - darkEndAngle) / transitionAngleRange);
                return Mathf.Lerp(nightOpacity, dayOpacity, t);
            }
            
            return nightOpacity;
        }
        
        return dayOpacity;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (darkStartAngle < 0) darkStartAngle += 360;
        if (darkEndAngle < 0) darkEndAngle += 360;
        darkStartAngle %= 360;
        darkEndAngle %= 360;
    }
#endif
}