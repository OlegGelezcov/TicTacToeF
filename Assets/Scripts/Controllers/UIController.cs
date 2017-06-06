namespace TTT {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Second MVC controller for management UI
    /// </summary>
    public class UIController : BaseController, IController {

        private UIModel m_Model;
        private UICanvas m_View;
        private CheckWinResult m_WinResult;

        public UIController(UIModel model, UICanvas view ) {
            m_Model = model;
            m_View = view;
            RegisterEventHandler(EventType.GameStateChanged, OnGameStateChanged);
            RegisterEventHandler(EventType.UIActionEvent, OnUIAction);
            RegisterEventHandler(EventType.UIPropertyChanged, OnUIPropertyChnaged);
            RegisterEventHandler(EventType.WinResult, OnWinResult);

            
        }

        public GameDifficulty difficulty {
            get {
                return m_Model.difficulty;
            }
        }

        private void OnGameStateChanged(EventData eventData ) {
            GameStateChangedEventData gameStateChangedEventData = eventData as GameStateChangedEventData;

            if(gameStateChangedEventData.state.stateName == GameStateName.mainMenu ) {
                m_View.ShowUI(UIType.mainMenu);
            } else {
                m_View.RemoveView(UIType.mainMenu);
            }

            if(gameStateChangedEventData.state.stateName == GameStateName.enterGame) {
                m_View.ShowUI(UIType.gameHud);
            } else if(gameStateChangedEventData.state.stateName == GameStateName.leaveGame ) {
                m_View.RemoveView(UIType.gameHud);
                m_View.RemoveView(UIType.roundCompleteView);
            }

            if(gameStateChangedEventData.state.stateName == GameStateName.roundStarted) {
                m_View.RemoveView(UIType.roundCompleteView);
            }

            if(gameStateChangedEventData.state.stateName == GameStateName.roundComplete ) {
                if(m_WinResult != null ) {
                    Debug.Log("HERE");
                    application.StartCoroutine(CorShowRoundComplete(m_WinResult.roundResultType));
                }
            }
        }

        private void OnUIAction(EventData eventData) {
            UIActionEventData uiActionEventData = eventData as UIActionEventData;
            var actionName = uiActionEventData.actionData.actionName;

            if(actionName == UIActionName.playClicked ) {
                application.gameContext.ChangeState(new EnterGameState());
            }
            if(actionName== UIActionName.exitMenuClicked ) {
                application.gameContext.ChangeState(new LeaveGameState());
            }
            if(actionName == UIActionName.playAgainClicked ) {
                application.StartCoroutine(CorRestartRound());
            }
            if(actionName == UIActionName.easySelected ) {
                m_Model.difficulty = GameDifficulty.easy;
            } 
            if(actionName == UIActionName.mediumSelected ) {
                m_Model.difficulty = GameDifficulty.medium;
            }
            if(actionName == UIActionName.hardSelected ) {
                m_Model.difficulty = GameDifficulty.hard;
            }
        }

        private IEnumerator CorRestartRound() {
            m_View.RemoveView(UIType.roundCompleteView);
            yield return new WaitForSeconds(0.5f);
            application.gameContext.ChangeState(new RoundStartedState());
        }

        private void OnUIPropertyChnaged(EventData eventData ) {
            UIPropertyChangedEventData uiPropertyEventData = eventData as UIPropertyChangedEventData;



            if(uiPropertyEventData.propertyName == UIPropertyName.playerScore  ) {
                var hud = m_View.GetView<GameHudView>(UIType.gameHud);
                if(hud != null ) {
                    hud.playerScore = (int)uiPropertyEventData.value;
                }
            }

            if (uiPropertyEventData.propertyName == UIPropertyName.enemyScore ) {
                var hud = m_View.GetView<GameHudView>(UIType.gameHud);
                if(hud != null ) {
                    hud.enemyScore = (int)uiPropertyEventData.value;
                }
            }
        }


        private void OnWinResult(EventData eventData ) {
            WinResultEventData winResultEventData = eventData as WinResultEventData;
            if(winResultEventData.result != null ) {
                m_WinResult = winResultEventData.result;
                
            }
        }

        private IEnumerator CorShowRoundComplete(RoundResultType roundResultType) {
            yield return new WaitForSeconds(0.5f);

            m_View.ShowUI(UIType.roundCompleteView);
            var roundCompleteView = m_View.GetView<RoundCompleteView>(UIType.roundCompleteView);


            if (roundResultType == RoundResultType.playerWin) {
                m_Model.IncrementPlayerScore();
                if (roundCompleteView) {
                    roundCompleteView.statusText = string.Format("<color=green>{0}</color>", "You are win!");
                }
            } else if (roundResultType == RoundResultType.enemyWin) {
                m_Model.IncrementEnemyScore();
                if (roundCompleteView) {
                    roundCompleteView.statusText = string.Format("<color=red>{0}</color>", "Enemy win!");
                }
            } else if (roundResultType == RoundResultType.deadHeat) {
                if (roundCompleteView) {
                    roundCompleteView.statusText = string.Format("<color=yellow>{0}</color>", "Dead heat!");
                }
            }
        }

        public override ControllerType controllerType {
            get {
                return ControllerType.UI;
            }
        }
    }

}