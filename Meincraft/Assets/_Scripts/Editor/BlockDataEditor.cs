using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockData))]
public class BlockDataEditor : Editor
{
    private PreviewRenderUtility previewRenderer;
    private Material previewMaterial;
    private bool isInitialized;
    
    void InitializePreviewRenderer()
    {
        if (isInitialized) return;
        
        previewRenderer = new PreviewRenderUtility();
        previewRenderer.camera.nearClipPlane = 0.001f;
        previewRenderer.camera.farClipPlane = 100f;
        previewRenderer.camera.clearFlags = CameraClearFlags.Skybox;
        previewRenderer.camera.transform.position = new Vector3(0, 5f, -10);
        previewRenderer.camera.transform.LookAt(Vector3.zero, Vector3.up);
        
        previewMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Block.mat");
        
        isInitialized = true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (!isInitialized) InitializePreviewRenderer();
        BlockData blockData = (BlockData) target;
        
        
        
        GUILayout.BeginVertical();
        blockData.Type = (BlockType)EditorGUILayout.EnumPopup("Type", blockData.Type);
        blockData.MeshData = EditorGUILayout.ObjectField("Mesh Data", blockData.MeshData, typeof(BlockMeshData), false) as BlockMeshData;
        blockData.DefaultColor = EditorGUILayout.ColorField("Default Color", blockData.DefaultColor);

        DrawBlockPreview(new Rect(100, 100, 250, 250), blockData);
        GUILayout.EndVertical();
        
        
        
        // Force repaint to animate rotation
        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBlockPreview(Rect previewRect, BlockData blockData)
    {
                if (previewRenderer == null) return;
        
        // Start the preview
        previewRenderer.BeginPreview(previewRect, GUIStyle.none);
        
        if (blockData.MeshData != null)
        {
            MeshBuilder meshBuilder = new MeshBuilder();
            
            foreach (var dir in Globals.Directions_3D)
            {
                meshBuilder.AddFace(blockData.MeshData.GetFaceData(dir.Key), dir.Key, Vector3Int.zero, blockData.GetTextureSliceIndex(dir.Key), blockData.DefaultColor);
            }
            
            Mesh blockMesh = meshBuilder.Build();
            
            previewRenderer.DrawMesh(blockMesh, -Vector3.one/2, Quaternion.Euler(0,45,0), previewMaterial, 0);
                
            previewRenderer.camera.Render();
        }
        
        GUI.DrawTexture(previewRect, previewRenderer.EndPreview());
        
        Rect labelRect = new Rect(previewRect.x, previewRect.y + previewRect.height - 20, previewRect.width, 20);
        GUI.Label(labelRect, "Block Preview", EditorStyles.centeredGreyMiniLabel);
    }
    
    // Clean up the preview renderer when the drawer is destroyed
    ~BlockDataEditor()
    {
        if (previewRenderer != null)
        {
            previewRenderer.Cleanup();
            previewRenderer = null;
            isInitialized = false;
        }
    }

    private void OnDisable()
    {
        if (previewRenderer != null)
        {
            previewRenderer.Cleanup();
            previewRenderer = null;
            isInitialized = false;
        }
    }
}
