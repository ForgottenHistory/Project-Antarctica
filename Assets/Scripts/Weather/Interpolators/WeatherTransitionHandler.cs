using UnityEngine;

public class WeatherTransitionHandler : MonoBehaviour
{
    private WeatherStateSnapshot currentSnapshot;
    private WeatherStateSnapshot targetSnapshot;
    private float transitionProgress;
    private float transitionDuration;
    private bool isTransitioning;

    public void StartTransition(WeatherStateSnapshot current, WeatherStateSO target, float duration)
    {
        currentSnapshot = current;
        targetSnapshot = WeatherStateSnapshot.CreateFromState(target);
        transitionDuration = duration;
        transitionProgress = 0f;
        isTransitioning = true;
    }

    public WeatherStateSnapshot UpdateTransition(float deltaTime)
    {
        if (!isTransitioning) return currentSnapshot;

        transitionProgress += deltaTime;
        float t = Mathf.Clamp01(transitionProgress / transitionDuration);

        var interpolatedSnapshot = WeatherStateSnapshot.Lerp(currentSnapshot, targetSnapshot, t);

        if (t >= 1f)
        {
            isTransitioning = false;
            currentSnapshot = targetSnapshot;
        }

        return interpolatedSnapshot;
    }

    public bool IsTransitioning => isTransitioning;

    public void InterruptTransition(WeatherStateSO newTarget, float newDuration)
    {
        // Create a snapshot of current interpolated values
        var interpolatedSnapshot = WeatherStateSnapshot.Lerp(currentSnapshot, targetSnapshot, 
            transitionProgress / transitionDuration);
        
        // Start new transition from current interpolated values
        StartTransition(interpolatedSnapshot, newTarget, newDuration);
    }
}