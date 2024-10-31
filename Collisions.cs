
using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;

public static class Collisions
{

    public static bool IntersectCirclePolygon(FlatBody circle, FlatBody polygon,
    out Vector2 normal, out float depth)
    {
        normal = Vector2.Zero;
        depth = 0f;

        if(Vector2.Distance(circle.position, polygon.position) > circle.radius + polygon.rectDiameter)
        {
            return false;
        }

        return IntersectCirclePolygon(circle.position, circle.radius,
        polygon.position, polygon.GetTransformedVertices(),
        out normal, out depth);
    }
    public static bool IntersectCirclePolygon(Vector2 circleCenter, float circleRadius,
    Vector2 polygonCenter, Vector2[] vertices,
    out Vector2 normal, out float depth)
    {
        normal = Vector2.Zero;
        depth = float.MaxValue;

        Vector2 axis = Vector2.Zero;
        float axisDepth = 0f;
        float minA, maxA, minB, maxB;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 va = vertices[i];
            Vector2 vb = vertices[(i + 1) % vertices.Length];

            Vector2 edge = vb - va;
            axis = new Vector2(-edge.Y, edge.X);
            axis = Vector2.Normalize(axis);

            Collisions.ProjectVertices(vertices, axis, out minA, out maxA);
            Collisions.ProjectCircle(circleCenter, circleRadius, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth)
            {
                depth = axisDepth;
                normal = axis;
            }
        }

        int cpIndex = Collisions.FindClosestPointOnPolygon(circleCenter, vertices);
        Vector2 cp = vertices[cpIndex];

        axis = cp - circleCenter;
        axis = Vector2.Normalize(axis);

        Collisions.ProjectVertices(vertices, axis, out minA, out maxA);
        Collisions.ProjectCircle(circleCenter, circleRadius, axis, out minB, out maxB);

        if (minA >= maxB || minB >= maxA)
        {
            return false;
        }

        axisDepth = MathF.Min(maxB - minA, maxA - minB);

        if (axisDepth < depth)
        {
            depth = axisDepth;
            normal = axis;
        }

        Vector2 direction = polygonCenter - circleCenter;

        if (Vector2.Dot(direction, normal) < 0f)
        {
            normal = -normal;
        }

        return true;
    }

    public static bool IntersectPolygons(FlatBody bodyA, FlatBody bodyB,
    out Vector2 normal, out float depth)
    {
        Vector2[] verticesA = bodyA.GetTransformedVertices();
        Vector2[] verticesB = bodyB.GetTransformedVertices();

        normal = Vector2.Zero;
        depth = 0;

        if (Vector2.Distance(bodyA.position, bodyB.position) > bodyA.rectDiameter + bodyB.rectDiameter)
        {
            return false;
        }
        // rlDrawingEx.drawLineCorrectedV(bodyA.position, bodyB.position, 1, Color.DarkGreen);
        return IntersectPolygons(verticesA, verticesB, out normal, out depth);
    }
    public static bool IntersectPolygons(Vector2[] verticesA, Vector2[] verticesB,
    out Vector2 normal, out float depth)
    {

        normal = Vector2.Zero;
        depth = float.MaxValue;

        Vector2 centerA = FindArithmeticMean(verticesA);
        Vector2 centerB = FindArithmeticMean(verticesB);

        for (int i = 0; i < verticesA.Length; i++)
        {
            Vector2 va = verticesA[i];
            int nextIndex = (i + 1) % verticesA.Length;
            Vector2 vb = verticesA[nextIndex];

            Vector2 edge = vb - va;
            Vector2 axis = new(-edge.Y, edge.X);

            ProjectVertices(verticesA, axis, out float minA, out float maxA);
            ProjectVertices(verticesB, axis, out float minB, out float maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            float seperationDistance = Math.Min(maxA - minB, maxB - minA);
            if (seperationDistance < depth)
            {
                depth = seperationDistance;
                normal = axis;
            }

        }

        for (int i = 0; i < verticesB.Length; i++)
        {
            Vector2 va = verticesB[i];
            Vector2 vb = verticesB[(i + 1) % verticesB.Length];

            Vector2 edge = vb - va;
            Vector2 axis = new(-edge.Y, edge.X);

            ProjectVertices(verticesA, axis, out float minA, out float maxA);
            ProjectVertices(verticesB, axis, out float minB, out float maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            float seperationDistance = Math.Min(maxA - minB, maxB - minA);
            if (seperationDistance < depth)
            {
                depth = seperationDistance;
                normal = axis;
            }

        }

        depth /= normal.Length();
        normal = Vector2.Normalize(normal);

        Vector2 direction = centerB - centerA;

        if (Vector2.Dot(direction, normal) < 0f)
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

        for (int i = 0; i < vertices.Length; i++)
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

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v = vertices[i];
            float proj = Vector2.Dot(v, axis);

            if (proj < min) { min = proj; }
            if (proj > max) { max = proj; }
        }
    }

    public static void ProjectCircle(Vector2 center, float radius, Vector2 axis, out float min, out float max)
    {

        Vector2 direction = Vector2.Normalize(axis);
        Vector2 directionAndRadius = direction * radius;

        Vector2 p1 = center + directionAndRadius;
        Vector2 p2 = center - directionAndRadius;

        min = Vector2.Dot(p1, axis);
        max = Vector2.Dot(p2, axis);
        if (min > max)
        {
            float t = min;
            min = max;
            max = t;
        }
    }

    public static int FindClosestPointOnPolygon(Vector2 circleCenter, Vector2[] vertices)
    {
        int result = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v = vertices[i];
            float distance = Vector2.Distance(v, circleCenter);
            if (distance < minDistance)
            {
                result = i;
                minDistance = distance;
            }
        }
        return result;
    }

    public static bool IntersectCircles(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB,
    out Vector2 normal, out float depth)
    {
        normal = Vector2.Zero;
        depth = 0;


        float distance = Vector2.Distance(centerA, centerB);
        float radii = radiusA + radiusB;

        if (distance >= radii)
        {
            return false;
        }

        normal = centerB - centerA;
        normal = Vector2.Normalize(normal);
        depth = radii - distance;

        return true;
    }

}