using System.Text;

namespace ProceduralToolkit
{
    public static class StringE
    {
        public static string Random(this string chars, int length = 8)
        {
            var randomString = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                randomString.Append(chars[UnityEngine.Random.Range(0, chars.Length)]);
            }
            return randomString.ToString();
        }
    }
}