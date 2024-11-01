
using System.Numerics;
using System.Text;

public static class CosmicForces
{

    //gravitational constant
    public static readonly float G = 6.6743e5f;

    public static void FindGravitationalForce(FlatBody bodyA, FlatBody bodyB, out Vector2 normal, out float magnitude)
    {
        float distance = Vector2.DistanceSquared(bodyA.position, bodyB.position);

        //F = (m1 * m2 / distance^2) * G
        magnitude = bodyA.mass * bodyB.mass / distance;
        magnitude *= CosmicForces.G;

        //direction bodyB to bodyA
        normal = bodyB.position - bodyA.position;
        normal = Vector2.Normalize(normal);
    }

}