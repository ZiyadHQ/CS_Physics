
using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;

public static class DebugDiagnostics
{

    public static void drawMouse(Color color, Camera2D camera2D, float size = 3f)
    {
        Vector2 mouse = Raylib.GetMousePosition();
        mouse -= camera2D.Offset;
        Raylib.DrawCircleV(mouse, size, color);
    }

}