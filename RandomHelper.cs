
using Raylib_cs;
using System.Numerics;

public static class RandomHelper
{
    private static readonly Random random = new();

    private static readonly float minRadius = MathF.Sqrt(FlatWorld.MinBodySize/MathF.PI);
    private static readonly float maxRadius = MathF.Sqrt(FlatWorld.MaxBodySize/MathF.PI);

    public static Vector2 randomVector2(float maxX, float maxY)
    {
        return new(random.NextSingle() * maxX, random.NextSingle() * maxY);
    }

    public static void genRandomBox(out float width, out float height)
    {
        float maxSide = MathF.Sqrt(FlatWorld.MaxBodySize);
        width = maxRadius * random.NextSingle();
        // height = maxRadius * random.NextSingle();
        
        width = Math.Clamp(width, 50f, 100f);

        height = width;
    }

    public static void addRandomFlatBody(FlatWorld world, int count)
    {
        for (int i = 0; i < count; i++)
        {
            FlatBody body;
            if (random.Next(1) == 0)
            {
                String message = String.Empty;
                FlatBody.CreateCircleBody
                (
                    Math.Clamp(random.NextSingle() * maxRadius, minRadius, maxRadius),
                    randomVector2(count * 10, count * 10),
                    // Math.Clamp(random.NextSingle() * FlatWorld.MaxDensity, FlatWorld.MinDensity, FlatWorld.MaxDensity),
                    FlatWorld.MaxDensity,
                    false,
                    // random.NextSingle(),
                    0.01f,
                    out body,
                    out message
                );
            }
            else
            {
                String message = String.Empty;

                float width;
                float height;
                genRandomBox(out width, out height);

                FlatBody.CreateBoxBodyAngular
                (
                    width,
                    height,
                    randomVector2(500, 500),
                    Math.Clamp(random.NextSingle() * FlatWorld.MaxDensity, FlatWorld.MinDensity, FlatWorld.MaxDensity),
                    false,
                    random.NextSingle(),
                    out body,
                    out message,
                    // random.NextSingle() * 2 * MathF.PI
                    0
                );
            }
            Color randomColor = getRandomColor();
            world.colorList.Add(randomColor);
            world.bodyList.Add(body);
        }
    }

    public static Color getRandomColor()
    {
        int R = (int)(255f * random.NextSingle());
        int G = (int)(255f * random.NextSingle());
        int B = (int)(255f * random.NextSingle());

        return new Color(R, G, B, 255);

    }
}