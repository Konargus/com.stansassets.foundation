using UnityEngine;

namespace StansAssets.Foundation
{
	/// <summary>
    /// Singleton pattern implementation.
    /// Can be used with classes extended from a MonoBehaviour.
    /// Once instance is found or created, game object will be marked as DontDestroyOnLoad
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		static T s_Instance;
		static bool s_ApplicationIsQuitting;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.transform.SetParent(SingletonService.Parent);
        }

        /// <summary>
        /// Returns a singleton class instance
        /// If current instance is not assigned it will try to find an object of the instance type,
        /// in case instance already exists on a scene. If not, new instance will be created
        /// </summary>
        public static T Instance
        {
			get
			{
				if (s_ApplicationIsQuitting)
				{
                    Debug.LogError(typeof(T) + " [SA_Singleton] is already destroyed. Returning null. Please check HasInstance first before accessing instance in destructor.");
                    return null;
                }

                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType(typeof(T)) as T;
                    if (s_Instance == null)
                        Instantiate();
                }
				return s_Instance;
			}
		}

        /// <summary>
        /// Methods will create new object Instantiate
        /// Normally method is called automatically when you referring to and Instance getter
        /// for a first time.
        /// But it may be useful if you want manually control when the instance is created,
        /// even if you do not this specific instance at the moment
        /// </summary>
        public static void Instantiate()
        {
            var name = typeof(T).FullName;
            s_Instance = new GameObject(name).AddComponent<T>();
        }

        /// <summary>
        /// True if Singleton Instance exists
        /// </summary>
        public static bool HasInstance => !IsDestroyed;

        /// <summary>
        /// True if Singleton Instance doesn't exist
        /// </summary>
        public static bool IsDestroyed => s_Instance == null;

        /// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed,
		/// it will create a buggy ghost object that will stay on the Editor scene
		/// even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		protected virtual void OnDestroy ()
        {
			s_Instance = null;
			s_ApplicationIsQuitting = true;
		}

		protected virtual void OnApplicationQuit ()
		{
			s_Instance = null;
			s_ApplicationIsQuitting = true;
		}
	}
}