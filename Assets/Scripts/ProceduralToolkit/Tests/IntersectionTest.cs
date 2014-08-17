using System.Collections.Generic;
using UnityEngine;

namespace ProceduralToolkit.Tests
{
    public class IntersectionTest : MonoBehaviour
    {
        private IntPolygon polygon1 = new IntPolygon
        {
            new IntVertex2(30, 30),
            new IntVertex2(30, 40),
            new IntVertex2(40, 40),
            new IntVertex2(40, 30)
        };

        private IntPolygon polygon2 = new IntPolygon
        {
            new IntVertex2(30, 30),
            new IntVertex2(35, 30),
            new IntVertex2(35, 35),
            new IntVertex2(30, 35)
        };

        private IntSegment2 segment1 = new IntSegment2(IntVertex2.up*70) + IntVertex2.one*100;
        private IntSegment2 segment2I = new IntSegment2(IntVertex2.down*50) + IntVertex2.one*100;
        private IntSegment2 segment3 = new IntSegment2(IntVertex2.right*70) + IntVertex2.one*100;
        private IntSegment2 segment4 = new IntSegment2(IntVertex2.left*50) + IntVertex2.one*100;

        private Texture2D texture;
        private int counter;

        private void Start()
        {
            texture = new Texture2D(64, 64) {filterMode = FilterMode.Point};
            renderer.material.mainTexture = texture;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                segment1 += IntVertex2.left;
                segment3 += IntVertex2.left;
                polygon1 += IntVertex2.left;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                segment1 += IntVertex2.right;
                segment3 += IntVertex2.right;
                polygon1 += IntVertex2.right;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                segment1 += IntVertex2.up;
                segment3 += IntVertex2.up;
                polygon1 += IntVertex2.up;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                segment1 += IntVertex2.down;
                segment3 += IntVertex2.down;
                polygon1 += IntVertex2.down;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                segment2I += IntVertex2.left;
                segment4 += IntVertex2.left;
                polygon2 += IntVertex2.left;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                segment2I += IntVertex2.right;
                segment4 += IntVertex2.right;
                polygon2 += IntVertex2.right;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                segment2I += IntVertex2.up;
                segment4 += IntVertex2.up;
                polygon2 += IntVertex2.up;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                segment2I += IntVertex2.down;
                segment4 += IntVertex2.down;
                polygon2 += IntVertex2.down;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (counter)
                {
                    case 0:
                        polygon1 = new IntPolygon
                        {
                            new IntVertex2(30, 30),
                            new IntVertex2(20, 35),
                            new IntVertex2(30, 40),
                            new IntVertex2(40, 35)
                        };
                        polygon2 = new IntPolygon
                        {
                            new IntVertex2(30, 30),
                            new IntVertex2(22, 34),
                            new IntVertex2(30, 38),
                            new IntVertex2(38, 34)
                        };
                        counter++;
                        break;
                    case 1:
                        polygon1 = new IntPolygon
                        {
                            new IntVertex2(30, 30),
                            new IntVertex2(30, 40),
                            new IntVertex2(40, 40),
                            new IntVertex2(40, 30)
                        };
                        polygon2 = new IntPolygon
                        {
                            new IntVertex2(30, 30),
                            new IntVertex2(35, 30),
                            new IntVertex2(35, 35),
                            new IntVertex2(30, 35)
                        };
                        counter = 0;
                        break;
                }
            }

            texture.Clear();

            //IntSegment2 intersection;
            //if (segment1.Intersects(segment2, out intersection))
            //{
            //    texture.DrawLine(segment1, Color.red);
            //    texture.DrawLine(segment2, Color.red);
            //    texture.DrawLine(intersection, RandomE.colorHSV);
            //}
            //else
            //{
            //    texture.DrawLine(segment1, Color.green);
            //    texture.DrawLine(segment2, Color.green);
            //}

            //if (segment3.Intersects(segment4, out intersection))
            //{
            //    texture.DrawLine(segment3, Color.red);
            //    texture.DrawLine(segment4, Color.red);
            //    texture.DrawLine(intersection, RandomE.colorHSV);
            //}
            //else
            //{
            //    texture.DrawLine(segment3, Color.green);
            //    texture.DrawLine(segment4, Color.green);
            //}

            List<IntSegment2> intersections;
            if (polygon1.Intersects(polygon2, out intersections))
            {
                texture.DrawGradientIntPolygon(polygon1, Color.blue, Color.red);
                texture.DrawGradientIntPolygon(polygon2, Color.blue, Color.red);
                foreach (var i in intersections)
                {
                    texture.DrawLine(i, RandomE.colorHSV);
                }
            }
            else
            {
                texture.DrawGradientIntPolygon(polygon1, Color.blue, Color.green);
                texture.DrawGradientIntPolygon(polygon2, Color.blue, Color.green);
            }
        }
    }
}