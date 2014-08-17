using UnityEngine;

public static class ColorE
{
    public static Color HSVToRGB(float h, float s, float v)
    {
        Color color = Color.white;
        if (s == 0f)
        {
            color.r = v;
            color.g = v;
            color.b = v;
            return color;
        }
        if (v == 0f)
        {
            color.r = 0f;
            color.g = 0f;
            color.b = 0f;
            return color;
        }
        color.r = 0f;
        color.g = 0f;
        color.b = 0f;
        float num = s;
        float num2 = v;
        float f = h*6f;
        int num4 = (int) Mathf.Floor(f);
        float num5 = f - num4;
        float num6 = num2*(1f - num);
        float num7 = num2*(1f - (num*num5));
        float num8 = num2*(1f - (num*(1f - num5)));
        int num9 = num4;
        switch ((num9 + 1))
        {
            case 0:
                color.r = num2;
                color.g = num6;
                color.b = num7;
                break;

            case 1:
                color.r = num2;
                color.g = num8;
                color.b = num6;
                break;

            case 2:
                color.r = num7;
                color.g = num2;
                color.b = num6;
                break;

            case 3:
                color.r = num6;
                color.g = num2;
                color.b = num8;
                break;

            case 4:
                color.r = num6;
                color.g = num7;
                color.b = num2;
                break;

            case 5:
                color.r = num8;
                color.g = num6;
                color.b = num2;
                break;

            case 6:
                color.r = num2;
                color.g = num6;
                color.b = num7;
                break;

            case 7:
                color.r = num2;
                color.g = num8;
                color.b = num6;
                break;
        }
        color.r = Mathf.Clamp(color.r, 0f, 1f);
        color.g = Mathf.Clamp(color.g, 0f, 1f);
        color.b = Mathf.Clamp(color.b, 0f, 1f);
        return color;
    }

    public static Color Inverted(this Color color)
    {
        return Color.white - color;
    }
}