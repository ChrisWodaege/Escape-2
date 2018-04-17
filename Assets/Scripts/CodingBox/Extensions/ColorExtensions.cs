using UnityEngine;

public static class ColorExtensions
{

    public static string ColorToHex(this Color color)
    {
        Color32 color32 = (Color32)color;
        string hex = "#" + color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2");
        return hex;
    }

    public static Color HexToColor(this string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }
}
