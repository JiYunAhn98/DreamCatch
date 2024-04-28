using System.Collections;
using System.Collections.Generic;

public class CSSingleton<T> where T: CSSingleton<T>
{
    static T _uniqueInstance;

    protected CSSingleton()
    { 
    }

    public static T _instance
    {
        get
        { 
            if (_uniqueInstance == null)
            {
                lock (typeof(T))
                {
                    if (_uniqueInstance == null)
                    {
                        _uniqueInstance = System.Activator.CreateInstance(typeof(T)) as T;
                    }
                }
            }
            return _uniqueInstance;
        }
    }
}
