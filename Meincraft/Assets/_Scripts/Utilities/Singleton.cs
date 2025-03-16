using UnityEngine;
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;
}
/*public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}*/
public class SingletonPersistent<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Static instance of PersistentGameObjectSingleton which allows it to be accessed by any other script.
    /// </summary>
    public static T Instance { get; private set; }

    /// <summary>
    /// Things to do as soon as the scene starts up
    /// </summary>
    void Awake()
    {

        if (Instance == null)
        {

            //if not, set instance to this
            Instance = this as T;

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GlobalManager.
            DestroyImmediate(gameObject);

            return;
        }
    }
}