namespace TTT {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class BaseUIView : TTTBehaviour {

        public virtual void Setup() { }
       
        public void SendEvent(UIActionData actionData ) {
            application.SendEvent(this, new UIActionEventData(actionData));
        }

        public abstract UIType uiType { get; }
    }



}