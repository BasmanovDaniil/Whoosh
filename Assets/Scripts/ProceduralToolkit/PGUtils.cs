using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralToolkit.Generic;
using UnityEngine;

namespace ProceduralToolkit
{
    public enum Orientation
    {
        CounterClockwise = -1,
        NonOrientable = 0,
        Clockwise = 1,
    }

    public class PGUtils
    {
        /// <summary>
        /// Triangulate simple polygon without holes.
        /// </summary>
        /// <param name="vertices">Vertices in clockwise winding order.</param>
        //public static int[] Triangulate(Vector2[] vertices)
        //{
        //    var polygon = new LinkedList<IndexedVector<Vector2>>();
        //    var ears = new LinkedList<IndexedVector<Vector2>>();
        //    var convexVertices = new List<IndexedVector<Vector2>>();
        //    var reflexVertices = new List<IndexedVector<Vector2>>();
        //    var triangles = new List<int>();

        //    for (var i = 0; i < vertices.Length; i++)
        //    {
        //        polygon.AddLast(new IndexedVector<Vector2>(vertices[i], i));
        //    }

        //    var currentNode = polygon.First;
        //    for (var i = 0; i < polygon.Count; i++)
        //    {
        //        if (IsConvex(currentNode))
        //        {
        //            convexVertices.Add(currentNode.Value);
        //        }
        //        else
        //        {
        //            reflexVertices.Add(currentNode.Value);
        //        }
        //        currentNode = currentNode.NextOrFirst();
        //    }

        //    for (var i = 0; i < convexVertices.Count; i++)
        //    {
        //        var vertex = polygon.Find(convexVertices[i]);
        //        if (IsEar(reflexVertices, vertex))
        //        {
        //            ears.AddLast(convexVertices[i]);
        //        }
        //    }

        //    var count = 1f;
        //    while (polygon.Count > 3 && ears.Count > 0)
        //    {
        //        foreach (var convexVertex in convexVertices)
        //        {
        //            Debug.DrawLine(convexVertex.vector, Vector3.zero, Color.green, 100);
        //        }
        //        foreach (var reflexVertex in reflexVertices)
        //        {
        //            Debug.DrawLine(reflexVertex.vector, Vector3.zero, Color.red, 100);
        //        }
        //        foreach (var vector in ears)
        //        {
        //            Debug.DrawLine(vector.vector, Vector3.zero, Color.yellow, 100);
        //        }
        //        var earNode = ears.First;
        //        var previousNode = polygon.Find(earNode.Value).PreviousOrLast();
        //        var nextNode = polygon.Find(earNode.Value).NextOrFirst();
        //        var ear = earNode.Value;
        //        var previous = previousNode.Value;
        //        var next = nextNode.Value;

        //        //Debug.DrawLine(previous.vector, ear.vector, new Color(1, count, count), 100);
        //        //Debug.DrawLine(ear.vector, next.vector, new Color(1, count, count), 100);
        //        count -= 0.1f;

        //        triangles.Add(previous.index);
        //        triangles.Add(ear.index);
        //        triangles.Add(next.index);

        //        ears.RemoveFirst();
        //        foreach (var indexedVector in polygon)
        //        {
        //            if (indexedVector.index == ear.index)
        //            {
        //                polygon.Remove(indexedVector);
        //                break;
        //            }
        //        }

        //        if (reflexVertices.Contains(previous))
        //        {
        //            if (IsConvex(previousNode))
        //            {
        //                reflexVertices.Remove(previous);
        //                convexVertices.Add(previous);
        //            }
        //        }
        //        if (convexVertices.Contains(previous))
        //        {
        //            var inEars = ears.Contains(previous);
        //            var isEar = IsEar(reflexVertices, previousNode);
        //            if (inEars && !isEar) ears.Remove(previous);
        //            else if (!inEars && isEar) ears.AddFirst(previous);
        //        }
        //        if (reflexVertices.Contains(next))
        //        {
        //            if (IsConvex(previousNode))
        //            {
        //                reflexVertices.Remove(next);
        //                convexVertices.Add(next);
        //            }
        //        }
        //        if (convexVertices.Contains(next))
        //        {
        //            var inEars = ears.Contains(next);
        //            var isEar = IsEar(reflexVertices, nextNode);
        //            if (inEars && !isEar) ears.Remove(next);
        //            else if (!inEars && isEar) ears.AddFirst(next);
        //        }

