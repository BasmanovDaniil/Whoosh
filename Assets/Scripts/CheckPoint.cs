using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int index;
    public Action<int, Car> callback;
    private Car firstCar;

    private void OnTriggerEnter(Collider other)
    {
        if (firstCar == null && other.gameObject.layer == LayerMask.NameToLayer("Vehicles"))
        {
            firstCar = other.GetComponentInParent<Car>();
            callback(index, firstCar);
        }
    }

    public void Reset()
    {
        firstCar = null;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}