// Interface to allow time control
public interface ISunController
{
    void SetTime(float hour);
    float GetCurrentTime();
    void SetMonth(int month);
    int CurrentMonth { get; }
}