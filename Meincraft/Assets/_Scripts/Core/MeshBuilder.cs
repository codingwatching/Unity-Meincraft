using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
    
    public MeshBuilder()
    {
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
        _normals = new List<Vector3>();
        _colors = new List<Color>();
        _uvs = new List<Vector3>();
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

    public void AddFace(BlockFaceData faceData, Globals.Direction dir, Vector3Int position, int textureSliceIndex)
    {
        for (int i = 0; i < faceData.Vertices.Length; i++)
        {
            _vertices.Add(position + faceData.Vertices[i]);
            _normals.Add(Globals.Directions_3D[dir]);
            _colors.Add(faceData.Colors[i]);
        }
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