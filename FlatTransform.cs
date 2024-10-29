
using System.Numerics;

public readonly struct FlatTransform
{
    public readonly float PositionX;
    public readonly float PositionY;
    public readonly float Sin;
    public readonly float Cos;

    public readonly static FlatTransform Zero = new FlatTransform(0, 0, 0);

    public static Vector2 Transform(Vector2 vec, FlatTransform transform)
    {
        float rx = transform.Cos * vec.X - transform.Sin * vec.Y;
        float ry = transform.Sin * vec.X + transform.Cos * vec.Y;

        float tx = rx + transform.PositionX;
        float ty = ry + transform.PositionY;

        return new(tx, ty);
    }

    public FlatTransform(Vector2 position, float angle)
    {
        this.PositionX = position.X;
        this.PositionY = position.Y;
        this.Sin = MathF.Sin(angle);
        this.Cos = MathF.Cos(angle);
    }

    public FlatTransform(float X, float Y, float angle)
    {
        this.PositionX = X;
        this.PositionY = Y;
        this.Sin = MathF.Sin(angle);
        this.Cos = MathF.Cos(angle);
    }

}