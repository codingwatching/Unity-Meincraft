using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Block Mesh Data", fileName = "Block Mesh Data")]
public class BlockMeshData : ScriptableObject
{
    public BlockFaceData TopFace;
    public BlockFaceData BottomFace;
    public BlockFaceData RightFace;
    public BlockFaceData LeftFace;
    public BlockFaceData FrontFace;
    public BlockFaceData BackFace;

    public BlockFaceData GetFaceData(Globals.Direction dir)
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
                return new BlockFaceData();
        }
    }
}
[Serializable]
public class BlockFaceData
{
    public Vector3[] Vertices;
}