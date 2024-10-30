
using System.Numerics;
using Raylib_cs;

public enum ShapeType
{
    Circle = 0,
    Box = 1
}

public sealed class FlatBody
{
    public Vector2 position;
    private Vector2 linearVelocity;
    private float angle;
    private float angularVelocity;

    public float mass;
    public float density;
    public float restitution;
    public float area;

    public bool isStatic;

    public float radius;
    public float width;
    public float height;
    public readonly float rectDiameter;

    public readonly Vector2[] vertices;
    public readonly int[] triangles;
    public Vector2[] transformedVertices;

    private bool transformUpdateRequired;

    public ShapeType shapeType;

    private FlatBody(Vector2 position,
    float mass, float density, float restitution, float area,
    bool isStatic,
    float radius, float width, float height,
    ShapeType shapeType
    )
    {
        this.position = position;
        this.linearVelocity = Vector2.Zero;
        this.angle = 0;
        this.angularVelocity = 0;

        this.mass = mass;
        this.density = density;
        this.restitution = restitution;
        this.area = area;
        
        this.isStatic = isStatic;
        
        this.radius = radius;
        this.width = width;
        this.height = height;

        this.shapeType = shapeType;

        if(this.shapeType == ShapeType.Box)
        {
            this.rectDiameter = MathF.Sqrt(this.width*this.width + this.height*this.height) * 0.5f;
        }

        if(shapeType is ShapeType.Box)
        {
            this.vertices = CreateBoxVerices(width, height);
            this.triangles = CreateBoxTriangles();
            this.transformedVertices = new Vector2[this.vertices.Length];
        }
        else
        {
            this.vertices = null;
            this.triangles = null;
            this.transformedVertices = null;
        }

        this.transformUpdateRequired = true;
    }

    private static Vector2[] CreateBoxVerices(float width, float height)
    {
        float left = -width / 2f;
        float right = left + width;
        float bottom = -height / 2f;
        float top = bottom + height;

        Vector2[] vertices = new Vector2[4];
        vertices[0] = new(left, top);
        vertices[1] = new(right, top);
        vertices[2] = new(right, bottom);
        vertices[3] = new(left, bottom);

        return vertices;
    }

    private static int[] CreateBoxTriangles()
    {
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 3;
        triangles[4] = 4;
        triangles[5] = 5;
        return triangles;
    }

    public Vector2[] GetTransformedVertices()
    {

        if(transformUpdateRequired)
        {
            FlatTransform transform = new(position, angle);

            for(int i=0; i<this.vertices.Length; i++)
            {
                Vector2 v = this.vertices[i];
                this.transformedVertices[i] = FlatTransform.Transform(v, transform);
            }

            transformUpdateRequired = false;
        }

        return this.transformedVertices;
    }

    public static bool CreateCircleBody(float radius, Vector2 position, float density, bool isStatic, float restitution,
    out FlatBody body, out String errorMessage)
    {
        body = null;
        errorMessage = String.Empty;

        float area = radius * radius * MathF.PI; //m^3

        if(area < FlatWorld.MinBodySize)
        {
            errorMessage = $"circle area too small, min circle area is: {FlatWorld.MinBodySize}";
            return false;
        }

        if(area > FlatWorld.MaxBodySize)
        {
            errorMessage = $"circle area too large, max circle area is: {FlatWorld.MaxBodySize}";
            return false;
        }

        if(density < FlatWorld.MinDensity)
        {
            errorMessage = $"Circle density too low, min density is: {FlatWorld.MinDensity}";
            return false;
        }

        if(density > FlatWorld.MaxDensity)
        {
            errorMessage = $"Circle density too high, max density is: {FlatWorld.MaxDensity}";
            return false;
        }

        restitution = Math.Clamp(restitution, 0f, 1f);

        float mass = area * density; //Kg

        body = new FlatBody(position, mass, density, restitution, area, isStatic, radius, 0f, 0f, ShapeType.Circle);
        return true;
    }

    public static bool CreateBoxBody(float width, float height, Vector2 position, float density, bool isStatic, float restitution,
    out FlatBody body, out String errorMessage)
    {
        body = null;
        errorMessage = String.Empty;

        float area = width * height; //m^3

        if(area < FlatWorld.MinBodySize)
        {
            errorMessage = $"Box area too small, min circle area is: {FlatWorld.MinBodySize}";
            return false;
        }

        if(area > FlatWorld.MaxBodySize)
        {
            errorMessage = $"Box area too large, max circle area is: {FlatWorld.MaxBodySize}";
            return false;
        }

        if(density < FlatWorld.MinDensity)
        {
            errorMessage = $"Box density too low, min density is: {FlatWorld.MinDensity}";
            return false;
        }

        if(density > FlatWorld.MaxDensity)
        {
            errorMessage = $"Box density too high, max density is: {FlatWorld.MaxDensity}";
            return false;
        }

        restitution = Math.Clamp(restitution, 0f, 1f);

        float mass = area * density; //Kg

        body = new FlatBody(position, mass, density, restitution, area, isStatic, 0f, width, height, ShapeType.Box);
        return true;
    }
    public static bool CreateBoxBodyAngular(float width, float height, Vector2 position, float density, bool isStatic, float restitution,
    out FlatBody body, out String errorMessage, float angle)
    {
        body = null;
        errorMessage = String.Empty;
        bool success = CreateBoxBody(width, height, position, density, isStatic, restitution, out body, out errorMessage);
        if(success)
        {
            body.angle = angle;
            return true;
        }
        return success;
    } 



    public void Move(Vector2 displacement)
    {
        position += displacement;
        transformUpdateRequired = true;
    }

    public void MoveTo(Vector2 position)
    {
        this.position = position;
        transformUpdateRequired = true;
    }

    public void Rotate(float angularDisplacement)
    {
        angle += angularDisplacement;
        transformUpdateRequired = true;
    }

    public void RotateTo(float angle)
    {
        this.angle = angle;
        transformUpdateRequired = true;
    }

    public void Draw(Color color)
    {
        Vector2 correctedPosition = new(position.X, Raylib.GetScreenHeight() - position.Y);
        if(shapeType == ShapeType.Circle)
        {
            //draw the shape
            Raylib.DrawCircleLinesV(correctedPosition, radius, color);

            //draw the angle line
            Vector2 normalVec = new(MathF.Cos(angle), MathF.Sin(angle));
            Raylib.DrawLineEx(correctedPosition, correctedPosition + normalVec * radius, 1, Color.DarkGreen);
        }
        else
        {
            //rlDrawingEx.DrawRectangleAngularV(correctedPosition, width, height, angle, color);

            rlDrawingEx.DrawVertices(GetTransformedVertices(), 1, color);

            //draw the angle line
            Vector2 normalVec = new(MathF.Cos(angle), MathF.Sin(angle));
            Raylib.DrawLineEx(correctedPosition, correctedPosition + normalVec * height * 0.5f, 1, Color.DarkGreen);
        }
    }

}