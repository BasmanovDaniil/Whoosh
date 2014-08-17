using System;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    public void StartCountdown(Action onFinished)
    {
        Pulse(() => Pulse(() => Pulse(onFinished)));
    }

    private void Pulse(Action onFinished)
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.5f).onComplete =
            () => { LeanTween.scale(gameObject, Vector3.one, 0.5f).onComplete = onFinished; };
    }
}