        //        //Debug.Log("Reflex: " + reflexVertices.Count);
        //        //Debug.Log("Convex: " + convexVertices.Count);
        //        //Debug.Log("Ear: " + ears.Count);
        //    }

        //    if (polygon.Count == 3)
        //    {
        //        triangles.Add(polygon.First.Value.index);
        //        triangles.Add(polygon.First.Next.Value.index);
        //        triangles.Add(polygon.First.Next.Next.Value.index);
        //    }

        //    return triangles.ToArray();
        //}
        /// <summary>
        /// Triangulate simple polygon without holes.
        /// </summary>
        /// <param name="polygonVertices">Polygon vertices in clockwise winding order.</param>
        public static int[] Triangulate(Vector2[] polygonVertices)
        {
            var triangles = new List<int>();
            var vertices =
                new CircularList<Indexed<Vector2>>(
                    polygonVertices.Select((t, i) => new Indexed<Vector2>(t, i)));

            while (vertices.Count > 0)
            {
                for (var i = 0; i < vertices.Count; i++)
                {
                    var previous = vertices[i - 1];
                    var middle = vertices[i];
                    var next = vertices[i + 1];

                    var isEar = true;
                    for (var j = 0; j < vertices.Count; j++)
                    {
                        if (IsInTriangle(previous.value, middle.value, next.value, vertices[j].value))
                        {
                            isEar = false;
                            break;
                        }
                    }
                    if (isEar)
                    {
                        triangles.Add(previous.index);
                        triangles.Add(middle.index);
                        triangles.Add(next.index);
                        vertices.Remove(middle);
                    }
                }
            }

            return triangles.ToArray();
        }

        public static int[] Triangulate(List<Vector2> vertices)
        {
            return Triangulate(vertices.ToArray());
        }

        private static bool IsConvex(Vector2 previousVertex, Vector2 middleVertex, Vector2 nextVertex)
        {
            return Vector3.Cross(previousVertex - middleVertex, nextVertex - middleVertex).z > 0;
        }

        private static bool IsConvex(LinkedListNode<Indexed<Vector2>> node)
        {
            return IsConvex(node.PreviousOrLast().Value.value, node.Value.value, node.NextOrFirst().Value.value);
        }

