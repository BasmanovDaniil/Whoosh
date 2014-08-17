using System;
using UnityEngine;

namespace ProceduralToolkit
{
    public static class TextureE
    {
        public static Texture2D whitePixel
        {
            get { return Pixel(Color.white); }
        }

        public static Texture2D blackPixel
        {
            get { return Pixel(Color.black); }
        }

        public static Texture2D checker
        {
            get { return Checker(Color.white, Color.black); }
        }

        public static Texture2D Pixel(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public static Texture2D Gradient(Color top, Color bottom)
        {
            var texture = new Texture2D(2, 2) {wrapMode = TextureWrapMode.Clamp};
            texture.SetPixels(new[] {bottom, bottom, top, top});
            texture.Apply();
            return texture;
        }

        public static Texture2D Checker(Color first, Color second)
        {
            var texture = new Texture2D(2, 2) {filterMode = FilterMode.Point};
            texture.SetPixels(new[] {second, first, first, second});
            texture.Apply();
            return texture;
        }

        public static void DrawLine(this Texture2D texture, IntSegment2 segment, Color color, bool AA = false)
        {
            DrawLine(texture, segment.a, segment.b, color, AA);
        }

        public static void DrawLine(this Texture2D texture, IntVertex2 v0, IntVertex2 v1, Color color, bool AA = false)
        {
            DrawLine(texture, v0.x, v0.y, v1.x, v1.y, color, AA);
        }

        public static void DrawLine(this Texture2D texture, int x0, int y0, int x1, int y1, Color color, bool AA = false)
        {
            if (AA)
            {
                Action<int, int, float> draw =
                    (x, y, t) => texture.SetPixel(x, y, Color.Lerp(texture.GetPixel(x, y), color, t));
                WuLine(x0, y0, x1, y1, draw);
            }
            else
            {
                Action<int, int> draw = (x, y) => texture.SetPixel(x, y, color);
                BresenhamLine(x0, y0, x1, y1, draw);
            }
            texture.Apply();
        }

        public static void DrawGradientLine(this Texture2D texture, IntSegment2 segment, Color color0, Color color1)
        {
            DrawGradientLine(texture, segment.a, segment.b, color0, color1);
        }

        public static void DrawGradientLine(this Texture2D texture, IntVertex2 v0, IntVertex2 v1, Color color0,
            Color color1)
        {
            DrawGradientLine(texture, v0.x, v0.y, v1.x, v1.y, color0, color1);
        }

        public static void DrawGradientLine(this Texture2D texture, int x0, int y0, int x1, int y1, Color color0,
            Color color1)
        {
            var distance = Vertex2.Distance(new Vertex2(x0, y0), new Vertex2(x1, y1));
            Action<int, int> draw;
            if (distance > 0)
            {
                draw = (x, y) =>
                {
                    var percent = Vertex2.Distance(new Vertex2(x0, y0), new Vertex2(x, y)) / distance;
                    texture.SetPixel(x, y, Color.Lerp(color0, color1, percent));
                };
            }
            else
            {
                draw = (x, y) => texture.SetPixel(x, y, color0);
            }
            BresenhamLine(x0, y0, x1, y1, draw);
            texture.Apply();
        }

        public static void DrawIntPolygon(this Texture2D texture, IntPolygon polygon, Color color)
        {
            if (polygon.Count < 2)
            {
                return;
            }
            Action<int, int> draw = (x, y) => texture.SetPixel(x, y, color);
            for (var i = 0; i < polygon.Count; i++)
            {
                BresenhamLine(polygon[i], polygon[i + 1], draw);
            }
            texture.Apply();
        }

        public static void DrawGradientIntPolygon(this Texture2D texture, IntPolygon polygon, Color color0, Color color1)
        {
            if (polygon.Count < 2)
            {
                return;
            }
            for (var i = 0; i < polygon.Count; i++)
            {
                var distance = Vertex2.Distance((Vertex2)polygon[i], (Vertex2)polygon[i + 1]);
                Action<int, int> draw;
                if (distance > 0)
                {
                    draw = (x, y) =>
                    {
                        var percent = Vertex2.Distance((Vertex2)polygon[i], new Vertex2(x, y)) / distance;
                        texture.SetPixel(x, y, Color.Lerp(color0, color1, percent));
                    };
                }
                else
                {
                    draw = (x, y) => texture.SetPixel(x, y, color0);
                }
                BresenhamLine(polygon[i], polygon[i + 1], draw);
            }
            texture.Apply();
        }

        public static void BresenhamLine(IntSegment2 segment, Action<int, int> draw)
        {
            BresenhamLine(segment.a, segment.b, draw);
        }

        public static void BresenhamLine(IntVertex2 v0, IntVertex2 v1, Action<int, int> draw)
        {
            BresenhamLine(v0.x, v0.y, v1.x, v1.y, draw);
        }

        public static void BresenhamLine(int x0, int y0, int x1, int y1, Action<int, int> draw)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                PGUtils.Swap(ref x0, ref y0);
                PGUtils.Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                PGUtils.Swap(ref x0, ref x1);
                PGUtils.Swap(ref y0, ref y1);
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx/2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                draw(steep ? y : x, steep ? x : y);
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        public static void BresenhamLineSwapless(int x0, int y0, int x1, int y1, Action<int, int> draw)
        {
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sy = (y0 < y1) ? 1 : -1;
            int sx = (x0 < x1) ? 1 : -1;
            int err = dx - dy;
            while (true)
            {
                draw(x0, y0);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2*err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (x0 == x1 && y0 == y1)
                {
                    draw(x0, y0);
                    break;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        private static void WuLine(IntSegment2 segment, Action<int, int, float> draw)
        {
            WuLine(segment.a, segment.b, draw);
        }

        private static void WuLine(IntVertex2 v0, IntVertex2 v1, Action<int, int, float> draw)
        {
            WuLine(v0.x, v0.y, v1.x, v1.y, draw);
        }

        private static void WuLine(int x0, int y0, int x1, int y1, Action<int, int, float> draw)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                PGUtils.Swap(ref x0, ref y0);
                PGUtils.Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                PGUtils.Swap(ref x0, ref x1);
                PGUtils.Swap(ref y0, ref y1);
            }

            if (steep)
            {
                draw(y0, x0, 1);
                draw(y1, x1, 1);
            }
            else
            {
                draw(x0, y0, 1);
                draw(x1, y1, 1);
            }
            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = dy/dx;
            float y = y0 + gradient;
            for (var x = x0 + 1; x <= x1 - 1; x++)
            {
                if (steep)
                {
                    draw((int) y, x, 1 - (y - (int) y));
                    draw((int) y + 1, x, y - (int) y);
                }
                else
                {
                    draw(x, (int) y, 1 - (y - (int) y));
                    draw(x, (int) y + 1, y - (int) y);
                }
                y += gradient;
            }
        }

        public static void DrawCircle(this Texture2D texture, IntVertex2 v0, int radius, Color color)
        {
            Action<int, int> draw = (x, y) => texture.SetPixel(x, y, color);
            BresenhamCircle(v0.x, v0.y, radius, draw);
            texture.Apply();
        }

        public static void BresenhamCircle(int x0, int y0, int radius, Action<int, int> draw)
        {
            int x = radius;
            int y = 0;
            int radiusError = 1 - x;
            while (x >= y)
            {
                draw(x + x0, y + y0);
                draw(y + x0, x + y0);
                draw(-x + x0, y + y0);
                draw(-y + x0, x + y0);
                draw(-x + x0, -y + y0);
                draw(-y + x0, -x + y0);
                draw(x + x0, -y + y0);
                draw(y + x0, -x + y0);
                y++;
                if (radiusError < 0)
                {
                    radiusError += 2*y + 1;
                }
                else
                {
                    x--;
                    radiusError += 2*(y - x + 1);
                }
            }
        }

        public static void DrawRect(this Texture2D texture, int x, int y, int blockWidth, int blockHeight, Color color)
        {
            var colors = new Color[blockWidth * blockHeight];
            for (int i = 0; i < blockHeight; i++)
            {
                for (int j = 0; j < blockWidth; j++)
                {
                    colors[j + i * blockWidth] = color;
                }
            }
            texture.SetPixels(x, y, blockWidth, blockHeight, colors);
            texture.Apply();
        }

        public static void Clear(this Texture2D texture)
        {
            Clear(texture, Color.white);
        }

        public static void Clear(this Texture2D texture, Color color)
        {
            var pixels = texture.GetPixels();
            for (var i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
        }
    }
}