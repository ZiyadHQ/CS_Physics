
using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;

public static class DebugDiagnostics
{

    public static void drawMouse(Color color, float size = 3f)
    {
        Vector2 mouse = Raylib.GetMousePosition();
        Raylib.DrawCircle((int)mouse.X, (int)mouse.Y, size, color);
    }

}