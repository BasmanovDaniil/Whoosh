using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace ProceduralToolkit.Examples
{
    public class Boid
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 cohesion;
        public Vector3 separation;
    }

    [RequireComponent(typeof (MeshRenderer), typeof (MeshFilter))]
    public class Boids : MonoBehaviour
    {
        public Vector3 anchor = Vector3.zero;
        public float spawnSphere = 20;
        public float worldSphere = 25;
        public int maxSpeed = 15;
        public float cohesionRadius = 5;
        public int maxBoids = 5;
        public float separationDistance = 3;
        public float cohesionCoefficient = 1;
        public float alignmentCoefficient = 5;
        public float separationCoefficient = 10;
        public int simulationUpdate = 100;
        public int swarmCount = 1000;

        private List<Boid> boids = new List<Boid>();
        private Vector3[] templateVertices;
        private int templateVerticesLength;
        private int[] templateTriangles;
        private int templateTrianglesLength;
        private Vector3[] vertices;
        private int[] triangles;
        private Color32[] colors32;
        private Mesh mesh;
        private Color32 color32 = new Color32(55, 55, 55, 255);

        private int simulationCount;
        private List<Boid> neighbours = new List<Boid>();
        private int separationCount;
        private Vector3 alignment;
        private Boid other;
        private Vector3 toOther;
        private Vector3 distanceToAnchor;

        private void Awake()
        {
            GetComponent<MeshRenderer>().material = GraphicsE.diffuseVertexColor;

            var template = MeshE.TetrahedronFlat(0.3f);
            templateVertices = template.vertices;
            templateVerticesLength = template.vertices.Length;
            templateTriangles = template.triangles;
            templateTrianglesLength = template.triangles.Length;
            swarmCount = Mathf.Min(65000/templateVerticesLength, swarmCount);

            vertices = new Vector3[swarmCount*templateVerticesLength];
            triangles = new int[swarmCount*templateTrianglesLength];
            colors32 = new Color32[swarmCount * templateVerticesLength];
            for (var i = 0; i < swarmCount; i++)
            {
                var boid = new Boid
                {
                    position = Random.insideUnitSphere*spawnSphere,
                    rotation = Random.rotation,
                    velocity = Random.onUnitSphere*maxSpeed
                };
                boids.Add(boid);

                SetBoidVertices(boid, i);
                SetBoidTriangles(i);
                SetBoidColors(boid, i);
            }

            mesh = new Mesh
            {
                name = "Boids",
                vertices = vertices,
                triangles = triangles,
                colors32 = colors32
            };
            mesh.RecalculateNormals();
            mesh.MarkDynamic();
            GetComponent<MeshFilter>().mesh = mesh;

            StartCoroutine(SimulateCoroutine());
        }

        private IEnumerator SimulateCoroutine()
        {
            simulationCount = 0;
            while (true)
            {
                for (int i = 0; i < swarmCount; i++)
                {
                    simulationCount++;
                    if (simulationCount > simulationUpdate)
                    {
                        simulationCount = 0;
                        yield return null;
                    }
                    var boid = boids[i];
                    neighbours.Clear();
                    for (int j = 0; j < swarmCount; j++)
                    {
                        var b = boids[j];
                        if ((b.position - boid.position).sqrMagnitude < cohesionRadius)
                        {
                            neighbours.Add(b);
                            if (neighbours.Count == maxBoids)
                            {
                                break;
                            }
                        }
                    }

                    if (neighbours.Count < 2) continue;

                    boid.velocity = Vector3.zero;
                    boid.cohesion = Vector3.zero;
                    boid.separation = Vector3.zero;

                    separationCount = 0;
                    alignment = Vector3.zero;

                    for (var j = 0; j < neighbours.Count && j < maxBoids; j++)
                    {
                        other = neighbours[j];
                        boid.cohesion += other.position;
                        alignment += other.velocity;
                        toOther = other.position - boid.position;
                        if (toOther.sqrMagnitude > 0 && toOther.sqrMagnitude < separationDistance*separationDistance)
                        {
                            boid.separation += toOther/toOther.sqrMagnitude;
                            separationCount++;
                        }
                    }

                    boid.cohesion /= Mathf.Min(neighbours.Count, maxBoids);
                    boid.cohesion = Vector3.ClampMagnitude(boid.cohesion - boid.position, maxSpeed);
                    boid.cohesion *= cohesionCoefficient;
                    if (separationCount > 0)
                    {
                        boid.separation /= separationCount;
                        boid.separation = Vector3.ClampMagnitude(boid.separation, maxSpeed);
                        boid.separation *= separationCoefficient;
                    }
                    alignment /= Mathf.Min(neighbours.Count, maxBoids);
                    alignment = Vector3.ClampMagnitude(alignment, maxSpeed);
                    alignment *= alignmentCoefficient;

                    boid.velocity = Vector3.ClampMagnitude(boid.cohesion + boid.separation + alignment, maxSpeed);
                    if (boid.velocity == Vector3.zero)
                    {
                        boid.velocity = Random.onUnitSphere*maxSpeed;
                    }
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < swarmCount; i++)
            {
                var boid = boids[i];
                boid.rotation = Quaternion.LookRotation(boid.velocity);

                distanceToAnchor = anchor - boid.position;
                if (distanceToAnchor.sqrMagnitude > worldSphere*worldSphere)
                {
                    boid.velocity += distanceToAnchor/worldSphere;
                    boid.velocity = Vector3.ClampMagnitude(boid.velocity, maxSpeed);
                }
                boid.position += boid.velocity*Time.deltaTime;
                SetBoidVertices(boid, i);
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }

        private void SetBoidVertices(Boid boid, int index)
        {
            for (int i = 0; i < templateVerticesLength; i++)
            {
                vertices[index*templateVerticesLength + i] = boid.rotation*templateVertices[i] + boid.position;
            }
        }

        private void SetBoidColors(Boid boid, int index)
        {
            //var x = (boid.position.x + spawnSphere / 2) / spawnSphere;
            //var y = (boid.position.y + spawnSphere / 2) / spawnSphere;
            //var z = (boid.position.z + spawnSphere / 2) / spawnSphere;
            //var color = ColorE.HSVToRGB(Mathf.Clamp01((x + y + z) / 3), 1, 1);
            for (int i = 0; i < templateVerticesLength; i++)
            {
                colors32[index*templateVerticesLength + i] = color32;
            }
        }

        private void SetBoidTriangles(int index)
        {
            for (int i = 0; i < templateTrianglesLength; i++)
            {
                triangles[index*templateTrianglesLength + i] = templateTriangles[i] + index*templateVerticesLength;
            }
        }
    }
}