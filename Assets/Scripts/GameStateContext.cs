namespace TTT {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public interface IStateContext {
        void ChangeState(GameState state);
        void Update();
        GameState state { get; }
    }

    /// <summary>
    /// Game context realize FSM - pattern
    /// </summary>
    public class GameStateContext : TTTObject, IStateContext  {

        private GameState m_CurrentState = new NoneGameState();

        public void ChangeState(GameState state) {
            var oldState = m_CurrentState;
            m_CurrentState = state;
            if(oldState.stateName != state.stateName ) {
                oldState.OnExit();
                m_CurrentState.OnEnter();
                application.SendEvent(this, new GameStateChangedEventData(state));
            }
        }

        public void Update() {
            m_CurrentState.OnStay();
        }

        public GameState state {
            get {
                return m_CurrentState;
            }
        }
    }



    public enum GameStateName {
        none,
        mainMenu,
        enterGame,
        roundStarted,
        playerTurn,
        enemyTurn,
        roundComplete,
        leaveGame
    }

    /// <summary>
    /// Base class for game states
    /// </summary>
    public abstract class GameState : TTTObject {
        public virtual void OnEnter()   { }
        public virtual void OnStay()    { }
        public virtual void OnExit()    { }
        public abstract GameStateName stateName { get; }
    }

    public class NoneGameState : GameState {
        public override GameStateName stateName {
            get {
                return GameStateName.none;
            }
        }
    }

    public class MainMenuState : GameState {

        public override GameStateName stateName {
            get {
                return GameStateName.mainMenu;
            }
        }
    }

    public class RoundStartedState : GameState {
        public override GameStateName stateName {
            get {
                return GameStateName.roundStarted;
            }
        }

        public override void OnEnter() {
            base.OnEnter();
            application.StartRound();
        }
    }

    public class PlayerTurnState : GameState {
        public override GameStateName stateName {
            get {
                return GameStateName.playerTurn;
            }
        }
    }

    public class EnemyTurnState : GameState {
        public override GameStateName stateName {
            get {
                return GameStateName.enemyTurn;
            }
        }

        public override void OnEnter() {
            base.OnEnter();
            Debug.Log("Enter enemy turn");
            application.MakeEnemyTurn();

        }

    }

    public class RoundCompleteState : GameState {
        public override GameStateName stateName {
            get {
                return GameStateName.roundComplete;
            }
        }

        public override void OnEnter() {
            base.OnEnter();

        }
    }

    public class EnterGameState : GameState {

        public override GameStateName stateName {
            get {
                return GameStateName.enterGame;
            }
        }

        public override void OnEnter() {
            base.OnEnter();
            SceneManager.LoadSceneAsync("game");
        }
    }

    public class LeaveGameState : GameState {
        public override GameStateName stateName {
            get {
                return GameStateName.leaveGame;
            }
        }

        public override void OnEnter() {
            base.OnEnter();
            SceneManager.LoadSceneAsync("menu");
        }
    }
}