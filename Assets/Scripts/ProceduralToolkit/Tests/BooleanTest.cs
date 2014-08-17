using ProceduralToolkit;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BooleanTest : MonoBehaviour
{
    public int count = 1000;

    private void Start()
    {
    }

    private void Update()
    {
        for (int i = 0; i < count; i++)
        {
            GraphicsE.DrawCube(Random.onUnitSphere * 10, Random.rotation);
        }
    }

    private void OnPostRender()
    {
        GLE.BeginPixel(-50, 50, -50, 50);

        GL.Color(Color.red);
        GL.Begin(GL.TRIANGLES);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0, 20, 0);
        GL.Vertex3(20, 20, 0);
        GL.Vertex3(20, 0, 0);
        GL.Vertex3(0, 0, 0);
        GL.End();

        GLE.End();
    }
}