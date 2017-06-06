namespace TTT {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class RoundCompleteView : BaseUIView {

        [SerializeField]
        private Text m_StatusText;

        [SerializeField]
        private Button m_PlayAgainButton;

        [SerializeField]
        private Button m_ExitMenuButton;


        public override void Setup() {
            base.Setup();

            statusText = string.Empty;

            m_PlayAgainButton.onClick.RemoveAllListeners();
            m_PlayAgainButton.onClick.AddListener(() => {
                SendEvent(new UIActionData(uiType, UIActionName.playAgainClicked));
                m_PlayAgainButton.interactable = false;
            });

            m_ExitMenuButton.onClick.RemoveAllListeners();
            m_ExitMenuButton.onClick.AddListener(() => {
                SendEvent(new UIActionData(uiType, UIActionName.exitMenuClicked));
            });
        }

        public override UIType uiType {
            get {
                return UIType.roundCompleteView;
            }
        }

        public string statusText {
            set {
                m_StatusText.text = value;
            }
        }
    }

}