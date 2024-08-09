using UnityEngine;

namespace Arknights.Tools
{
    public abstract class Single<T> where T : Single<T>,new() 
    {
        private static T instance;
        public static T Instance()
        {
            if(instance != null) return instance;
            instance = new T();
            instance.Initialization();
            return instance;
        }

        protected virtual void Initialization() { }
    }

    public abstract class MonoSingle<T> : MonoBehaviour where T : MonoSingle<T> 
    {
        private static T instance;
        public static T Instance()
        {
            if (instance) return instance;
            GameObject gameObject = new GameObject(typeof(T).Name);
            instance = gameObject.AddComponent<T>();
            DontDestroyOnLoad(gameObject);
            return instance;
        }

        protected virtual void Initialization() { }

        private void Awake()
        {
            Initialization();
        }
    }
}
