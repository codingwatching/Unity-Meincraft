using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="Block Library", fileName ="Block Library")]
public class BlockLibrary : ScriptableObject
{
    [SerializeField] BlockData[] data;
    
    public BlockData this[byte idx] => data[idx];
}