
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using Raylib_cs;

Raylib.InitWindow(1000, 1000, "physics engine");

Raylib.SetTargetFPS(0);

FlatWorld world = new();

RandomHelper.addRandomFlatBody(world, 10000);

float dx = 0f;
float dy = 0f;
float speed = 10f;

Vector2 playerVelocity = Vector2.Zero;

Vector2 cameraPosition = Vector2.Zero;

Stopwatch timer = new();

while (!Raylib.WindowShouldClose())
{
    timer.Start();
    Raylib.ClearBackground(Color.DarkGray);
    Raylib.BeginDrawing();
    DebugDiagnostics.drawMouse(color: Color.Red);
    world.Draw();
    world.step(Raylib.GetFrameTime());

    if (Raylib.IsKeyDown(KeyboardKey.Up)) { dy += 1f; Math.Clamp(dy, -1f, 1f); }
    if (Raylib.IsKeyDown(KeyboardKey.Down)) { dy -= 1f; Math.Clamp(dy, -1f, 1f); }
    if (Raylib.IsKeyDown(KeyboardKey.Right)) { dx += 1f; Math.Clamp(dx, -1f, 1f); }
    if (Raylib.IsKeyDown(KeyboardKey.Left)) { dx -= 1f; Math.Clamp(dx, -1f, 1f); }
    if(Raylib.IsKeyDown(KeyboardKey.E)){world.bodyList[0].Rotate(-MathF.PI * 0.5f * Raylib.GetFrameTime());}
    if(Raylib.IsKeyDown(KeyboardKey.Q)){world.bodyList[0].Rotate(MathF.PI * 0.5f * Raylib.GetFrameTime());}

    if (Raylib.IsMouseButtonDown(MouseButton.Left))
    {
        Vector2 position = Raylib.GetMousePosition();
        Vector2 correctedPosition = new(position.X, Raylib.GetScreenHeight() - position.Y);
        world.bodyList[0].MoveTo(correctedPosition);
    }

    if (dx != 0 || dy != 0)
    {
        Vector2 displacement = Vector2.Normalize(new(dx, dy)) * speed * Raylib.GetFrameTime();
        displacement = new(dx, dy);
        
        world.bodyList[0].linearVelocity = displacement;
    }

    Raylib.EndDrawing();
    Raylib.DrawText($"frame time: {timer.ElapsedMilliseconds}", 100, 100, 20, Color.DarkGreen);
    timer.Restart();
}

Raylib.CloseWindow();