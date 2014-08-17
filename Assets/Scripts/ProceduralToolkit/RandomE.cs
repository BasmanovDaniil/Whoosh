using System.Collections.Generic;
using UnityEngine;

namespace ProceduralToolkit
{
    public static class RandomE
    {
        public static Color color
        {
            get { return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value); }
        }

        public static Color colorHSV
        {
            get { return ColorE.HSVToRGB(UnityEngine.Random.value, 1, 1); }
        }

        /// <summary>
        /// Returns 8-character string
        /// </summary>
        public static string shortString
        {
            get { return Datasets.alphanumerics.Random(); }
        }

        /// <summary>
        /// Returns 16-character string
        /// </summary>
        public static string longString
        {
            get { return Datasets.alphanumerics.Random(16); }
        }

        public static string name
        {
            get { return Chance(0.5f) ? femaleName : maleName; }
        }

        public static string femaleName
        {
            get { return Datasets.femaleNames.Random(); }
        }

        public static string maleName
        {
            get { return Datasets.maleNames.Random(); }
        }

        public static string lastName
        {
            get { return Datasets.lastNames.Random(); }
        }

        public static Vector2 insideUnitSquare
        {
            get { return new Vector2(UnityEngine.Random.value, UnityEngine.Random.value); }
        }

        public static Vector2 insideUnitRect
        {
            get { return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)); }
        }

        public static Polygon polygon
        {
            get
            {
                return new Polygon
                {
                    UnityEngine.Random.insideUnitCircle,
                    UnityEngine.Random.insideUnitCircle,
                    UnityEngine.Random.insideUnitCircle,
                    UnityEngine.Random.insideUnitCircle,
                    UnityEngine.Random.insideUnitCircle,
                    UnityEngine.Random.insideUnitCircle,
                    UnityEngine.Random.insideUnitCircle,
                    UnityEngine.Random.insideUnitCircle
                };
            }
        }

        public static Polygon rectPolygon
        {
            get
            {
                var width = UnityEngine.Random.value;
                var height = UnityEngine.Random.value;
                return new Polygon
                {
                    new Vector2(width, height),
                    new Vector2(width, -height),
                    new Vector2(-width, -height),
                    new Vector2(-width, height)
                };
            }
        }

        public static T Random<T>(this List<T> items)
        {
            if (items.Count == 0)
            {
                Debug.LogError("RandomE.Choice: items.Count == 0");
                return default(T);
            }
            return items[UnityEngine.Random.Range(0, items.Count)];
        }

        public static T Random<T>(this T[] items)
        {
            if (items.Length == 0)
            {
                Debug.LogError("RandomE.Choice: items.Length == 0");
                return default(T);
            }
            return items[UnityEngine.Random.Range(0, items.Length)];
        }

        public static T Choice<T>(T item1, T item2, params T[] items)
        {
            var list = new List<T>(items) {item1, item2};
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Fisher–Yates shuffle
        /// </summary>
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int j = UnityEngine.Random.Range(i, array.Length);
                T tmp = array[j];
                array[j] = array[i];
                array[i] = tmp;
            }
        }

        /// <summary>
        /// Fisher–Yates shuffle
        /// </summary>
        public static void Shuffle<T>(this List<T> array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                int j = UnityEngine.Random.Range(i, array.Count);
                T tmp = array[j];
                array[j] = array[i];
                array[i] = tmp;
            }
        }

        public static bool Chance(float percent)
        {
            return UnityEngine.Random.value < percent;
        }
    }
}