using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProceduralToolkit.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralToolkit.Examples
{
    public class Room : IntPolygon
    {
        public bool canGrow = true;
        public bool[] growableWalls;
        public List<Segment2> freeSegments = new List<Segment2>();
        public Color color;

        public Room(IEnumerable<IntVertex2> polygon) : base(polygon)
        {
            growableWalls = new bool[Count];
            for (int i = 0; i < growableWalls.Length; i++)
            {
                growableWalls[i] = true;
            }

            color = RandomE.colorHSV;
        }
    }

    public class FloorPlanGenerator : MonoBehaviour
    {
        public int resolution = 256;
        public bool canGrow = true;
        public bool canExtrude = true;
        public bool debug = false;

        private Texture2D texture;
        private Texture2D debugTexture;
        private RaycastHit hit;
        private List<Room> rooms = new List<Room>();
        private Color fillColor = Color.white;
        private List<Indexed<IntSegment2>> freeSegments = new List<Indexed<IntSegment2>>();
        private List<int[]> outerWalls = new List<int[]>();

        private void Awake()
        {
            texture = new Texture2D(resolution, resolution) {filterMode = FilterMode.Point};
            texture.Clear(Color.clear);
            debugTexture = new Texture2D(resolution, resolution) {filterMode = FilterMode.Point};
            debugTexture.Clear(Color.clear);
        }

        private void Start()
        {
            GenerateOuterWalls();
            GenerateOuterRoom();
            //StartCoroutine(GenerateRooms());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                rooms.Clear();
                GenerateOuterWalls();
                GenerateOuterRoom();
                //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform == tr)
                //{
                //    var x = (int) (hit.textureCoord.x*texture.width);
                //    var y = (int) (hit.textureCoord.y*texture.height);
                //    //if (!CheckRect(x - 1, y - 1, x + 1, y + 1, Color.white, Color.white)) return;
                //    rooms.Add(new Room(new IntVertex2(x, y) + IntPolygon.unitRect));
                //}
            }
            texture.Clear(Color.clear);
            DrawWalls();
            foreach (var room in rooms)
            {
                texture.DrawIntPolygon(room, room.color);
            }

            if (Input.GetMouseButton(1))
            {
                foreach (var room in rooms)
                {
                    var intersections = new List<IntSegment2>();
                    if (rooms.Any(r => r != room && r.Intersects(room, out intersections)))
                    {
                        foreach (var intersection in intersections)
                        {
                            texture.DrawLine(intersection, RandomE.colorHSV);
                        }
                    }
                }
            }

            //debugTexture.Clear(Color.clear);
            //foreach (var segment in freeSegments)
            //{
            //    debugTexture.DrawLine(segment.value, RandomE.colorHSV);
            //}
        }

        private void OnGUI()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture, ScaleMode.ScaleToFit);
            if (debug)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), debugTexture, ScaleMode.ScaleToFit);
            }
        }

        private void RandomRect(out int x, out int y, out int blockWidth, out int blockHeight)
        {
            blockWidth = Random.Range(resolution/8, resolution/2);
            blockHeight = Random.Range(resolution/8, resolution/2);
            x = Random.Range(0, resolution - blockWidth);
            y = Random.Range(0, resolution - blockHeight);
        }

        private void GenerateOuterWalls()
        {
            outerWalls = new List<int[]>();
            for (int i = 0; i < 2; i++)
            {
                int x, y, blockWidth, blockHeight;
                RandomRect(out x, out y, out blockWidth, out blockHeight);
                outerWalls.Add(new[] {x, y, blockWidth, blockHeight});
            }
        }

        private void GenerateOuterRoom()
        {
            var outerRooms = new List<Room>();
            foreach (var o in outerWalls)
            {
                outerRooms.Add(new Room(new[]
                {
                    new IntVertex2(o[0], o[1]),
                    new IntVertex2(o[0], o[1] + o[3]),
                    new IntVertex2(o[0] + o[2], o[1] + o[3]),
                    new IntVertex2(o[0] + o[2], o[1])
                }));
            }
            var outerVertices = new List<IntVertex2>();
            foreach (var room in outerRooms)
            {
                outerVertices.AddRange(room);
                foreach (var o in outerRooms)
                {
                    List<IntSegment2> intersections;
                    if (room != o && Intersection.IntPolygonToIntPolygon(room, o, out intersections))
                    {
                        foreach (var intersection in intersections)
                        {
                            if (intersection.a == intersection.b)
                            {
                                outerVertices.Add(intersection.a);
                            }
                        }
                    }
                }
            }
            var outerRoom = new Room(outerVertices) {canGrow = false, color = Color.gray};
            //outerRoom.RemoveDuplicateVertices();
            outerRoom.RemoveCollinearVertices();
            rooms.Add(outerRoom);
        }

        public Room RoomHull(Room room)
        {
            var aabb = room.aabb;
            var vertices = new List<IntVertex2>(room);
            var newRoom = new List<IntVertex2>();
            var last = ScanY(vertices, aabb.min, aabb.max.y) ?? ScanX(vertices, aabb.min, aabb.max.x);
            if (last == null)
            {
                return null;
            }
            newRoom.Add(last);
            vertices.Remove(last);

            bool parallelX = false;
            while (vertices.Count > 0)
            {
                if (parallelX)
                {
                    last = ScanX(vertices, newRoom.Last(), aabb.max.x) ?? ScanX(vertices, newRoom.Last(), aabb.min.x);
                    if (last != null)
                    {
                        newRoom.Add(last);
                        vertices.Remove(last);
                        parallelX = false;
                    }
                }
                else
                {
                    last = ScanY(vertices, newRoom.Last(), aabb.max.y) ?? ScanY(vertices, newRoom.Last(), aabb.min.y);
                    if (last != null)
                    {
                        newRoom.Add(last);
                        vertices.Remove(last);
                        parallelX = true;
                    }
                }
            }
            return new Room(newRoom);
        }

        private IntVertex2 ScanX(List<IntVertex2> vertices, IntVertex2 start, int endX)
        {
            if (start.x < endX)
            {
                for (int x = start.x; x <= endX; x++)
                {
                    var vertex = vertices.Find(v => v.y == start.y && v.x == x);
                    if (vertex != null)
                    {
                        return vertex;
                    }
                }
            }
            else
            {
                for (int x = start.x; x >= endX; x--)
                {
                    var vertex = vertices.Find(v => v.y == start.y && v.x == x);
                    if (vertex != null)
                    {
                        return vertex;
                    }
                }
            }
            return null;
        }

        private IntVertex2 ScanY(List<IntVertex2> vertices, IntVertex2 start, int endY)
        {
            if (start.y < endY)
            {
                for (int y = start.y; y <= endY; y++)
                {
                    var vertex = vertices.Find(v => v.x == start.x && v.y == y);
                    if (vertex != null)
                    {
                        return vertex;
                    }
                }
            }
            else
            {
                for (int y = start.y; y >= endY; y--)
                {
                    var vertex = vertices.Find(v => v.x == start.x && v.y == y);
                    if (vertex != null)
                    {
                        return vertex;
                    }
                }
            }
            return null;
        }

        private void DrawWalls()
        {
            foreach (var o in outerWalls)
            {
                texture.DrawRect(o[0], o[1], o[2], o[3], Color.black);
            }
            foreach (var o in outerWalls)
            {
                texture.DrawRect(o[0] + 1, o[1] + 1, o[2] - 2, o[3] - 2, Color.white);
            }
        }

        private IEnumerator GenerateRooms()
        {
            canGrow = true;
            canExtrude = true;
            while (true)
            {
                yield return StartCoroutine(Grow());
                //yield return StartCoroutine(Extrude());
            }
        }

        private IEnumerator Grow()
        {
            //while (canGrow)
            {
                for (int i = 1; i < rooms.Count; i++)
                {
                    var room = rooms[i];
                    if (!room.canGrow) continue;

                    var indexedWalls = room.indexedSegments;
                    indexedWalls.Shuffle();
                    indexedWalls.Sort((a, b) => a.value.CompareByLength(b.value));

                    for (int j = indexedWalls.Count - 1; j >= 0; j--)
                    {
                        var index = indexedWalls[j].index;
                        if (!room.growableWalls[index]) continue;

                        var wall = room.Segment(index);

                        if (rooms.Any(r => r != room && r.Intersects(wall)))
                        {
                            room.growableWalls[index] = false;
                        }
                        else
                        {
                            var left = wall.left;
                            var newWall = wall + left;

                            if (rooms.Any(r => r != room && r.Intersects(newWall)))
                            {
                                room.growableWalls[index] = false;
                                if (!room.growableWalls.Any(w => w))
                                {
                                    room.canGrow = false;
                                }
                            }
                            else
                            {
                                room[index] += left;
                                room[index + 1] += left;
                            }
                        }
                    }
                }
                canGrow = rooms.Any(r => r.canGrow);
                if (!canGrow)
                {
                    foreach (var room in rooms)
                    {
                        room.canGrow = true;
                    }
                }
                yield return null;
            }
        }

        private IEnumerator Extrude()
        {
            //while (canExtrude)
            {
                freeSegments = new List<Indexed<IntSegment2>>();

                for (int i = 1; i < rooms.Count; i++)
                {
                    var room = rooms[i];
                    //if (!room.canGrow) continue;

                    var newSegments = new List<Indexed<IntSegment2>>();

                    foreach (var wall in room.segments)
                    {
                        IntVertex2 start = null, end = null;
                        Action<int, int> getPixel = (x, y) =>
                        {
                            var color = texture.GetPixel(x, y);
                            if (color == fillColor || color == room.color)
                            {
                                debugTexture.SetPixel(x, y, RandomE.colorHSV);
                                if (start == null)
                                {
                                    start = new IntVertex2(x, y);
                                }
                                if (null == end)
                                {
                                    end = new IntVertex2(x, y);
                                }
                                else
                                {
                                    end.x = x;
                                    end.y = y;
                                }
                            }
                            else
                            {
                                if (start != null)
                                {
                                    newSegments.Add(new Indexed<IntSegment2>(new IntSegment2(start, end), i));
                                    start = null;
                                    end = null;
                                }
                            }
                        };
                        TextureE.BresenhamLine(wall + wall.left, getPixel);
                    }
                    if (newSegments.Count == 0)
                    {
                        //room.canGrow = false;
                    }
                    else
                    {
                        freeSegments.AddRange(newSegments);
                    }
                }
                debugTexture.Apply();
                canExtrude = rooms.Any(r => r.canGrow);
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}