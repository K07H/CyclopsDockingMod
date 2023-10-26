namespace CyclopsDockingMod;
using UnityEngine;

internal static class FastHelper
{
    public static bool NearlyEquals(float a, float b)
    {
        return a < b + 0.1f && a > b - 0.1f;
    }

    public static bool IsNear(Vector3 a, Vector3 b)
    {
        return a.x < b.x + 0.1f && a.x > b.x - 0.1f && a.y < b.y + 0.1f && a.y > b.y - 0.1f && a.z < b.z + 0.1f && a.z > b.z - 0.1f;
    }

    public static float AngleDiff(float a, float b)
    {
        if (a >= 180f)
            a = (360f - a) * -1f;
        if (b >= 180f)
            b = (360f - b) * -1f;
        float num = (a - b) * -1f;
        if (num < -180f)
            return 360f + num;
        if (num <= 180f)
            return num;
        return num - 360f;
    }
}
