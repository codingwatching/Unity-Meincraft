using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Newtonsoft.Json;

public class TextureAtlasGenerator : EditorWindow
{
    int textureSize = 16;
    int gridSizeInBlocksX = 8;
    int gridSizeInBlocksY = 8;
    string resourcePath = "Textures/blocks";

    [MenuItem("Window/Texture Atlas Generator")]
    public static void ShowWindow()
    {
        TextureAtlasGenerator window = GetWindow<TextureAtlasGenerator>("Texture Atlas Generator");
        window.minSize = new Vector2(400, 400);
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Resourcess Folder Path (Resources/x)");
        resourcePath = EditorGUILayout.TextField(resourcePath);
        EditorGUILayout.EndHorizontal();
        
        textureSize = EditorGUILayout.IntField(new GUIContent("Block Size") ,textureSize);
        gridSizeInBlocksX = EditorGUILayout.IntField(new GUIContent("Grid Size In Blocks X") , gridSizeInBlocksX);
        gridSizeInBlocksY = EditorGUILayout.IntField(new GUIContent("Grid Size In Blocks Y") , gridSizeInBlocksY);
        
        if (GUILayout.Button("Generate"))
        {
            string path = EditorUtility.SaveFilePanel("Select Output Directory", "Assets/", "Atlas", ".png");
            if (string.IsNullOrEmpty(path)) return;
            GenerateAtlas(path);
            Close();
        }
    }
    void GenerateAtlas(string outputPath)
    {
        Object[] rawTextures = Resources.LoadAll(resourcePath, typeof(Texture2D));

        int atlasResolutionX  = textureSize * gridSizeInBlocksX;
        int atlasResolutionY  = textureSize * gridSizeInBlocksY;
        
        Texture2D[] albedos = new Texture2D[rawTextures.Length];
        for (int i = 0; i < rawTextures.Length; i++)
        {
            albedos[i] = ((Texture2D)rawTextures[i]);
        }
        
        Color[] albedoPixels = new Color[atlasResolutionX * atlasResolutionY];

        for (int y = 0; y < atlasResolutionY; y++)
        {
            for (int x = 0; x < atlasResolutionX; x++)
            {
                int currentBlockX = x / textureSize;
                int currentBlockY = y / textureSize;

                int index = currentBlockY * gridSizeInBlocksY + currentBlockX;

                if(index < albedos.Length)
                {
                    albedoPixels[(atlasResolutionY - y - 1) * atlasResolutionX + x] = albedos[index].GetPixel(x, textureSize - y - 1);
                }
                else
                {
                    albedoPixels[(atlasResolutionY - y - 1) * atlasResolutionX + x] = new Color(0,0,0,0);
                }
            }
        }
        Texture2D atlas = new Texture2D(atlasResolutionX, atlasResolutionY);

        atlas.SetPixels(albedoPixels);
        atlas.Apply();
        byte[] bytes = atlas.EncodeToPNG();
        File.WriteAllBytes(outputPath, bytes);
    }
}

