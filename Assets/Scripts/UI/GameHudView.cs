namespace TTT {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameHudView : BaseUIView {

        [SerializeField]
        private Text m_PlayerScore;

        [SerializeField]
        private Text m_EnemyScore;

        [SerializeField]
        private Button m_ExitMenuButton;

        public override void Setup() {
            Debug.Log("setup...");
            base.Setup();
            playerScore = 0;
            enemyScore = 0;

            m_ExitMenuButton.onClick.RemoveAllListeners();
            m_ExitMenuButton.onClick.AddListener(() => {
                Debug.Log("clicked...");
                SendEvent(new UIActionData(uiType, UIActionName.exitMenuClicked));
            });
        }

        public override UIType uiType {
            get {
                return UIType.gameHud;
            }
        }

        public int playerScore {
            set {
                m_PlayerScore.text = value.ToString();
            }
        }

        public int enemyScore {
            set {
                m_EnemyScore.text = value.ToString();
            }
        }
    }

}