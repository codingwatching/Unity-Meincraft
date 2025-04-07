using System;
using Unity.Android.Gradle;
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
    private Mesh blockMesh;

    private Vector2 lastMousePosition;
    private bool isDragging;
    private bool isMouseOver;
    private Vector2 cameraOrbit = new Vector2(0f, 45f);
    private float cameraOrbitSpeed = 1f;
    private float cameraDistance = 3f;
    
    private SerializedProperty typeProperty;
    private SerializedProperty isSolidProperty;
    private SerializedProperty meshDataObject;
    private SerializedProperty defaultColor;
    void InitializePreviewRenderer()
    {
        if (isInitialized) return;
        BlockData blockData = (BlockData)target;
        
        previewRenderer = new PreviewRenderUtility();
        previewRenderer.camera.nearClipPlane = 0.001f;
        previewRenderer.camera.farClipPlane = 100f;
        previewRenderer.camera.clearFlags = CameraClearFlags.Skybox;
        previewRenderer.camera.fieldOfView = 50;
        UpdateCameraPosition();
        
        if(blockData.IsSolid) previewMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Block.mat");
        else previewMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Water.mat");
        blockAtlas = AssetDatabase.LoadAssetAtPath<Texture2DArray>("Assets/Textures/BlockAtlas.png");
        previewMeshData = AssetDatabase.LoadAssetAtPath<BlockMeshData>("Assets/SO/Mesh Datas/CenteredBlockMesh.asset");
        
        MeshBuilder meshBuilder = new MeshBuilder();
        foreach (var dir in Globals.Directions_3D)
        {
            meshBuilder.AddFace(previewMeshData.GetFaceData(dir.Key), dir.Key, Vector3Int.zero, blockData.GetTextureSliceIndex(dir.Key), blockData.DefaultColor);
        }
        blockMesh = meshBuilder.Build();
        
        isInitialized = true;
    }

    private void OnEnable()
    {
        typeProperty = serializedObject.FindProperty("Type");
        isSolidProperty = serializedObject.FindProperty("IsSolid");
        meshDataObject = serializedObject.FindProperty("MeshData");
        defaultColor = serializedObject.FindProperty("DefaultColor");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (!isInitialized) InitializePreviewRenderer();
        BlockData blockData = (BlockData)target;
    
        EditorGUILayout.PropertyField(typeProperty);
        EditorGUILayout.PropertyField(isSolidProperty);
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
        
        Event currentEvent = Event.current;
        isMouseOver = previewRect.Contains(currentEvent.mousePosition);

        if (isMouseOver)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    isDragging = true;
                    lastMousePosition = currentEvent.mousePosition;
                    break;
                case EventType.MouseUp:
                    isDragging = false;
                    break;
            }
            if (isDragging)
            {
                float deltaX = currentEvent.mousePosition.x - lastMousePosition.x;
                float deltaY = currentEvent.mousePosition.y - lastMousePosition.y;
                        
                cameraOrbit.y += deltaX * cameraOrbitSpeed;
                cameraOrbit.x += deltaY * cameraOrbitSpeed;
            
                cameraOrbit.x = Mathf.Clamp(cameraOrbit.x, -85f, 85f);
                        
                lastMousePosition = currentEvent.mousePosition;
                EditorGUIUtility.AddCursorRect(previewRect, MouseCursor.Pan);
                UpdateCameraPosition();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }  
    
    private void DrawBlockPreview(Rect previewRect, BlockData blockData)
    {
        if (previewRenderer == null) return;
        
        // Start the preview
        previewRenderer.BeginPreview(previewRect, GUIStyle.none);
        previewRenderer.DrawMesh(blockMesh, Vector3.zero, Quaternion.identity, previewMaterial, 0);
            
        previewRenderer.camera.Render();
        
        GUI.DrawTexture(previewRect, previewRenderer.EndPreview());
        
        Rect labelRect = new Rect(previewRect.x, previewRect.y + previewRect.height - 20, previewRect.width, 20);
        GUI.Label(labelRect, "Block Preview", EditorStyles.centeredGreyMiniLabel);
    }
    private void UpdateCameraPosition()
    {
        float horizontalRadians = cameraOrbit.y * Mathf.Deg2Rad;
        float verticalRadians = cameraOrbit.x * Mathf.Deg2Rad;
        
        float x = Mathf.Sin(horizontalRadians) * Mathf.Cos(verticalRadians);
        float y = Mathf.Sin(verticalRadians);
        float z = Mathf.Cos(horizontalRadians) * Mathf.Cos(verticalRadians);
        
        Vector3 cameraPosition = new Vector3(x, y, z);
        previewRenderer.camera.transform.position = cameraPosition * cameraDistance;
        previewRenderer.camera.transform.LookAt(Vector3.zero, Vector3.up);
        
        Repaint();
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
