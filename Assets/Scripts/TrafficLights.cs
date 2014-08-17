using System;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    public GameObject red;
    public GameObject yellow;
    public GameObject green;

    public void StartCountdown(int count, Action onStep, Action onFinished)
    {
        if (count == 1)
        {
            LeanTween.scale(yellow, Vector3.zero, 0.5f);
            Pulse(green, () => CheckCount(count, onStep, onFinished));
        }
        else if (count == 2)
        {
            LeanTween.scale(red, Vector3.zero, 0.5f);
            Pulse(yellow, () => CheckCount(count, onStep, onFinished));
        }
        else
        {
            Pulse(red, () => CheckCount(count, onStep, onFinished));
        }
    }

    private void CheckCount(int count, Action onStep, Action onFinished)
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
    }

    private void Pulse(GameObject go, Action onFinished)
    {
        LeanTween.scale(go, Vector3.zero, 0.5f).onComplete =
            () => { LeanTween.scale(go, Vector3.one, 0.5f).onComplete = onFinished; };
    }
}