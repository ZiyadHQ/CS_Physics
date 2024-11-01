
using System.Globalization;
using System.Numerics;
using Raylib_cs;

public class FlatWorld
{
    public static readonly float MinBodySize = 0.01f * 0.01f;
    public static readonly float MaxBodySize = 64f * 64f;

    public static readonly float MinDensity = 0.2f;
    public static readonly float MaxDensity = 21.4f;

    Vector2 gravity;
    public List<FlatBody> bodyList = [];
    public List<Color> colorList = [];

    public void Draw()
    {
        for (int i = 0; i < bodyList.Count; i++)
        {
            bodyList[i].Draw(colorList[i]);
        }
    }

    public void addBody(FlatBody flatBody)
    {
        bodyList.Add(flatBody);
    }

    public bool removeBody(FlatBody flatBody)
    {
        return bodyList.Remove(flatBody);
    }

    public bool GetBody(int index, out FlatBody flatBody)
    {
        flatBody = null;
        if (index < 0 || index >= bodyList.Count)
        {
            return false;
        }

        flatBody = bodyList[index];
        return true;
    }

    public int bodycount()
    {
        return bodyList.Count();
    }

    public void step(float dt)
    {

        //movement step
        for(int i=0; i<bodyList.Count; i++)
        {
            bodyList[i].Step(dt);
        }

        // gravity step
        float gravity_sum = 0f;
        for(int i=0; i<bodyList.Count - 1; i++)
        {
            FlatBody bodyA = bodyList[i];
            for(int j=i+1; j<bodyList.Count; j++)
            {
                FlatBody bodyB = bodyList[j];
                float magnitude;
                Vector2 normal;

                CosmicForces.FindGravitationalForce(bodyA, bodyB, out normal, out magnitude);

                gravity_sum += magnitude * 2;

                bodyA.AddForce(normal * magnitude);
                bodyB.AddForce(-normal * magnitude);
            }
        }

        Raylib.DrawText($"total force of gravity: {gravity_sum}", 50, 50, 12, Color.DarkGreen);

        for (int i = 0; i < bodyList.Count - 1; i++)
        {
            FlatBody bodyA = bodyList[i];

            for (int j = i + 1; j < bodyList.Count; j++)
            {
                FlatBody bodyB = bodyList[j];

                if(Collide(bodyA, bodyB, out Vector2 normal, out float depth))
                {
                    bodyA.Move(-normal * depth / 2f);
                    bodyB.Move(normal * depth / 2f);

                    ResolveCollision(bodyA, bodyB, normal, depth);
                }

            }
        }

    }

    public void ResolveCollision(FlatBody bodyA, FlatBody bodyB, Vector2 normal, float depth)
    {
        Vector2 relativeVelocity = bodyB.linearVelocity - bodyA.linearVelocity;

        //restitution
        float e = Math.Min(bodyA.restitution, bodyB.restitution);

        float j = -(1 + e) * Vector2.Dot(relativeVelocity, normal);
        j /= (1f / bodyA.mass) + (1f / bodyB.mass);

        bodyA.linearVelocity -= j / bodyA.mass * normal;
        bodyB.linearVelocity += j / bodyB.mass * normal;
    }

    public bool Collide(FlatBody bodyA, FlatBody bodyB, out Vector2 normal, out float depth)
    {
        normal = Vector2.Zero;
        depth = 0;

        ShapeType shapeTypeA = bodyA.shapeType;
        ShapeType shapeTypeB = bodyB.shapeType;

        if(shapeTypeA is ShapeType.Box)
        {
            if(shapeTypeB is ShapeType.Box)
            {
                return Collisions.IntersectPolygons(bodyA, bodyB, out normal, out depth);
            }
            else if(shapeTypeB is ShapeType.Circle)
            {
                bool result = Collisions.IntersectCirclePolygon(bodyB, bodyA, out normal, out depth);
                normal = -normal;
                return result;
            }
        }
        else if(shapeTypeA is ShapeType.Circle)
        {
            if(shapeTypeB is ShapeType.Box)
            {
                return Collisions.IntersectCirclePolygon(bodyA, bodyB, out normal, out depth);
            }
            else if(shapeTypeB is ShapeType.Circle)
            {
                return Collisions.IntersectCircles(bodyA.position, bodyA.radius, bodyB.position, bodyB.radius, out normal, out depth);
            }
        }

        return false;
    }

}