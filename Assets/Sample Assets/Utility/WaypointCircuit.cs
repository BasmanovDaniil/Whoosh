using UnityEngine;

public class WaypointCircuit : MonoBehaviour
{
    public bool smoothRoute = true;
    private int numPoints;
    private Vector3[] points;
    private float[] distances;

    private float editorVisualisationSubsteps = 100;
    public float Length { get; private set; }

    public Vector3[] waypoints;

    //this being here will save GC allocs
    private int p0n;
    private int p1n;
    private int p2n;
    private int p3n;

    private float i;
    private Vector3 P0;
    private Vector3 P1;
    private Vector3 P2;
    private Vector3 P3;

    public void Initialize()
    {
        if (waypoints.Length > 1)
        {
            CachePositionsAndDistances();
        }
        numPoints = waypoints.Length;
    }

    public RoutePoint GetRoutePoint(float dist)
    {
        // position and direction
        Vector3 p1 = GetRoutePosition(dist);
        Vector3 p2 = GetRoutePosition(dist + 0.1f);
        Vector3 delta = p2 - p1;
        return new RoutePoint(p1, delta.normalized);
    }

    public Vector3 GetRoutePosition(float dist)
    {
        int point = 0;

        if (Length == 0)
        {
            Length = distances[distances.Length - 1];
        }

        dist = Mathf.Repeat(dist, Length);

        while (distances[point] < dist)
        {
            ++point;
        }


        // get nearest two points, ensuring points wrap-around start & end of circuit
        p1n = ((point - 1) + numPoints)%numPoints;
        p2n = point;

        // found point numbers, now find interpolation value between the two middle points

        i = Mathf.InverseLerp(distances[p1n], distances[p2n], dist);

        if (smoothRoute)
        {
            // smooth catmull-rom calculation between the two relevant points


            // get indices for the surrounding 2 points, because
            // four points are required by the catmull-rom function
            p0n = ((point - 2) + numPoints)%numPoints;
            p3n = (point + 1)%numPoints;

            // 2nd point may have been the 'last' point - a dupe of the first,
            // (to give a value of max track distance instead of zero)
            // but now it must be wrapped back to zero if that was the case.
            p2n = p2n%numPoints;

            P0 = points[p0n];
            P1 = points[p1n];
            P2 = points[p2n];
            P3 = points[p3n];

            return CatmullRom(P0, P1, P2, P3, i);
        }
        else
        {
            // simple linear lerp between the two points:

            p1n = ((point - 1) + numPoints)%numPoints;
            p2n = point;

            return Vector3.Lerp(points[p1n], points[p2n], i);
        }
    }


    private Vector3 CatmullRom(Vector3 _P0, Vector3 _P1, Vector3 _P2, Vector3 _P3, float _i)
    {
        // comments are no use here... it's the catmull-rom equation.
        // Un-magic this, lord vector!
        return 0.5f*
               ((2*_P1) + (-_P0 + _P2)*_i + (2*_P0 - 5*_P1 + 4*_P2 - _P3)*_i*_i + (-_P0 + 3*_P1 - 3*_P2 + _P3)*_i*_i*_i);
    }


    private void CachePositionsAndDistances()
    {
        // transfer the position of each point and distances between points to arrays for
        // speed of lookup at runtime
        points = new Vector3[waypoints.Length + 1];
        distances = new float[waypoints.Length + 1];

        float accumulateDistance = 0;
        for (int i = 0; i < points.Length; ++i)
        {
            var t1 = waypoints[(i)%waypoints.Length];
            var t2 = waypoints[(i + 1)%waypoints.Length];
            if (t1 != null && t2 != null)
            {
                Vector3 p1 = t1;
                Vector3 p2 = t2;
                points[i] = waypoints[i%waypoints.Length];
                distances[i] = accumulateDistance;
                accumulateDistance += (p1 - p2).magnitude;
            }
        }
    }


    private void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    private void DrawGizmos(bool selected)
    {
        if (waypoints.Length > 1)
        {
            numPoints = waypoints.Length;

            CachePositionsAndDistances();
            Length = distances[distances.Length - 1];

            Gizmos.color = selected ? Color.yellow : new Color(1, 1, 0, 0.5f);
            Vector3 prev = waypoints[0];
            if (smoothRoute)
            {
                for (float dist = 0; dist < Length; dist += Length/editorVisualisationSubsteps)
                {
                    Vector3 next = GetRoutePosition(dist + 1);
                    Gizmos.DrawLine(prev, next);
                    prev = next;
                }
                Gizmos.DrawLine(prev, waypoints[0]);
            }
            else
            {
                for (int n = 0; n < waypoints.Length; ++n)
                {
                    Vector3 next = waypoints[(n + 1)%waypoints.Length];
                    Gizmos.DrawLine(prev, next);
                    prev = next;
                }
            }
        }
        foreach (var waypoint in waypoints)
        {
            Gizmos.DrawWireSphere(waypoint, 0.3f);
        }
    }

    public struct RoutePoint
    {
        public Vector3 position;
        public Vector3 direction;

        public RoutePoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }
}