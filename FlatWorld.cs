
using System.Numerics;
using Raylib_cs;

public class FlatWorld
{
    public static readonly float MinBodySize = 0.01f * 0.01f;
    public static readonly float MaxBodySize = 64f * 64f;

    public static readonly float MinDensity = 0.2f;
    public static readonly float MaxDensity = 21.4f;

    public List<FlatBody> bodyList = [];
    public List<Color> colorList = [];

    public Vector2 staticForce = new(0, -1);

    public void Draw()
    {
        for(int i=0; i<bodyList.Count; i++)
        {
            bodyList[i].Draw(colorList[i]);
        }
    }

    public void tick()
    {
        bodyList.ForEach(e => 
        {
            e.Move(staticForce);
            e.Rotate(0.1f);
        });
    }

}