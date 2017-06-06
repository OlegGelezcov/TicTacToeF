namespace TTT {
    using UnityEngine;

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T s_Instance = default(T);

        public static T get {
            get {
                if(!s_Instance ) {
                    s_Instance = GameObject.FindObjectOfType<T>();
                }
                if(!s_Instance) {
                    GameObject newSingletonObj = new GameObject(typeof(T).Name, typeof(T));
                    s_Instance = newSingletonObj.GetComponent<T>();
                }
                return s_Instance;
            }
        }
    }

}