        private static bool IsEar(List<Indexed<Vector2>> reflexVertices,
            Indexed<Vector2> previousVertex,
            Indexed<Vector2> middleVertex,
            Indexed<Vector2> nextVertex)
        {
            foreach (var reflexVertex in reflexVertices)
            {
                if (reflexVertex == previousVertex || reflexVertex == middleVertex || reflexVertex == nextVertex)
                {
                    continue;
                }
                if (IsInTriangle(previousVertex.value, middleVertex.value, nextVertex.value, reflexVertex.value))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsEar(List<Indexed<Vector2>> reflexVertices,
            LinkedListNode<Indexed<Vector2>> node)
        {
            return IsEar(reflexVertices, node.PreviousOrLast().Value, node.Value, node.NextOrFirst().Value);
        }

        public static bool IsInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 point)
        {
            if (a == point || b == point || c == point)
            {
                return true;
            }

            var ab = PerpDot(b - a, point - a);
            var bc = PerpDot(c - b, point - b);
            var ca = PerpDot(a - c, point - c);
            return (ab <= 0 && bc <= 0 && ca <= 0) || (-ab <= 0 && -bc <= 0 && -ca <= 0);
        }

        /// <summary>
        /// Returns perp of vector
        /// </summary>
        /// <remarks>
        /// Hill, F. S. Jr. "The Pleasures of 'Perp Dot' Products."
        /// Ch. II.5 in Graphics Gems IV (Ed. P. S. Heckbert). San Diego: Academic Press, pp. 138-148, 1994
        /// </remarks>
        public static Vertex2 Perp(Vertex2 vector)
        {
            return new Vertex2(-vector.y, vector.x);
        }

        public static IntVertex2 Perp(IntVertex2 vector)
        {
            return new IntVertex2(-vector.y, vector.x);
        }

        /// <summary>
        /// Returns perp dot product of vectors
        /// </summary>
        /// <remarks>
        /// Hill, F. S. Jr. "The Pleasures of 'Perp Dot' Products."
        /// Ch. II.5 in Graphics Gems IV (Ed. P. S. Heckbert). San Diego: Academic Press, pp. 138-148, 1994
        /// </remarks>
        public static float PerpDot(Vector2 a, Vector2 b)
        {
            return a.x*b.y - a.y*b.x;
        }

        public static float PerpDot(Vertex2 a, Vertex2 b) 
        {
            return a.x * b.y - a.y * b.x;
        }

        public static int PerpDot(IntVertex2 a, IntVertex2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        public static bool Approximately(Vector2 a, Vector2 b, float epsilon)
        {
            return Mathf.Abs(a.x - b.x) < epsilon && Mathf.Abs(a.y - b.y) < epsilon;
        }

        public static void Swap<T>(ref T left, ref T right)
        {
            T temp = left;
            left = right;
            right = temp;
        }
        
        /// <summary>
        /// More than 0 - left, less than 0 - right, equals 0 - on line
        /// </summary>
        public static int LocatePointOnLine(IntVertex2 line1, IntVertex2 line2, IntVertex2 point)
        {
            return (line2.x - line1.x)*(point.y - line1.y) - (point.x - line1.x)*(line2.y - line1.y);
        }

        /// <summary>
        /// More than 0 - left, less than 0 - right, equals 0 - on line
        /// </summary>
        public static float LocatePointOnLine(Vertex2 line1, Vertex2 line2, Vertex2 point)
        {
            return (line2.x - line1.x)*(point.y - line1.y) - (point.x - line1.x)*(line2.y - line1.y);
        }

        /// <summary>
        /// Ramer-Douglas-Peucker algorithm
        /// </summary>
        public static void RamerDouglasPeucker()
        {
            
        }


        //public List<Vector2> properRDP(List<Vector2> points,float epsilon){
        //    Vector2 firstPoint=points[0];
        //    Vector2 lastPoint=points[points.Count-1];
        //    if (points.Count<3){
        //        return points;
        //    }
        //    int index=-1;
        //    float dist=0;
        //    for (var i=1;i<points.Count-1;i++){
        //        var cDist=findPerpendicularDistance(points[i],firstPoint,lastPoint);
        //        if (cDist>dist){
        //            dist=cDist;
        //            index=i;
        //        }
        //    }
        //    if (dist>epsilon){
        //        // iterate
        //        var l1=points.GetRange(0, index+1);
        //        var l2=points.GetRange(index, 1);
        //        var r1=properRDP(l1,epsilon);
        //        var r2=properRDP(l2,epsilon);
        //        // concat r2 to r1 minus the end/startpoint that will be the same
        //        var rs=r1.GetRange(0,r1.Count-1).Add(r2);
        //        return rs;
        //    }else{
        //        return [firstPoint,lastPoint];
        //    }
        //}
    
    
        //public float findPerpendicularDistance(Vector2 p, Vector2 p1, Vector2 p2)
        //{
        //    // if start and end point are on the same x the distance is the difference in X.
        //    float result;
        //    if (p1[0] == p2[0])
        //    {
        //        result = Math.Abs(p[0] - p1[0]);
        //    }
        //    else
        //    {
        //        float slope = (p2[1] - p1[1])/(p2[0] - p1[0]);
        //        float intercept = p1[1] - (slope*p1[0]);
        //        result = Mathf.Abs(slope*p[0] - p[1] + intercept)/Mathf.Sqrt(Mathf.Pow(slope, 2) + 1);
        //    }
        //    return result;
        //}
    }

    internal static class CircularLinkedList
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }
    }
}