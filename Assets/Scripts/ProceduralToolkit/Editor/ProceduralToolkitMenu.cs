using UnityEditor;
using UnityEngine;

namespace ProceduralToolkit.Editor
{
    public class ProceduralToolkitMenu
    {
        private const string menuPath = "GameObject/ProceduralToolkit/";
        private const string create = "Create ";
        private const string tetrahedron = "Tetrahedron";
        private const string icosahedron = "Icosahedron";

        [MenuItem(menuPath + tetrahedron)]
        public static void Tetrahedron()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.name = tetrahedron;
            go.GetComponent<MeshFilter>().mesh = MeshE.Tetrahedron(1);
            Undo.RegisterCreatedObjectUndo(go, create + tetrahedron);
        }

        [MenuItem(menuPath + icosahedron)]
        public static void Icosahedron()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.name = icosahedron;
            go.GetComponent<MeshFilter>().mesh = MeshE.Icosahedron(1);
            Undo.RegisterCreatedObjectUndo(go, create + icosahedron);
        }

        [MenuItem("ProceduralToolkit/RecalculateTangents")]
        public static void RecalculateTangents()
        {
            var meshFilter = Selection.activeGameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.sharedMesh.RecalculateTangents();
            }
            else
            {
                var renderer = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMesh.RecalculateTangents();
                }
            }
        }

        [MenuItem("ProceduralToolkit/RecalculateUv")]
        public static void RecalculateUv()
        {
            var meshFilter = Selection.activeGameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.sharedMesh.RecalculateUv();
            }
            else
            {
                var renderer = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMesh.RecalculateUv();
                }
            }
        }
    }
}