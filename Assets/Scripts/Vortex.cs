using UnityEngine;

public class Vortex : MonoBehaviour
{
    public Transform roof;

    private string playerTag = "Player";

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            var vector = roof.position - other.transform.position + Vector3.up;
            other.rigidbody.AddForce(vector*1000);
        }
    }
}