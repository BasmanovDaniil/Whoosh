using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int index;
    public Action<int, Car> callback;
    public bool disabled = false;
    private Car firstCar;

    private void OnTriggerEnter(Collider other)
    {
        if (!Mastermind.started)
        {
            return;
        }
        if (!disabled && firstCar == null && other.gameObject.layer == LayerMask.NameToLayer("Vehicles"))
        {
            firstCar = other.GetComponentInParent<Car>();
            Disable();
            callback(index, firstCar);
        }
    }

    public void Reset()
    {
        firstCar = null;
        gameObject.SetActive(true);
        collider.enabled = true;
        disabled = false;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        collider.enabled = false;
        disabled = true;
    }
}