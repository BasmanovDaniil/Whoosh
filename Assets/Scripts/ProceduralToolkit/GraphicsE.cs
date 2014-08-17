using UnityEngine;

namespace ProceduralToolkit
{
    public static class GraphicsE
    {
        public static readonly Material diffuse = new Material(Shader.Find("Diffuse"));
        public static readonly Material unlitColor = new Material(Shader.Find("Unlit/Color"));
        public static readonly Material diffuseVertexColor = new Material(Shader.Find("Diffuse/Vertex Color"));

        private static readonly MaterialPropertyBlock materialProperty = new MaterialPropertyBlock();
        private static readonly Mesh cube = MeshE.Cube(1);

        public static void DrawMesh(Mesh mesh, Vector3 position)
        {
            DrawMesh(mesh, position, Quaternion.identity);
        }

        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation)
        {
            Graphics.DrawMesh(mesh, position, rotation, unlitColor, 0);
        }

        public static void DrawMesh(Mesh mesh, Color color, Vector3 position)
        {
            DrawMesh(mesh, color, position, Quaternion.identity);
        }

        public static void DrawMesh(Mesh mesh, Color color, Vector3 position, Quaternion rotation)
        {
            materialProperty.Clear();
            materialProperty.AddColor("_Color", color);
            Graphics.DrawMesh(mesh, position, rotation, unlitColor, 0, null, 0, materialProperty);
        }

        public static void DrawDiffuseMesh(Mesh mesh, Vector3 position)
        {
            DrawDiffuseMesh(mesh, position, Quaternion.identity);
        }

        public static void DrawDiffuseMesh(Mesh mesh, Vector3 position, Quaternion rotation)
        {
            Graphics.DrawMesh(mesh, position, rotation, diffuse, 0);
        }

        public static void DrawDiffuseMesh(Mesh mesh, Color color, Vector3 position)
        {
            DrawDiffuseMesh(mesh, color, position, Quaternion.identity);
        }

        public static void DrawDiffuseMesh(Mesh mesh, Color color, Vector3 position, Quaternion rotation)
        {
            materialProperty.Clear();
            materialProperty.AddColor("_Color", color);
            Graphics.DrawMesh(mesh, position, rotation, diffuse, 0, null, 0, materialProperty);
        }

        public static void DrawCube(Vector3 position)
        {
            DrawCube(position, Quaternion.identity);
        }

        public static void DrawCube(Vector3 position, Quaternion rotation)
        {
            Graphics.DrawMesh(cube, position, rotation, unlitColor, 0);
        }

        public static void DrawCube(Color color, Vector3 position)
        {
            DrawCube(color, position, Quaternion.identity);
        }

        public static void DrawCube(Color color, Vector3 position, Quaternion rotation)
        {
            materialProperty.Clear();
            materialProperty.AddColor("_Color", color);
            Graphics.DrawMesh(cube, position, rotation, unlitColor, 0, null, 0, materialProperty);
        }

        public static void DrawDiffuseCube(Vector3 position)
        {
            DrawDiffuseCube(position, Quaternion.identity);
        }

        public static void DrawDiffuseCube(Vector3 position, Quaternion rotation)
        {
            Graphics.DrawMesh(cube, position, rotation, diffuse, 0);
        }

        public static void DrawDiffuseCube(Color color, Vector3 position)
        {
            DrawDiffuseCube(color, position, Quaternion.identity);
        }

        public static void DrawDiffuseCube(Color color, Vector3 position, Quaternion rotation)
        {
            materialProperty.Clear();
            materialProperty.AddColor("_Color", color);
            Graphics.DrawMesh(cube, position, rotation, diffuse, 0, null, 0, materialProperty);
        }
    }
}