
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using Raylib_cs;

Raylib.InitWindow(1000, 1000, "physics engine");

Raylib.SetTargetFPS(0);

FlatWorld world = new();

RandomHelper.addRandomFlatBody(world, 2500);

FlatBody chosenBody = world.bodyList[0];

float dx = 0f;
float dy = 0f;
float speed = 10f;

float currTimeFactor = 10f;
float timeFactor = currTimeFactor;

Vector2 playerVelocity = Vector2.Zero;

Vector2 cameraPosition = Vector2.Zero;

Camera2D camera2D = new()
{
    Target = Vector2.Zero,
    Offset = new(Raylib.GetScreenWidth()/2, Raylib.GetScreenHeight()/2),
    // Offset = Vector2.Zero,
    Rotation = 0f,
    Zoom = 1f
};

Stopwatch timer = new();

while (!Raylib.WindowShouldClose())
{
    timer.Start();
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.BeginMode2D(camera2D);
    DebugDiagnostics.drawMouse(color: Color.Red, camera2D);
    world.Draw();
    rlDrawingEx.DrawCircleVCorrected(chosenBody.position, 2f, Color.Red);
    world.step(Raylib.GetFrameTime() * currTimeFactor);

    if (Raylib.IsKeyDown(KeyboardKey.Up)) { dy += 1f; Math.Clamp(dy, -1f, 1f); }
    if (Raylib.IsKeyDown(KeyboardKey.Down)) { dy -= 1f; Math.Clamp(dy, -1f, 1f); }
    if (Raylib.IsKeyDown(KeyboardKey.Right)) { dx += 1f; Math.Clamp(dx, -1f, 1f); }
    if (Raylib.IsKeyDown(KeyboardKey.Left)) { dx -= 1f; Math.Clamp(dx, -1f, 1f); }
    if(Raylib.IsKeyDown(KeyboardKey.E)){world.bodyList[0].Rotate(-MathF.PI * 0.5f * Raylib.GetFrameTime());}
    if(Raylib.IsKeyDown(KeyboardKey.Q)){world.bodyList[0].Rotate(MathF.PI * 0.5f * Raylib.GetFrameTime());}
    if (Raylib.IsKeyDown(KeyboardKey.W)) {camera2D.Offset += new Vector2(0, 100) * Raylib.GetFrameTime();}
    if (Raylib.IsKeyDown(KeyboardKey.S)) {camera2D.Offset += new Vector2(0, -100) * Raylib.GetFrameTime();}
    if (Raylib.IsKeyDown(KeyboardKey.D)) {camera2D.Offset += new Vector2(-100, 0) * Raylib.GetFrameTime();}
    if (Raylib.IsKeyDown(KeyboardKey.A)) {camera2D.Offset += new Vector2(100, 0) * Raylib.GetFrameTime();}
    if (Raylib.IsKeyDown(KeyboardKey.Space)) {camera2D.Zoom *= 1.1f;}
    if (Raylib.IsKeyDown(KeyboardKey.LeftControl)) {camera2D.Zoom *= 0.9f;}

    if (Raylib.IsKeyReleased(KeyboardKey.LeftShift)) 
    {
        timeFactor = currTimeFactor;
        currTimeFactor = 0f;
    }

    if (Raylib.IsMouseButtonDown(MouseButton.Left))
    {
        Vector2 position = Raylib.GetMousePosition() - camera2D.Offset;
        Vector2 correctedPosition = new(position.X, Raylib.GetScreenHeight() - position.Y);
        chosenBody.MoveTo(correctedPosition);
    }

    if (dx != 0 || dy != 0)
    {
        Vector2 displacement = Vector2.Normalize(new(dx, dy)) * speed * Raylib.GetFrameTime();
        displacement = new(dx, dy);
        chosenBody.linearVelocity = displacement;
    }

    Raylib.EndMode2D();
    Raylib.EndDrawing();
    Raylib.DrawText($"frame time: {timer.ElapsedMilliseconds}\noffset: {camera2D.Offset}\nzoom: {camera2D.Zoom}x", 50, 10, 20, Color.DarkGreen);
    timer.Restart();
}

Raylib.CloseWindow();