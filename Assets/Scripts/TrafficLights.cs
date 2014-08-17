using System;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    public void StartCountdown(int count, Action onStep, Action onFinished)
    {
        Pulse(() =>
        {
            count--;
            if (count > 0)
            {
                onStep();
                StartCountdown(count, onStep, onFinished);
            }
            else
            {
                onFinished();
            }
        });
    }

    private void Pulse(Action onFinished)
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.5f).onComplete =
            () => { LeanTween.scale(gameObject, Vector3.one, 0.5f).onComplete = onFinished; };
    }
}