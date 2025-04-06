using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockData))]
public class BlockDataEditor : Editor
{
    private PreviewRenderUtility previewRenderer;
    private Material previewMaterial;
    private BlockMeshData previewMeshData;
    Texture2DArray blockAtlas;
    private bool isInitialized;
    private float rotationAmount;
    
    private SerializedProperty typeProperty;
    private SerializedProperty meshDataObject;
    private SerializedProperty defaultColor;
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
        blockAtlas = AssetDatabase.LoadAssetAtPath<Texture2DArray>("Assets/Textures/BlockAtlas.png");
        previewMeshData = AssetDatabase.LoadAssetAtPath<BlockMeshData>("Assets/SO/Mesh Datas/CenteredBlockMesh.asset");
        
        isInitialized = true;
    }

    private void OnEnable()
    {
        rotationAmount = 0;
        typeProperty = serializedObject.FindProperty("Type");
        meshDataObject = serializedObject.FindProperty("MeshData");
        defaultColor = serializedObject.FindProperty("DefaultColor");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (!isInitialized) InitializePreviewRenderer();
        BlockData blockData = (BlockData)target;
    
        EditorGUILayout.PropertyField(typeProperty);
        EditorGUILayout.ObjectField(meshDataObject, typeof(BlockMeshData));
        EditorGUILayout.PropertyField(defaultColor);
    
        EditorGUILayout.Space(10);

        foreach (var dir in Globals.Directions_3D)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(dir.Key.ToString() + " Face");
            GUILayout.FlexibleSpace();
            Texture2D img = new Texture2D(16,16);
            img.SetPixels(blockAtlas.GetPixels(blockData.GetTextureSliceIndex(dir.Key)));
            img.Apply();
            if (GUILayout.Button(img, GUILayout.Width(32), GUILayout.Height(32)))
            {
                BlockTextureSelectionWindow.ShowWindow(blockAtlas,blockData,dir.Key);
            }
            GUILayout.EndHorizontal();
        }
    
        EditorGUILayout.Space(10);
    
        Rect previewRect = GUILayoutUtility.GetRect(250, 250);
        DrawBlockPreview(previewRect, blockData);
        if (previewRect.Contains(Event.current.mousePosition))
        {
            Repaint();
        }
    
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
                meshBuilder.AddFace(previewMeshData.GetFaceData(dir.Key), dir.Key, Vector3Int.zero, blockData.GetTextureSliceIndex(dir.Key), blockData.DefaultColor);
            }
            
            Mesh blockMesh = meshBuilder.Build();

            rotationAmount += 0.5f;
            if (rotationAmount >= 360) rotationAmount -= 360;
            previewRenderer.DrawMesh(blockMesh, Vector3.zero, Quaternion.Euler(0,rotationAmount,0), previewMaterial, 0);
            
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
