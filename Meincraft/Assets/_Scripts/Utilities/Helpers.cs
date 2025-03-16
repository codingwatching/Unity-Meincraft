using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Helpers
{
    public static Vector2 ToVector2(this Vector3 reference)
    {
        return new Vector2(reference.x, reference.y);
    }
    public static Vector2Int ToVector2Int(this Vector3Int reference)
    {
        return new Vector2Int(reference.x, reference.y);
    }
    public static Vector3 ToVector3(this Vector2 reference, float zValue = 0)
    {
        return new Vector3(reference.x, reference.y, zValue);
    }
    public static Vector3Int ToVector3Int(this Vector2Int reference, int zValue)
    {
        return new Vector3Int(reference.x, reference.y, zValue);
    }
    public static Vector3Int ToVector3Int(this Vector2Int reference)
    {
        return new Vector3Int(reference.x, 0, reference.y);
    }

    public static Vector4 ToVector4(this Vector3 reference, float w = 0)
    {
        return new Vector4(reference.x, reference.y, reference.z, w);
    }
    public static Vector4 ToVector4(this Vector3Int reference, float w = 0)
    {
        return new Vector4(reference.x, reference.y, reference.z, w);
    }
    public static Vector3 Absolute(this Vector3 reference)
    {
        return new Vector3(Mathf.Abs(reference.x), Mathf.Abs(reference.y), Mathf.Abs(reference.z));
    }
    public static Vector3Int Absolute(this Vector3Int reference)
    {
        return new Vector3Int(Mathf.Abs(reference.x), Mathf.Abs(reference.y), Mathf.Abs(reference.z));
    }


    public static bool IsArrived(this NavMeshAgent agent)
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }


    private static readonly Dictionary<float, WaitForSeconds> _waitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds WaitForSecondsNonAlloc(float time)
    {
        if (_waitDictionary.TryGetValue(time, out WaitForSeconds waitForSeconds)) return waitForSeconds;

        _waitDictionary[time] = new WaitForSeconds(time);

        return _waitDictionary[time];
    }

    private static readonly Dictionary<float, WaitForSecondsRealtime> _waitRealtimeDictionary = new Dictionary<float, WaitForSecondsRealtime>();
    public static WaitForSecondsRealtime WaitForSecondsRealtimeNonAlloc(float time)
    {
        if (_waitRealtimeDictionary.TryGetValue(time, out WaitForSecondsRealtime waitForSeconds)) return waitForSeconds;

        _waitRealtimeDictionary[time] = new WaitForSecondsRealtime(time);

        return _waitRealtimeDictionary[time];
    }

    public static T Rand<T>(this T[] array)
    {
        if(array.Length == 0) return default;
        return array[Random.Range(0, array.Length)];
    }

    public static T Rand<T>(this List<T> list)
    {
        if(list.Count == 0) return default;
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
