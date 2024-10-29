
using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;

public static class Collisions
{

    [SkipLocalsInit]
    public static bool IntersectPolygons(Vector2[] verticesA, Vector2[] verticesB,
    out Vector2 normal, out float depth)
    {

        normal = Vector2.Zero;
        depth = float.MaxValue;

        for(int i=0; i< verticesA.Length; i++)
        {
            Vector2 va = verticesA[i];
            int nextIndex = (i+1) % verticesA.Length;
            Vector2 vb = verticesA[nextIndex];

            Vector2 edge = vb - va;
            Vector2 axis = new(-edge.Y, edge.X);

            ProjectVertices(verticesA, axis, out float minA, out float maxA);
            ProjectVertices(verticesB, axis, out float minB, out float maxB);

            if(minA >= maxB || minB >= maxA)
            {
                return false;
            }

            float seperationDistance = Math.Min(maxA - minB, maxB - minA);
            if(seperationDistance < depth)
            {
                depth = seperationDistance;
                normal = axis;
            }

        }

        for(int i=0; i< verticesB.Length; i++)
        {
            Vector2 va = verticesB[i];
            Vector2 vb = verticesB[(i+1) % verticesB.Length];

            Vector2 edge = vb - va;
            Vector2 axis = new(-edge.Y, edge.X);

            ProjectVertices(verticesA, axis, out float minA, out float maxA);
            ProjectVertices(verticesB, axis, out float minB, out float maxB);

            if(minA >= maxB || minB >= maxA)
            {
                return false;
            }

            float seperationDistance = Math.Min(maxA - minB, maxB - minA);
            if(seperationDistance < depth)
            {
                depth = seperationDistance;
                normal = axis;
            }

        }

        depth /= normal.Length();
        normal = Vector2.Normalize(normal);

        Vector2 centerA = FindArithmeticMean(verticesA);
        Vector2 centerB = FindArithmeticMean(verticesB);

        Vector2 direction = centerB - centerA;

        if(Vector2.Dot(direction, normal) < 0f)
        {
            normal *= -1;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector2 FindArithmeticMean(in Vector2[] vertices)
    {
        float sumX = 0;
        float sumY = 0;

        for(int i=0; i<vertices.Length; i++)
        {
            sumX += vertices[i].X;
            sumY += vertices[i].Y;
        }
        return new(sumX, sumY);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ProjectVertices(in Vector2[] vertices, in Vector2 axis, out float min, out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;

        for(int i=0; i<vertices.Length; i++)
        {
            Vector2 v = vertices[i];
            float proj = Vector2.Dot(v, axis);

            if(proj < min){min = proj;}
            if(proj > max){max = proj;}
        }
    }

    public static bool IntersectCircles(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB,
    out Vector2 normal, out float depth)
    {
        normal = Vector2.Zero;
        depth = 0;


        float distance = Vector2.Distance(centerA, centerB);
        float radii = radiusA + radiusB;

        if(distance >= radii)
        {
            return false;
        }

        normal = centerB - centerA;
        normal = Vector2.Normalize(normal);
        depth = radii - distance;

        return true;
    }

}