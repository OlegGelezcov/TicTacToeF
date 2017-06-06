namespace TTT {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class TTTBehaviour : MonoBehaviour {

        public TTTApplication application {
            get {
                return TTTApplication.get;
            }
        }
    }

    public abstract class TTTObject {

        public TTTApplication application {
            get {
                return TTTApplication.get;
            }
        }
    }
}