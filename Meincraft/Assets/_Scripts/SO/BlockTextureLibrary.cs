using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Block Texture Library", fileName ="Block Texture Library")]
public class BlockTextureLibrary : ScriptableObject
{
    public BlockData[] Data;
}
[Serializable]
public class BlockData
{
    public BlockType BlockType;
    public bool IsSolid = true;

    [Space(10), Header("Textures")]
    public byte TopFace;
    public byte BottomFace;
    public byte RightFace;
    public byte LeftFace;
    public byte FrontFace;
    public byte BackFace;
}