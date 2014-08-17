using UnityEngine;

namespace ProceduralToolkit.Tests
{
    public class RandomETest : MonoBehaviour
    {
        public int length = 256;
        public int count = 1000;
        public int m = 150;
        public Color color = Color.blue;

        private Texture2D texture;
        private int[,] statistic;
        private int[] test;

        private void Update()
        {
            texture = new Texture2D(length, length) {filterMode = FilterMode.Point};
            renderer.material.mainTexture = texture;
            statistic = new int[length, length];
            test = new int[length];

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    test[j] = j;
                }
                test.Shuffle();
                for (int j = 0; j < test.Length; j++)
                {
                    statistic[j, test[j]]++;
                }
            }

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    texture.SetPixel(i, j, color*statistic[i, j]/count*m);
                }
            }
            texture.Apply();
        }
    }
}