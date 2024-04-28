using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    static volatile T _uniqueInstance = null;
    static volatile GameObject _uniqueObject = null;
    protected MonoSingleton()
    {
    }
    public static T _instance
    {
        get {
            if (_uniqueInstance == null)
            {
                lock (typeof(T))
                {
                    if (_uniqueInstance == null && _uniqueObject == null)
                    {
                        T[] objects = GameObject.FindObjectsOfType<T>();

                        if (objects.Length > 1)
                        {
                            Debug.Log("Singleton Error - Not Single Instance " + typeof(T).ToString());
                            return _instance;
                        }
                        else if (objects.Length == 1)
                        {
                            _uniqueObject = objects[0].gameObject;
                            _uniqueInstance = objects[0].gameObject.GetComponent<T>();
                            Debug.Log("Already Instance " + objects[0]);
                        }
                        else
                        {
                            _uniqueObject = new GameObject(typeof(T).Name, typeof(T));
                            _uniqueInstance = _uniqueObject.GetComponent<T>();
                            Debug.Log("Singleton Instance Success " + typeof(T).ToString());
                        }
                        _uniqueInstance.InitSetting();
                    }
                }
            }

            return _uniqueInstance;
        }
    }

    void InitSetting()
    {
        DontDestroyOnLoad(this);
    }
}
