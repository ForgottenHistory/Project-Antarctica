using UnityEngine.Events;
using System;

[Serializable]
public class TimeOfDayEvent : UnityEvent<float> { } // current time (0-24)
