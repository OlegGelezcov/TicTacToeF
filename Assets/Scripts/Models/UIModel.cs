namespace TTT {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Model for UI
    /// </summary>
    public class UIModel : TTTObject {

        private int m_PlayerScore = 0;
        private int m_EnemyScore = 0;
        private GameDifficulty m_GameDifficulty = GameDifficulty.easy;

        public void SetGameDifficulty(GameDifficulty difficulty) {
            var oldDifficulty = m_GameDifficulty;
            m_GameDifficulty = difficulty;
            if(oldDifficulty != difficulty ) {
                application.SendEvent(this, new UIPropertyChangedEventData(UIPropertyName.gameDifficulty, difficulty));
            }
        }

        public GameDifficulty difficulty {
            get {
                return m_GameDifficulty;
            }
            set {
                var oldDifficulty = difficulty;
                m_GameDifficulty = value;
                if (value != oldDifficulty) {
                    application.SendEvent(this, new UIPropertyChangedEventData(UIPropertyName.gameDifficulty, difficulty));
                }
            }
        }

        public void IncrementPlayerScore() {
            m_PlayerScore++;
            application.SendEvent(this, new UIPropertyChangedEventData(UIPropertyName.playerScore, m_PlayerScore));
        }

        public void IncrementEnemyScore() {
            m_EnemyScore++;
            application.SendEvent(this, new UIPropertyChangedEventData(UIPropertyName.enemyScore, m_EnemyScore));
        }

    }

    public enum GameDifficulty {
        easy,
        medium,
        hard
    }


}