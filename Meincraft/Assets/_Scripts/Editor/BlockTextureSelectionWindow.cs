using System;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BlockTextureSelectionWindow : EditorWindow
{
    private static Texture2DArray blockAtlas;

    private Vector2 scrollPos;
    
    private static event Action<int> onClose;
    public static void ShowWindow(Texture2DArray atlas, Action<int> onCloseEvent)
    {
        onClose = onCloseEvent;
        
        BlockTextureSelectionWindow window = CreateInstance<BlockTextureSelectionWindow>();
        var position = window.position;
        position.center = new Rect(0f, 0f, Screen.currentResolution.width/2f, Screen.currentResolution.height/2f).position;
        window.position = position;
        float gridSize = Mathf.Sqrt(atlas.depth);
        Vector2 atlasSize = new Vector2(70*gridSize, 70*gridSize);
        window.minSize = atlasSize;
        window.maxSize = atlasSize;
        window.ShowModal();
    }
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        List<Texture> textures = new List<Texture>();
        for (int i = 0; i < blockAtlas.depth; i++)
        {
            Texture2D img = new Texture2D(16,16);
            Color[] colors = blockAtlas.GetPixels(i);
            if (colors.Length == 0) continue;
            img.SetPixels(colors);
            img.filterMode = FilterMode.Point;
            img.Apply();
            textures.Add(img);
        }
        int res = GUILayout.SelectionGrid(-1,textures.ToArray(),8, new GUIStyle(GUI.skin.box)
        {
            fixedWidth = 64,
            fixedHeight = 64,
            imagePosition = ImagePosition.ImageOnly,
            alignment = TextAnchor.MiddleCenter
        });
        if (res != -1)
        {
            onClose?.Invoke(res);
            Close();
        }
        EditorGUILayout.EndScrollView();
    }
}
