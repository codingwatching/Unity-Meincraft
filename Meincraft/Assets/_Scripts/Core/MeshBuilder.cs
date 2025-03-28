using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct MeshBuilderSettings
{
    public int MaxVertices;
    public int MaxTriangles;
    public bool UseTransparency;
    public Color DefaultColor;
}

public class MeshBuilder
{
    private List<Vector3> _vertices;
    private List<int> _triangles;
    private List<Vector3> _normals;
    private List<Color> _colors;
    private List<Vector3> _uvs;
    
    private int _vertexCount;
    private int _triangleCount;
    private int _uvCount;
    
    private readonly Dictionary<Globals.Direction, Action<int,int,int,int>> _directionActions;
    
    public MeshBuilder(MeshBuilderSettings settings)
    {
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
        _normals = new List<Vector3>();
        _colors = new List<Color>();
        _uvs = new List<Vector3>();
     
        _directionActions = new Dictionary<Globals.Direction, Action<int,int,int,int>>
        {
            { Globals.Direction.FRONT, FrontFace },
            { Globals.Direction.UP, TopFace },
            { Globals.Direction.RIGHT, RightFace },
            { Globals.Direction.BACK, BackFace },
            { Globals.Direction.DOWN, BottomFace },
            { Globals.Direction.LEFT, LeftFace }
        };     
    }

    public Mesh Build()
    {
        Mesh mesh = new Mesh();
        
        if (_triangles == null || _triangles.Count == 0)
        {
            //Dummy
            mesh.subMeshCount = 2;
            
            mesh.SetVertices(new List<Vector3> { Vector3.zero, Vector3.zero, Vector3.zero });
            mesh.SetTriangles(new int[] { 0, 1, 2 }, 1);
        }
        else
        {
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_triangles, 0);
        }
        mesh.SetUVs(0, _uvs);
        mesh.SetNormals(_normals);
        mesh.SetColors(_colors);
        
        return mesh;
    }

    public void AddFace(Globals.Direction dir, Vector3Int position, int textureSliceIndex)
    {
        _directionActions[dir].Invoke(position.x, position.y, position.z, textureSliceIndex);
    } 
    
    void BackFace(int x, int y, int z, int textureSliceIndex)
    {
            
        _vertices.Add(new Vector3(x + 1, y, z));
        _vertices.Add(new Vector3(x, y, z));
        _vertices.Add(new Vector3(x, y + 1, z));
        _vertices.Add(new Vector3(x + 1, y + 1, z));

        _normals.Add(Vector3.back);
        _normals.Add(Vector3.back);
        _normals.Add(Vector3.back);
        _normals.Add(Vector3.back);
        
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        
        AddQuadTriangles();
        AddUV(textureSliceIndex);
    }
    void FrontFace(int x, int y, int z, int textureSliceIndex)
    {
        _vertices.Add(new Vector3(x, y, z + 1));
        _vertices.Add(new Vector3(x + 1, y, z + 1));
        _vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        _vertices.Add(new Vector3(x, y + 1, z + 1));
        
        _normals.Add(Vector3.forward);
        _normals.Add(Vector3.forward);
        _normals.Add(Vector3.forward);
        _normals.Add(Vector3.forward);
        
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        
        AddQuadTriangles();
        AddUV(textureSliceIndex);
    }
    void TopFace(int x, int y, int z, int textureSliceIndex)
    {
        _vertices.Add(new Vector3(x, y + 1, z));
        _vertices.Add(new Vector3(x, y + 1, z + 1));
        _vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        _vertices.Add(new Vector3(x + 1, y + 1, z));

        _normals.Add(Vector3.up);
        _normals.Add(Vector3.up);
        _normals.Add(Vector3.up);
        _normals.Add(Vector3.up);
        
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        
        AddQuadTriangles();
        AddUV(textureSliceIndex);
    }
    void BottomFace(int x, int y, int z, int textureSliceIndex)
    {
        _vertices.Add(new Vector3(x, y, z));
        _vertices.Add(new Vector3(x + 1, y, z));
        _vertices.Add(new Vector3(x + 1, y, z + 1));
        _vertices.Add(new Vector3(x, y, z + 1));

        _normals.Add(Vector3.down);
        _normals.Add(Vector3.down);
        _normals.Add(Vector3.down);
        _normals.Add(Vector3.down);
        
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        
        AddQuadTriangles();
        AddUV(textureSliceIndex);
    }
    void LeftFace(int x, int y, int z, int textureSliceIndex)
    {
        _vertices.Add(new Vector3(x, y, z));
        _vertices.Add(new Vector3(x, y, z + 1));
        _vertices.Add(new Vector3(x, y + 1, z + 1));
        _vertices.Add(new Vector3(x, y + 1, z));

        _normals.Add(Vector3.left);
        _normals.Add(Vector3.left);
        _normals.Add(Vector3.left);
        _normals.Add(Vector3.left);
        
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        
        AddQuadTriangles();
        AddUV(textureSliceIndex);
    }
    void RightFace(int x, int y, int z, int textureSliceIndex)
    {
        _vertices.Add(new Vector3(x + 1, y, z + 1));
        _vertices.Add(new Vector3(x + 1, y, z));
        _vertices.Add(new Vector3(x + 1, y + 1, z));
        _vertices.Add(new Vector3(x + 1, y + 1, z + 1));

        _normals.Add(Vector3.right);
        _normals.Add(Vector3.right);
        _normals.Add(Vector3.right);
        _normals.Add(Vector3.right);
        
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        _colors.Add(Color.white);
        
        AddQuadTriangles();
        AddUV(textureSliceIndex);
    }
    public void AddQuadTriangles()
    {
        int vertCount = _vertices.Count;
        // First triangle
        _triangles.Add(vertCount - 4);
        _triangles.Add(vertCount - 3);
        _triangles.Add(vertCount - 2);

        // Second triangle
        _triangles.Add(vertCount - 4);
        _triangles.Add(vertCount - 2);
        _triangles.Add(vertCount - 1);
    }
    public void AddUV(int textureSliceIndex)
    {
        _uvs.AddRange(new Vector3[]
        {
            new Vector3(0.0f, 0.0f, textureSliceIndex),
            new Vector3(1.0f, 0.0f, textureSliceIndex),
            new Vector3(1.0f, 1.0f, textureSliceIndex),
            new Vector3(0.0f, 1.0f, textureSliceIndex)
        });
    }
    public void Clear()
    {
        _vertices.Clear();
        _triangles.Clear();
        _normals.Clear();
        _colors.Clear();
        _uvs.Clear();
    }
}