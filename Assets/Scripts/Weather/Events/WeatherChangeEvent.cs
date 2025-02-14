using UnityEngine.Events;
using System;

[Serializable]
public class WeatherChangeEvent : UnityEvent<WeatherStateSO, WeatherStateSO> { } // from, to
