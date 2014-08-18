using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform roof;
    public Renderer[] renderers;

    private bool attach = false;
    private CarController carController;
    private float defaultMaxSpeed = 90;
    private float weightedMaxSpeed = 40;

    public void Initialize()
    {
        carController = GetComponent<CarController>();
        carController.MaxSpeed = defaultMaxSpeed;
    }

    public void SetColors(Color mainColor, Color specularColor)
    {
        var block = new MaterialPropertyBlock();
        block.AddColor("_Color", mainColor);
        block.AddColor("_SpecColor", specularColor);
        foreach (var r in renderers)
        {
            r.SetPropertyBlock(block);
        }
    }

    public void Attach()
    {
        attach = true;
        Character.instance.car = this;
        carController.MaxSpeed = weightedMaxSpeed;
    }

    public void Detach()
    {
        Character.instance.rigidbody.AddExplosionForce(20000, rigidbody.position, 10);
        attach = false;
        Character.instance.car = null;
        carController.MaxSpeed = defaultMaxSpeed;
    }

    private void FixedUpdate()
    {
        if (attach)
        {
            Character.instance.rigidbody.AddForce(rigidbody.velocity);
            var currentPosition = Character.instance.rigidbody.position;
            var targetPosition = roof.position + Vector3.up;

            var sqrDistance = (targetPosition - currentPosition).sqrMagnitude;
            if (sqrDistance > 16)
            {
                Character.instance.rigidbody.MovePosition(Vector3.MoveTowards(currentPosition, targetPosition, 1.5f));
            }
            else if (sqrDistance > 4)
            {
                Character.instance.rigidbody.MovePosition(Vector3.MoveTowards(currentPosition, targetPosition, 1f));
            }
            else
            {
                Character.instance.rigidbody.MovePosition(Vector3.Slerp(currentPosition, targetPosition, 0.35f));
            }
        }
    }

    public void Activate()
    {
        carController.Reset();
    }

    public void Deactivate()
    {
        carController.Immobilize();
    }
}