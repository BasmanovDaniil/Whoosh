using System;
using UnityEngine;

namespace ProceduralToolkit
{
    public static class DebugE
    {
        public static void DrawCircle(Vector3 center, Vector3 radius)
        {
            DrawCircle(center, radius, Color.white, 0, true);
        }

        public static void DrawCircle(Vector3 center, Vector3 radius, Color color)
        {
            DrawCircle(center, radius, color, 0, true);
        }

        public static void DrawCircle(Vector3 center, Vector3 radius, Color color, float duration)
        {
            DrawCircle(center, radius, color, duration, true);
        }

        public static void DrawCircle(Vector3 center, Vector3 radius, Color color, float duration, bool depthTest)
        {
            var polygon = Polygon.Circle(radius.magnitude) + center;
            DrawPolygon(polygon, color, duration, depthTest);
        }

        public static void DrawPolygon(Polygon polygon)
        {
            DrawPolygon(polygon, Color.white, 0f, true);
        }

        public static void DrawPolygon(Polygon polygon, Color color)
        {
            DrawPolygon(polygon, color, 0f, true);
        }

        public static void DrawPolygon(Polygon polygon, Color color, float duration)
        {
            DrawPolygon(polygon, color, duration, true);
        }

        public static void DrawPolygon(Polygon polygon, Color color, float duration, bool depthTest)
        {
            if (polygon.Count < 2)
            {
                return;
            }
            for (var i = 0; i < polygon.Count - 1; i++)
            {
                Debug.DrawLine((Vector3) polygon[i], (Vector3) polygon[i + 1], color, duration, depthTest);
            }
            Debug.DrawLine((Vector3) polygon[polygon.Count - 1], (Vector3) polygon[0], color, duration, depthTest);
        }

        public static void DrawIntPolygon(IntPolygon polygon)
        {
            DrawIntPolygon(polygon, Color.white, 0f, true);
        }

        public static void DrawIntPolygon(IntPolygon polygon, Color color)
        {
            DrawIntPolygon(polygon, color, 0f, true);
        }

        public static void DrawIntPolygon(IntPolygon polygon, Color color, float duration)
        {
            DrawIntPolygon(polygon, color, duration, true);
        }

        public static void DrawIntPolygon(IntPolygon polygon, Color color, float duration, bool depthTest)
        {
            if (polygon.Count < 2)
            {
                return;
            }
            for (var i = 0; i < polygon.Count - 1; i++)
            {
                Debug.DrawLine((Vector3) polygon[i], (Vector3) polygon[i + 1], color, duration, depthTest);
            }
            Debug.DrawLine((Vector3) polygon[polygon.Count - 1], (Vector3) polygon[0], color, duration, depthTest);
        }

        public static void DrawSegment(Segment2 segment)
        {
            DrawSegment(segment, Color.white, 0f, true);
        }

        public static void DrawSegment(Segment2 segment, Color color)
        {
            DrawSegment(segment, color, 0f, true);
        }

        public static void DrawSegment(Segment2 segment, Color color, float duration)
        {
            DrawSegment(segment, color, duration, true);
        }

        public static void DrawSegment(Segment2 segment, Color color, float duration, bool depthTest)
        {
            Debug.DrawLine((Vector3) segment.a, (Vector3) segment.b, color, duration, depthTest);
        }

        public static void DrawRect(Rect rect)
        {
            DrawRect(rect, Color.white, 0f, true);
        }

        public static void DrawRect(Rect rect, Color color)
        {
            DrawRect(rect, color, 0f, true);
        }

        public static void DrawRect(Rect rect, Color color, float duration)
        {
            DrawRect(rect, color, duration, true);
        }

        public static void DrawRect(Rect rect, Color color, float duration, bool depthTest)
        {
            var v1 = new Vector3(rect.xMin, rect.yMin);
            var v2 = new Vector3(rect.xMin, rect.yMax);
            var v3 = new Vector3(rect.xMax, rect.yMax);
            var v4 = new Vector3(rect.xMax, rect.yMin);
            Debug.DrawLine(v1, v2, color, duration, depthTest);
            Debug.DrawLine(v2, v3, color, duration, depthTest);
            Debug.DrawLine(v3, v4, color, duration, depthTest);
            Debug.DrawLine(v4, v1, color, duration, depthTest);
        }

        public static void DrawTest(Color color)
        {
            var shader = string.Format("Shader\"\"{{SubShader{{Pass{{Color({0},{1},{2},{3})}}}}}}", color.r, color.g,
                color.b, color.a);
            var material = new Material(shader);
            var mesh = MeshE.Triangle(Vector3.up, Vector3.right, Vector3.zero);
            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
        }

        public static void CaptureScreenshot()
        {
            Application.CaptureScreenshot(DateTime.Now.ToString("yyyy-M-d HH-mm-ss.ffff") + ".png");
        }
    }
}