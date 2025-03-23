using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Block Texture Library", fileName ="Block Texture Library")]
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

    [Space(10), Header("Textures")]
    public byte TopFace;
    public byte BottomFace;
    public byte RightFace;
    public byte LeftFace;
    public byte FrontFace;
    public byte BackFace;
}