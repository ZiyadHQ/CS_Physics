
using System.Diagnostics;
using System.Numerics;
using System.Text;
using Raylib_cs;

Raylib.InitWindow(1000, 1000, "physics engine");

Raylib.SetTargetFPS(0);

FlatWorld world = new();

RandomHelper.addRandomFlatBody(world, 100);

float dx = 0f;
float dy = 0f;
float speed = 100f;

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
    // world.tick();

    if (Raylib.IsKeyDown(KeyboardKey.Up)) { dy += 1f; Math.Clamp(dy, -20f, 10f); }
    if (Raylib.IsKeyDown(KeyboardKey.Down)) { dy -= 1f; Math.Clamp(dy, -20f, 10f); }
    if (Raylib.IsKeyDown(KeyboardKey.Right)) { dx += 1f; Math.Clamp(dx, -20f, 10f); }
    if (Raylib.IsKeyDown(KeyboardKey.Left)) { dx -= 1f; Math.Clamp(dx, -20f, 10f); }

    if(Raylib.IsMouseButtonDown(MouseButton.Left))
    {
        Vector2 position = Raylib.GetMousePosition();
        Vector2 correctedPosition = new(position.X, Raylib.GetScreenHeight() - position.Y);
        world.bodyList[0].MoveTo(correctedPosition);
    }

    if (dx != 0 || dy != 0)
    {
        Vector2 displacement = Vector2.Normalize(new(dx, dy)) * speed * Raylib.GetFrameTime();
        displacement = new(dx, dy);
        world.bodyList[0].MoveTo(displacement);
        float vecToAngle = MathF.Atan2(dy, dx);
        world.bodyList[0].RotateTo(vecToAngle);
    }

    for(int i=0; i<world.bodyList.Count; i++)
    {
        // world.bodyList[i].Rotate(MathF.PI/2f * Raylib.GetFrameTime());
        world.bodyList[i].Move(new Vector2(1f, 0));
    }

    for (int i = 0; i < world.bodyList.Count - 1; i++)
    {
        FlatBody bodyA = world.bodyList[i];
        for (int j = i + 1; j < world.bodyList.Count; j++)
        {
            FlatBody bodyB = world.bodyList[j];

            // if(collisions.IntersectCircles(bodyA.position, bodyA.radius, bodyB.position, bodyB.radius,
            //  out Vector2 normal, out float depth))
            // {
            //     bodyA.Move(-normal * depth/2);
            //     bodyB.Move(normal * depth/2);
            // }
            if(Collisions.IntersectPolygons(bodyA.GetTransformedVertices(), bodyB.GetTransformedVertices(), out Vector2 normal, out float depth))
            {

                bodyA.Move(-normal * depth/2f);
                bodyB.Move(normal * depth/2f);

                // Vector2 position = bodyA.position;
                // Vector2 correctedPosition = new(position.X, Raylib.GetScreenHeight() - position.Y);
                // Raylib.DrawCircleV(correctedPosition, 5f, Color.DarkGreen);

                // position = bodyB.position;
                // correctedPosition = new(position.X, Raylib.GetScreenHeight() - position.Y);
                // Raylib.DrawCircleV(correctedPosition, 5f, Color.DarkGreen);
            }
        }
    }

    Raylib.EndDrawing();
    Raylib.DrawText($"frame time: {timer.ElapsedMilliseconds}", 100, 100, 20, Color.DarkGreen);
    timer.Restart();
}

Raylib.CloseWindow();