using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;

public static class rlDrawingEx
{

    public static void DrawCircleVCorrected(Vector2 position, float radius, Color color)
    {
        Vector2 correctedPosition = new(position.X, Raylib.GetScreenHeight() - position.Y);
        Raylib.DrawCircleV(correctedPosition, radius, color);
        // Raylib.DrawCircle(10, 100, radius, color);
    }

    public static void DrawRectangleAngularV(Vector2 position, float width, float height, float angle, Color color)
    {
        int posX = (int)position.X;
        int posY = (int)position.Y;
        DrawRectangleAngular(posX, posY, width, height, angle, color);
    }

    public static void drawLineCorrectedV(Vector2 startPos, Vector2 endPos, float thickness, Color color)
    {
        Raylib.DrawLineEx(correctVector(startPos), correctVector(endPos), thickness, color);
    }

    public static void DrawRectangleAngular(int posX, int posY, float width, float height, float angle, Color color)
    {
        Vector2 normalDirection = new(MathF.Cos(angle), MathF.Sin(angle));

        Vector2 position = new(posX, posY);
        Vector2 perpendicularNormal = new(normalDirection.Y, -normalDirection.X);
        Vector2 bottomDown = position - (normalDirection * height)/2 + (perpendicularNormal * width)/2;
        Vector2 topRight = position - (perpendicularNormal * width)/2 + (normalDirection * height)/2;

        Raylib.DrawLineEx(bottomDown, bottomDown + (normalDirection * height), 1, color);
        Raylib.DrawLineEx(bottomDown, bottomDown - (perpendicularNormal * width), 1, color);
        Raylib.DrawLineEx(topRight, topRight - (normalDirection * height), 1, color);
        Raylib.DrawLineEx(topRight, topRight + (perpendicularNormal * width), 1, color);
    }

    public static void DrawVertices(Vector2[] vertices, float width, Color color)
    {
        for(int i=0; i<vertices.Length; i++)
        {
            Vector2 va = vertices[i];
            Vector2 vb = vertices[(i+1) % vertices.Length];

            va = correctVector(va);
            vb = correctVector(vb);

            Raylib.DrawLineEx(va, vb, width, color);
            Raylib.DrawCircleV(va, 2f, Color.Red);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 correctVector(Vector2 position)
    {
        return new(position.X, Raylib.GetScreenHeight() - position.Y);
    }

}