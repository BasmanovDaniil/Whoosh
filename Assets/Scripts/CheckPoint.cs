using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Action<Car> callback;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicles"))
        {
            var car = other.GetComponentInParent<Car>();
            callback(car);
            collider.enabled = false;
        }
    }
}