using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtil
{
    // Idea taken from https://stackoverflow.com/questions/5928725/hashing-2d-3d-and-nd-vectors
    private static int p1 = 73856093, p3 = 83492791;
    public static int HashableV2I(this Vector2Int v)
    {
        return (v.x ^ p1) * (v.y ^ p3);
    }
}

//class V2IEqualityComparer : IEqualityComparer<Vector2Int>
//{
//    public bool Equals(Vector2Int v1, Vector2Int v2)
//    {
//        if (v1 == null || v2 == null) return false;
//        return v1.x == v2.x && v1.y == v2.y;
//    }

//    public int GetHashCode(Vector2Int v)
//    {
//        return v.HashableV2I();
//    }
//}