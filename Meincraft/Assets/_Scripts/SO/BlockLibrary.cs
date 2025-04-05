using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Block Library", fileName ="Block Library")]
public class BlockLibrary : ScriptableObject
{
    [SerializeField] BlockData[] data;

    public BlockData this[byte idx] => data[idx];
}