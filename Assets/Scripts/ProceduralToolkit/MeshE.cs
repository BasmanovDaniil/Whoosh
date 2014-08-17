using System.Collections.Generic;
using UnityEngine;

namespace ProceduralToolkit
{
    public static class MeshE
    {
        #region Primitives

        public enum Shape
        {
            Triangle,
            Quad,
            Plane,
            Tetrahedron,
            Cube,
            Octahedron,
            Icosahedron,
        };

        public static Mesh Triangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            var normal = Vector3.Cross((vertex1 - vertex0), (vertex2 - vertex0)).normalized;
            return new Mesh
            {
                vertices = new[] {vertex0, vertex1, vertex2},
                normals = new[] {normal, normal, normal},
                uv = new[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)},
                triangles = new[] {0, 1, 2}
            };
        }

        public static Mesh Quad(Vector3 origin, Vector3 width, Vector3 length)
        {
            var normal = Vector3.Cross(length, width).normalized;
            var mesh = new Mesh
            {
                vertices = new[] {origin, origin + length, origin + length + width, origin + width},
                normals = new[] {normal, normal, normal, normal},
                uv = new[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)},
                triangles = new[]
                {
                    0, 1, 2,
                    0, 2, 3
                }
            };
            return mesh;
        }

        public static Mesh Quad(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            var normal = Vector3.Cross((vertex1 - vertex0), (vertex2 - vertex0)).normalized;
            var mesh = new Mesh
            {
                vertices = new[] {vertex0, vertex1, vertex2, vertex3},
                normals = new[] {normal, normal, normal, normal},
                uv = new[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)},
                triangles = new[]
                {
                    0, 1, 2,
                    0, 2, 3
                }
            };
            return mesh;
        }

        public static Mesh Plane(Vector3 origin, Vector3 width, Vector3 length, int widthCount, int lengthCount)
        {
            var combine = new CombineInstance[widthCount*lengthCount];

            var i = 0;
            for (var x = 0; x < widthCount; x++)
            {
                for (var y = 0; y < lengthCount; y++)
                {
                    combine[i].mesh = Quad(origin + width*x + length*y, width, length);
                    i++;
                }
            }

            var mesh = new Mesh();
            mesh.CombineMeshes(combine, true, false);
            return mesh;
        }

        public static Mesh Tetrahedron(float radius)
        {
            var tetrahedralAngle = Mathf.PI*109.4712f/180;
            var segmentAngle = Mathf.PI*2/3;
            var currentAngle = 0f;
            var sinTetrahedralAngle = Mathf.Sin(tetrahedralAngle);
            var cosTetrahedralAngle = Mathf.Cos(tetrahedralAngle);

            var vertices = new Vector3[4];
            vertices[0] = new Vector3(0, radius, 0);
            for (var i = 1; i < 4; i++)
            {
                vertices[i] = new Vector3(radius*Mathf.Sin(currentAngle)*sinTetrahedralAngle,
                    radius*cosTetrahedralAngle,
                    radius*Mathf.Cos(currentAngle)*sinTetrahedralAngle);
                currentAngle += segmentAngle;
            }
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new[]
                {
                    0, 1, 2,
                    1, 3, 2,
                    0, 2, 3,
                    0, 3, 1
                }
            };
            mesh.RecalculateNormals();
            mesh.name = "Tetrahedron";
            return mesh;
        }

        public static Mesh TetrahedronFlat(float radius)
        {
            var tetrahedralAngle = Mathf.PI*109.4712f/180;
            var segmentAngle = Mathf.PI*2/3;
            var currentAngle = 0f;
            var sinTetrahedralAngle = Mathf.Sin(tetrahedralAngle);
            var cosTetrahedralAngle = Mathf.Cos(tetrahedralAngle);

            var vertices = new Vector3[12];
            vertices[0] = new Vector3(0, radius, 0);
            vertices[1] = vertices[0];
            vertices[2] = vertices[0];
            for (var i = 3; i < 12; i += 3)
            {
                vertices[i] = new Vector3(radius*Mathf.Sin(currentAngle)*sinTetrahedralAngle,
                    radius*cosTetrahedralAngle,
                    radius*Mathf.Cos(currentAngle)*sinTetrahedralAngle);
                vertices[i + 1] = vertices[i];
                vertices[i + 2] = vertices[i];
                currentAngle += segmentAngle;
            }
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new[]
                {
                    0, 9, 3,
                    1, 6, 4,
                    5, 7, 10,
                    2, 11, 8
                }
            };
            mesh.RecalculateNormals();
            return mesh;
        }

        public static Mesh BaselessPyramid(Vector3 baseCenter, Vector3 apex, float radius, int segments,
            bool inverted = false)
        {
            var segmentAngle = Mathf.PI*2/segments;
            var currentAngle = 0f;

            var v = new Vector3[segments + 1];
            v[0] = apex;
            for (var i = 1; i <= segments; i++)
            {
                v[i] = new Vector3(radius*Mathf.Sin(currentAngle), 0,
                    radius*Mathf.Cos(currentAngle)) + baseCenter;
                if (inverted) currentAngle -= segmentAngle;
                else currentAngle += segmentAngle;
            }

            var combine = new CombineInstance[segments];
            for (var i = 0; i < segments - 1; i++)
            {
                combine[i].mesh = Triangle(v[0], v[i + 1], v[i + 2]);
            }
            combine[combine.Length - 1].mesh = Triangle(v[0], v[v.Length - 1], v[1]);

            var mesh = new Mesh();
            mesh.CombineMeshes(combine, true, false);
            return mesh;
        }

        public static Mesh BaselessPyramid(float radius, int segments, float heignt, bool inverted = false)
        {
            if (inverted) return BaselessPyramid(Vector3.zero, Vector3.down*heignt, radius, segments, true);
            return BaselessPyramid(Vector3.zero, Vector3.up*heignt, radius, segments);
        }

        public static Mesh TriangleFan(Vector3 center, float radius, int segments, bool inverted = false)
        {
            var points = Polygon.Circle(radius, segments);
            if (inverted)
            {
                points.Reverse();
            }

            var vertices = new Vector3[points.Count];
            var uv = new Vector2[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = center + new Vector3(points[i].x, points[i].y);
                uv[i] = new Vector2(points[i].x/radius, points[i].y/radius);
            }

            var triangles = new List<int>();
            for (int i = 1; i < vertices.Length; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i - 1);
            }

            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles.ToArray(),
                uv = uv
            };
            mesh.RecalculateNormals();

            return mesh;
        }

        public static Mesh Pyramid(float radius, int segments, float heignt, bool grounded = false)
        {
            var combine = new CombineInstance[2];
            combine[0].mesh = BaselessPyramid(radius, segments, heignt);
            combine[1].mesh = TriangleFan(Vector3.zero, radius, segments, true);

            var mesh = new Mesh();
            mesh.CombineMeshes(combine, true, false);
            return mesh;
        }

        public static Mesh BiPyramid(float radius, int segments, float heignt)
        {
            var combine = new CombineInstance[2];
            combine[0].mesh = BaselessPyramid(radius, segments, heignt);
            combine[1].mesh = BaselessPyramid(radius, segments, heignt, true);

            var mesh = new Mesh();
            mesh.CombineMeshes(combine, true, false);
            return mesh;
        }

        public static Mesh Cube(float edge)
        {
            var mesh = Parallelepiped(Vector3.right*edge, Vector3.forward*edge, Vector3.up*edge);
            mesh.name = "Cube";
            return mesh;
        }

        public static Mesh Parallelepiped(Vector3 width, Vector3 length, Vector3 height)
        {
            var corner0 = -width/2 - length/2 - height/2;
            var corner1 = width/2 + length/2 + height/2;

            var combine = new CombineInstance[6];
            combine[0].mesh = Quad(corner0, length, width);
            combine[1].mesh = Quad(corner0, width, height);
            combine[2].mesh = Quad(corner0, height, length);
            combine[3].mesh = Quad(corner1, -width, -length);
            combine[4].mesh = Quad(corner1, -height, -width);
            combine[5].mesh = Quad(corner1, -length, -height);

            var mesh = new Mesh();
            mesh.CombineMeshes(combine, true, false);
            return mesh;
        }

        //public static Mesh Octahedron(float radius)
        //{
        //    var v = new Vector3[6];
        //    v[0] = new Vector3(0, -radius, 0);
        //    v[1] = new Vector3(-radius, 0, 0);
        //    v[2] = new Vector3(0, 0, -radius);
        //    v[3] = new Vector3(+radius, 0, 0);
        //    v[4] = new Vector3(0, 0, +radius);
        //    v[5] = new Vector3(0, radius, 0);

        //    var mesh = new Mesh
        //    {
        //        vertices = v,
        //        triangles = new[] { 0, 1, 2,
        //                            0, 2, 3,
        //                            0, 3, 4,
        //                            0, 4, 1,
        //                            5, 2, 1,
        //                            5, 3, 2,
        //                            5, 4, 3,
        //                            5, 1, 4}
        //    };
        //    mesh.RecalculateNormals();
        //    return mesh;
        //}

        //public static Mesh Octahedron(float radius)
        //{
        //    var v0 = new Vector3(0, -radius, 0);
        //    var v1 = new Vector3(-radius, 0, 0);
        //    var v2 = new Vector3(0, 0, -radius);
        //    var v3 = new Vector3(+radius, 0, 0);
        //    var v4 = new Vector3(0, 0, +radius);
        //    var v5 = new Vector3(0, radius, 0);

        //    var combine = new CombineInstance[8];
        //    combine[0].mesh = Triangle(v0, v1, v2);
        //    combine[1].mesh = Triangle(v0, v2, v3);
        //    combine[2].mesh = Triangle(v0, v3, v4);
        //    combine[3].mesh = Triangle(v0, v4, v1);
        //    combine[4].mesh = Triangle(v5, v2, v1);
        //    combine[5].mesh = Triangle(v5, v3, v2);
        //    combine[6].mesh = Triangle(v5, v4, v3);
        //    combine[7].mesh = Triangle(v5, v1, v4);

        //    var mesh = new Mesh();
        //    mesh.CombineMeshes(combine, true, false);
        //    return mesh;
        //}

        public static Mesh Octahedron(float radius)
        {
            return BiPyramid(1, 4, 1);
        }

        //public static Mesh Icosahedron(float radius)
        //{
        //    float a = 1;
        //    float b = (1 + Mathf.Sqrt(5)) / 2;
        //    float c = 0;
        //    float scale = radius / Mathf.Sqrt(a * a + b * b + c * c);
        //    a = a * scale;
        //    b = b * scale;
        //    c = c * scale;

        //    var v0 = new Vector3(-a, b, c);
        //    var v1 = new Vector3(a, b, c);
        //    var v2 = new Vector3(-a, -b, c);
        //    var v3 = new Vector3(a, -b, c);
        //    var v4 = new Vector3(c, -a, b);
        //    var v5 = new Vector3(c, a, b);
        //    var v6 = new Vector3(c, -a, -b);
        //    var v7 = new Vector3(c, a, -b);
        //    var v8 = new Vector3(b, c, -a);
        //    var v9 = new Vector3(b, c, a);
        //    var v10 = new Vector3(-b, c, -a);
        //    var v11 = new Vector3(-b, c, a);

        //    var combine = new CombineInstance[20];
        //    combine[0].mesh = Triangle(v0, v11, v5);
        //    combine[1].mesh = Triangle(v0, v5, v1);
        //    combine[2].mesh = Triangle(v0, v1, v7);
        //    combine[3].mesh = Triangle(v0, v7, v10);
        //    combine[4].mesh = Triangle(v0, v10, v11);
        //    combine[5].mesh = Triangle(v1, v5, v9);
        //    combine[6].mesh = Triangle(v5, v11, v4);
        //    combine[7].mesh = Triangle(v11, v10, v2);
        //    combine[8].mesh = Triangle(v10, v7, v6);
        //    combine[9].mesh = Triangle(v7, v1, v8);
        //    combine[10].mesh = Triangle(v3, v9, v4);
        //    combine[11].mesh = Triangle(v3, v4, v2);
        //    combine[12].mesh = Triangle(v3, v2, v6);
        //    combine[13].mesh = Triangle(v3, v6, v8);
        //    combine[14].mesh = Triangle(v3, v8, v9);
        //    combine[15].mesh = Triangle(v4, v9, v5);
        //    combine[16].mesh = Triangle(v2, v4, v11);
        //    combine[17].mesh = Triangle(v6, v2, v10);
        //    combine[18].mesh = Triangle(v8, v6, v7);
        //    combine[19].mesh = Triangle(v9, v8, v1);

        //    var mesh = new Mesh();
        //    mesh.CombineMeshes(combine, true, false);
        //    return mesh;
        //}

        public static Mesh Icosahedron(float radius)
        {
            var magicAngle = Mathf.PI*26.565f/180;
            var segmentAngle = Mathf.PI*72/180;
            var currentAngle = 0f;

            var v = new Vector3[12];
            v[0] = new Vector3(0, radius, 0);
            v[11] = new Vector3(0, -radius, 0);

            for (var i = 1; i < 6; i++)
            {
                v[i] = new Vector3(radius*Mathf.Sin(currentAngle)*Mathf.Cos(magicAngle),
                    radius*Mathf.Sin(magicAngle),
                    radius*Mathf.Cos(currentAngle)*Mathf.Cos(magicAngle));
                currentAngle += segmentAngle;
            }
            currentAngle = Mathf.PI*36/180;
            for (var i = 6; i < 11; i++)
            {
                v[i] = new Vector3(radius*Mathf.Sin(currentAngle)*Mathf.Cos(-magicAngle),
                    radius*Mathf.Sin(-magicAngle),
                    radius*Mathf.Cos(currentAngle)*Mathf.Cos(-magicAngle));
                currentAngle += segmentAngle;
            }

            var combine = new CombineInstance[20];
            combine[0].mesh = Triangle(v[0], v[1], v[2]);
            combine[1].mesh = Triangle(v[0], v[2], v[3]);
            combine[2].mesh = Triangle(v[0], v[3], v[4]);
            combine[3].mesh = Triangle(v[0], v[4], v[5]);
            combine[4].mesh = Triangle(v[0], v[5], v[1]);

            combine[5].mesh = Triangle(v[11], v[7], v[6]);
            combine[6].mesh = Triangle(v[11], v[8], v[7]);
            combine[7].mesh = Triangle(v[11], v[9], v[8]);
            combine[8].mesh = Triangle(v[11], v[10], v[9]);
            combine[9].mesh = Triangle(v[11], v[6], v[10]);

            combine[10].mesh = Triangle(v[2], v[1], v[6]);
            combine[11].mesh = Triangle(v[3], v[2], v[7]);
            combine[12].mesh = Triangle(v[4], v[3], v[8]);
            combine[13].mesh = Triangle(v[5], v[4], v[9]);
            combine[14].mesh = Triangle(v[1], v[5], v[10]);

            combine[15].mesh = Triangle(v[6], v[7], v[2]);
            combine[16].mesh = Triangle(v[7], v[8], v[3]);
            combine[17].mesh = Triangle(v[8], v[9], v[4]);
            combine[18].mesh = Triangle(v[9], v[10], v[5]);
            combine[19].mesh = Triangle(v[10], v[6], v[1]);

            var mesh = new Mesh();
            mesh.CombineMeshes(combine, true, false);
            mesh.name = "Icosahedron";
            return mesh;
        }

        //public static Mesh Cylinder(Vector3 origin, Vector3 width, Vector3 length, int widthCount, int lengthCount)
        //{
        //    var combine = new CombineInstance[widthCount * lengthCount];

        //    var i = 0;
        //    for (var x = 0; x < widthCount; x++)
        //    {
        //        for (var y = 0; y < widthCount; y++)
        //        {
        //            combine[i].mesh = Quad(origin + width * x + length * y, width, length);
        //            i++;
        //        }
        //    }

        //    var mesh = new Mesh();
        //    mesh.CombineMeshes(combine, true, false);
        //    return mesh;
        //}

        //public static Mesh Ring(int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles)
        //{
        //    var vertices = new List<Vector3>();
        //    var uv = new List<Vector2>();
        //    var normals = new List<Vector3>();
        //    var triangles = new List<int>();

        //    float angleInc = (Mathf.PI * 2.0f) / segmentCount;

        //    for (int i = 0; i <= segmentCount; i++)
        //    {
        //        float angle = angleInc * i;

        //        var unitPosition = Vector3.zero;
        //        unitPosition.x = Mathf.Cos(angle);
        //        unitPosition.z = Mathf.Sin(angle);

        //        vertices.Add(centre + unitPosition * radius);
        //        normals.Add(unitPosition);
        //        uv.Add(new Vector2((float)i / segmentCount, v));

        //        if (i > 0 && buildTriangles)
        //        {
        //            int baseIndex = vertices.Count - 1;

        //            int vertsPerRow = segmentCount + 1;

        //            int index0 = baseIndex;
        //            int index1 = baseIndex - 1;
        //            int index2 = baseIndex - vertsPerRow;
        //            int index3 = baseIndex - vertsPerRow - 1;

        //            triangles.AddRange(new [] {index0, index2, index1});
        //            triangles.AddRange(new [] {index2, index3, index1});
        //        }
        //    }
        //    var mesh = new Mesh
        //        {
        //            vertices = vertices.ToArray(),
        //            normals = normals.ToArray(),
        //            uv = uv.ToArray(),
        //            triangles = triangles.ToArray()
        //        };

        //    return mesh;
        //}

        #endregion Primitives

        public static Mesh Spherify(Mesh mesh, float radius = 1)
        {
            var vertices = mesh.vertices;
            var normals = mesh.normals;
            var center = Vector3.zero;
            for (var i = 0; i < vertices.Length; i++)
            {
                center += vertices[i];
            }
            center /= vertices.Length;

            for (var i = 0; i < vertices.Length; i++)
            {
                normals[i] = (vertices[i] - center).normalized;
                vertices[i] = normals[i]*radius;
            }
            mesh.vertices = vertices;
            mesh.normals = normals;
            return mesh;
        }

        public static Vector3 MiddleVector(Vector3 vertex0, Vector3 vertex1)
        {
            return (vertex0 + vertex1)*0.5f;
        }

        public static Mesh Subdivide(Mesh mesh)
        {
            var oldTriangles = mesh.triangles;
            var vertices = new List<Vector3>(mesh.vertices);
            var normals = new List<Vector3>(mesh.normals);
            var uv = new List<Vector2>(mesh.uv);
            var triangles = new List<int>();

            for (var i = 0; i < oldTriangles.Length - 2; i += 3)
            {
                var v0 = oldTriangles[i];
                var v1 = oldTriangles[i + 1];
                var v2 = oldTriangles[i + 2];

                vertices.Add(MiddleVector(vertices[v0], vertices[v1]));
                vertices.Add(MiddleVector(vertices[v1], vertices[v2]));
                vertices.Add(MiddleVector(vertices[v2], vertices[v0]));

                normals.Add(MiddleVector(normals[v0], normals[v1]));
                normals.Add(MiddleVector(normals[v1], normals[v2]));
                normals.Add(MiddleVector(normals[v2], normals[v0]));

                uv.Add(MiddleVector(uv[v0], uv[v1]));
                uv.Add(MiddleVector(uv[v1], uv[v2]));
                uv.Add(MiddleVector(uv[v2], uv[v0]));

                var v01 = vertices.Count - 3;
                var v12 = vertices.Count - 2;
                var v20 = vertices.Count - 1;

                triangles.AddRange(new[] {v0, v01, v20});
                triangles.AddRange(new[] {v1, v12, v01});
                triangles.AddRange(new[] {v2, v20, v12});
                triangles.AddRange(new[] {v01, v12, v20});
            }
            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = triangles.ToArray();
            return mesh;
        }

        // TODO:
        //public static void Scale(this Mesh mesh)
        //{
        //}
        //public static void Snap(this Mesh mesh)
        //{
        //}
        //public static void Mirror(this Mesh mesh)
        //{
        //}
        //public static void SetPivot(this Mesh mesh, Vector3 pivot)
        //{
        //}

        public static Mesh CombineMeshes(List<Mesh> meshes, List<Matrix4x4> matrices = null)
        {
            var combine = new CombineInstance[meshes.Count];
            for (var i = 0; i < meshes.Count; i++)
            {
                combine[i].mesh = meshes[i];
                if (matrices != null)
                {
                    combine[i].transform = matrices[i];
                }
            }

            var mesh = new Mesh();
            mesh.CombineMeshes(combine, true, matrices != null);
            return mesh;
        }

        public static Mesh CombineMeshes(this Mesh mesh, List<Mesh> meshes, List<Matrix4x4> matrices = null)
        {
            var combine = new CombineInstance[meshes.Count];
            for (var i = 0; i < meshes.Count; i++)
            {
                combine[i].mesh = meshes[i];
                if (matrices != null)
                {
                    combine[i].transform = matrices[i];
                }
            }

            mesh.Clear();
            mesh.CombineMeshes(combine, true, matrices != null);
            return mesh;
        }

        public static Mesh CombineMeshesWithSubMeshes(List<Mesh> meshes, List<List<int>> submeshMask = null)
        {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uv = new List<Vector2>();
            var triangles = new List<int>();
            var submeshes = new List<List<int>>();

            for (var i = 0; i < 100; i++)
            {
                submeshes.Add(new List<int>());
            }

            for (var i = 0; i < meshes.Count; i++)
            {
                for (var j = 0; j < meshes[i].subMeshCount; j++)
                {
                    var newSubmesh = meshes[i].GetTriangles(j);
                    for (var k = 0; k < newSubmesh.Length; k++)
                    {
                        newSubmesh[k] += vertices.Count;
                    }

                    if (submeshMask != null)
                    {
                        submeshes[submeshMask[i][j]].AddRange(newSubmesh);
                    }
                    else
                    {
                        submeshes[j].AddRange(newSubmesh);
                    }
                }
                var newTriangles = meshes[i].triangles;
                for (var j = 0; j < newTriangles.Length; j++)
                {
                    triangles.Add(newTriangles[j] + vertices.Count);
                }
                vertices.AddRange(meshes[i].vertices);
                normals.AddRange(meshes[i].normals);
                uv.AddRange(meshes[i].uv);
            }

            var mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                normals = normals.ToArray(),
                uv = uv.ToArray(),
                triangles = triangles.ToArray(),
                subMeshCount = submeshes.Count
            };
            for (var i = 0; i < submeshes.Count; i++)
            {
                mesh.SetTriangles(submeshes[i].ToArray(), i);
            }

            return mesh;
        }

        public static void InvertNormals(this Mesh mesh)
        {
            var normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;
        }

        public static void InvertTriangles(this Mesh mesh)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                var triangles = mesh.GetTriangles(i);
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    PGUtils.Swap(ref triangles[j], ref triangles[j + 1]);
                }
                mesh.SetTriangles(triangles, i);
            }
        }

        /// <summary>
        /// Recalculates the tangents of the mesh from the normals and uv.
        /// 
        /// Based on:
        /// Eric Lengyel. “Computing Tangent Space Basis Vectors for an Arbitrary Mesh”. Terathon Software 3D Graphics Library, 2001.
        /// http://www.terathon.com/code/tangent.html
        /// Aras Pranckevičius. “Unity tangent space calculation”
        /// https://gist.github.com/aras-p/2843984
        /// NVIDIA Mesh Processing Tools
        /// http://code.google.com/p/nvidia-mesh-tools/
        /// </summary>
        public static void RecalculateTangents(this Mesh mesh)
        {
            var vertices = mesh.vertices;
            var vertexCount = mesh.vertexCount;
            var triangles = mesh.triangles;
            var triangleCount = triangles.Length;
            var normals = mesh.normals;
            var uv = mesh.uv;
            var tangents = new Vector3[vertexCount];
            var bitangents = new Vector3[vertexCount];
            var outTangents = new Vector4[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                tangents[i] = Vector3.zero;
                bitangents[i] = Vector3.zero;
            }

            for (int i = 0; i < triangleCount; i += 3)
            {
                int i0 = triangles[i];
                int i1 = triangles[i + 1];
                int i2 = triangles[i + 2];

                Vector3 p = vertices[i1] - vertices[i0];
                Vector3 q = vertices[i2] - vertices[i0];

                Vector2 s = uv[i1] - uv[i0];
                Vector2 t = uv[i2] - uv[i0];

                float denom = s.x*t.y - s.y*t.x;

                if (Mathf.Abs(denom) >= 1e-8)
                {
                    float r = 1f/denom;
                    s *= r;
                    t *= r;

                    var tangent = new Vector3((t.y*p.x - s.y*q.x), (t.y*p.y - s.y*q.y), (t.y*p.z - s.y*q.z));
                    var bitangent = new Vector3((s.x*q.x - t.x*p.x), (s.x*q.y - t.x*p.y), (s.x*q.z - t.x*p.z));

                    tangents[i0] += tangent;
                    tangents[i1] += tangent;
                    tangents[i2] += tangent;

                    bitangents[i0] += bitangent;
                    bitangents[i1] += bitangent;
                    bitangents[i2] += bitangent;
                }
            }

            for (int i = 0; i < vertexCount; i++)
            {
                var normal = normals[i];
                var tangent = tangents[i].normalized;
                var bitangent = bitangents[i].normalized;

                // Gram-Schmidt orthogonalize
                Vector3.OrthoNormalize(ref normal, ref tangent);

                outTangents[i] = tangent;
                // Calculate handedness
                outTangents[i].w = (Vector3.Dot(Vector3.Cross(normal, tangent), bitangent) < 0f) ? -1f : 1f;
            }
            mesh.tangents = outTangents;
        }

        public static void RecalculateUv(this Mesh mesh)
        {
            var vertices = mesh.vertices;
            var vertexCount = vertices.Length;
            var uv = new Vector2[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                uv[i] = GetSphericalUV(ref vertices[i]);
            }

            mesh.uv = uv;
        }

        private static Vector2 GetSphericalUV(ref Vector3 p)
        {
            var pn = p.normalized;
            return new Vector2((0.5f + Mathf.Atan2(pn.z, pn.x)/(Mathf.PI*2.0f)),
                1.0f - (0.5f - Mathf.Asin(pn.y)/Mathf.PI));
        }
    }
}