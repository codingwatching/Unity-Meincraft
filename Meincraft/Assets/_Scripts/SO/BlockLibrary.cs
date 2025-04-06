using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Block Library", fileName ="Block Library")]
public class BlockLibrary : ScriptableObject
{
    [SerializeField] BlockData[] data;

    public BlockData this[BlockType t] => data.FirstOrDefault(x => x.Type == t);
    public BlockData this[byte idx] => this[(BlockType)idx];
}