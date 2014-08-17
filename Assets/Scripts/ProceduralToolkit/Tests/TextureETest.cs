using UnityEngine;

namespace ProceduralToolkit.Tests
{
    public class TextureETest : MonoBehaviour
    {
        private int counter;
        private RaycastHit hit;
        private IntVertex2 start;

        private void Start()
        {
            renderer.material.mainTexture = TextureE.Pixel(Color.blue);
            counter++;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (counter)
                {
                    case 0:
                        renderer.material.mainTexture = TextureE.Pixel(Color.blue);
                        counter++;
                        break;
                    case 1:
                        renderer.material.mainTexture = TextureE.whitePixel;
                        counter++;
                        break;
                    case 2:
                        renderer.material.mainTexture = TextureE.blackPixel;
                        counter++;
                        break;
                    case 3:
                        renderer.material.mainTexture = TextureE.Gradient(Color.red, Color.green);
                        counter++;
                        break;
                    case 4:
                        renderer.material.mainTexture = TextureE.checker;
                        counter++;
                        break;
                    case 5:
                        renderer.material.mainTexture = new Texture2D(256, 256) {filterMode = FilterMode.Point};
                        counter = 0;
                        break;
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform == transform)
                {
                    var texture = (Texture2D)renderer.material.mainTexture;
                    var x = (int)(hit.textureCoord.x * texture.width);
                    var y = (int)(hit.textureCoord.y * texture.height);
                    start = new IntVertex2(x, y);
                    texture.DrawCircle(start, 4, RandomE.colorHSV);
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform == transform)
                {
                    var texture = (Texture2D)renderer.material.mainTexture;
                    var x = (int)(hit.textureCoord.x * texture.width);
                    var y = (int)(hit.textureCoord.y * texture.height);
                    var end = new IntVertex2(x, y);
                    texture.DrawLine(start, end, RandomE.colorHSV);
                    texture.DrawCircle(end, 4, RandomE.colorHSV);
                }
            }
        }
    }
}
