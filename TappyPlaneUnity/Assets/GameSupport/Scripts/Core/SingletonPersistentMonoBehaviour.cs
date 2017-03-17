using UnityEngine;


abstract public class SingletonPersistentMonoBehaviour<T> : MonoBehaviour where T : SingletonPersistentMonoBehaviour<T>
{
    private static T m_instance;
    protected static bool m_destroyed;

    protected abstract void OnAwake();


    public static void EnsureInstanceCreated()
    {
        m_instance = Instance;
    }

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject prefab = Resources.Load<GameObject>(typeof(T).Name);
                if (prefab == null)
                {
                    Debug.LogError("Attempting to create the " + typeof(T).ToString() + " singleton but the prefab doesn't exist!");
                    return null;
                }

                Instantiate(prefab);

                if (m_instance == null)
                {
                    Debug.LogError("The prefab for " + typeof(T) + " does not have that component!");
                    return null;
                }

                GameObject.DontDestroyOnLoad(m_instance.gameObject);
            }

            return m_instance;
        }
    }

    void Awake()
    {
        m_instance = this as T;
        m_destroyed = false;

        OnAwake();
    }

    protected virtual void OnDestroy()
    {
        m_destroyed = true;
        m_instance = null;
    }
}
