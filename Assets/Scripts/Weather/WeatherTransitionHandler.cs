using UnityEngine;

[RequireComponent(typeof(WeatherComponentManager))]
public class WeatherTransitionHandler : MonoBehaviour
{
    private WeatherComponentManager componentManager;
    private WeatherStateComponentData currentState;
    private WeatherStateComponentData targetState;
    private float transitionProgress;
    private float transitionDuration;
    private bool isTransitioning;

    private void Awake()
    {
        componentManager = GetComponent<WeatherComponentManager>();
    }

    public void StartTransition(WeatherStateComponentData current, WeatherStateSO target, float duration)
    {
        currentState = current;
        targetState = WeatherStateComponentData.CreateFromState(target);
        transitionDuration = duration;
        transitionProgress = 0f;
        isTransitioning = true;
    }

    public WeatherStateComponentData UpdateTransition(float deltaTime)
    {
        if (!isTransitioning) return currentState;

        transitionProgress += deltaTime;
        float t = Mathf.Clamp01(transitionProgress / transitionDuration);

        var interpolatedState = WeatherStateComponentData.Lerp(currentState, targetState, t);
        componentManager.InterpolateAll(currentState, targetState, t);

        if (t >= 1f)
        {
            isTransitioning = false;
            currentState = targetState;
        }

        return interpolatedState;
    }

    public bool IsTransitioning => isTransitioning;

    public void InterruptTransition(WeatherStateSO newTarget, float newDuration)
    {
        // Create a snapshot of current interpolated values
        var interpolatedState = WeatherStateComponentData.Lerp(currentState, targetState, 
            transitionProgress / transitionDuration);
        
        // Start new transition from current interpolated values
        StartTransition(interpolatedState, newTarget, newDuration);
    }
}