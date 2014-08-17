using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform roof;
    public Renderer[] renderers;

    private bool attach = false;

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
        //Character.instance.transform.parent = roof;
        attach = true;
        Character.instance.car = this;
    }

    public void Detach()
    {
        //Character.instance.transform.parent = null;
        Character.instance.rigidbody.AddForce(rigidbody.velocity + Vector3.up*700);
        attach = false;
        Character.instance.car = null;
    }

    private void FixedUpdate()
    {
        if (attach)
        {
            Character.instance.rigidbody.AddForce(rigidbody.velocity);
            var position = Vector3.Lerp(Character.instance.rigidbody.position, roof.position + Vector3.up, 0.35f);
            Character.instance.rigidbody.MovePosition(position);
        }
    }
}