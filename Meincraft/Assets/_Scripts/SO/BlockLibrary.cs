using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Block Library", fileName ="Block Library")]
public class BlockLibrary : ScriptableObject
{
    [SerializeField] BlockData[] data;

    public BlockData this[byte idx] => data[idx];
}
[Serializable]
public class BlockData
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

    public BlockType Type;
    public bool IsSolid = true;

    [Space(10),Header("Mesh Data")]
    public BlockMeshData MeshData;
    
    [Space(10), Header("Textures")]
    public byte TopFace;
    public byte BottomFace;
    public byte RightFace;
    public byte LeftFace;
    public byte FrontFace;
    public byte BackFace;
    public byte GetTextureSliceIndex(Globals.Direction dir)
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