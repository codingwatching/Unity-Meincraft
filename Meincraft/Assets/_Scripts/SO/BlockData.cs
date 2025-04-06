using UnityEngine;

[CreateAssetMenu(menuName ="Block Data", fileName ="Block Data")]
public class BlockData : ScriptableObject
{
    public BlockData(BlockType t)
    {
        this.Type = t;
    }

    public override bool Equals(object obj)
    {
        if (obj is BlockData other)
        {
            return other.Type == this.Type;
        }
        if (obj is BlockType t)
        {
            return t == this.Type;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }

    public BlockType Type = 0;
    public bool IsSolid = true;

    [Space(10),Header("Mesh Data")]
    public BlockMeshData MeshData;
    public Color DefaultColor = Color.white;
    
    [Space(10), Header("Textures")]
    public int TopFace;
    public int BottomFace;
    public int RightFace;
    public int LeftFace;
    public int FrontFace;
    public int BackFace;
    public int GetTextureSliceIndex(Globals.Direction dir)
    {
        switch (dir)
        {
            case Globals.Direction.UP:
                return TopFace;
            case Globals.Direction.DOWN:
                return BottomFace;
            case Globals.Direction.FRONT:
                return FrontFace;
            case Globals.Direction.BACK:
                return BackFace;
            case Globals.Direction.LEFT:
                return LeftFace;
            case Globals.Direction.RIGHT:
                return RightFace;
            default:
                return 0;
        }
    }
}