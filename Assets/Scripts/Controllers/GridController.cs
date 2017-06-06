namespace TTT {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TTT;
    using UnityEngine;


    /// <summary>
    /// First MVC-controller for management game logic
    /// </summary>
    public class GridController : BaseController {

        private GridModel m_Model;
        private GridView m_View;


        public GridController(GridModel model, GridView view) {
            m_Model = model;
            m_View = view;

            RegisterEventHandler(EventType.CellItemChanged, OnCellItemChanged);
            RegisterEventHandler(EventType.CellViewClicked, OnCellViewClicked);
            RegisterEventHandler(EventType.WinResult, OnWinResult);


        }

        private void OnCellViewClicked(EventData eventData ) {
            CellViewClickedEventData cellViewClickedEventData = eventData as CellViewClickedEventData;
            if(cellViewClickedEventData != null ) {
                if (application.gameContext.state.stateName == GameStateName.playerTurn) {
                    cellViewClickedEventData.cellView.Deactivate();
                    m_Model.SetState(cellViewClickedEventData.cellView.index, Sign2CellState(m_Model.playerSing));
                }
                //} else if(application.gameContext.state.stateName == GameStateName.enemyTurn ) {
                //    m_Model.SetState(cellViewClickedEventData.cellView.index, Sign2CellState(m_Model.enemySign));
                //}
            }
        }

        private void OnCellItemChanged(EventData eventData) {
            CellItemChangedEventData cellItemChangedEventData = eventData as CellItemChangedEventData;
            if(cellItemChangedEventData != null ) {
                m_View.SetState(cellItemChangedEventData.cellItem);
            }
        }

        private void OnWinResult(EventData eventData ) {
            WinResultEventData winResultEventData = eventData as WinResultEventData;
            if(winResultEventData != null ) {
                if (winResultEventData.result != null) {
                    m_View.ShowWinSequence(winResultEventData.result);
                    m_Model.SetRoundResult(winResultEventData.result.roundResultType);

                    application.gameContext.ChangeState(new RoundCompleteState());
                    
                } else {
                    if(application.gameContext.state.stateName == GameStateName.playerTurn ) {
                        application.gameContext.ChangeState(new EnemyTurnState());
                    } 
                }
            }
        }

        private CellState Sign2CellState(SignType sign) {
            return (sign == SignType.X) ? CellState.xFilled : CellState.oFilled;
        }

        public override ControllerType controllerType {
            get {
                return ControllerType.grid;
            }
        }

        public void StartRound() {
            m_Model.PrepareRound();

            if(m_Model.playerSing == SignType.X ) {
                application.gameContext.ChangeState(new PlayerTurnState());
            } else {
                application.gameContext.ChangeState(new EnemyTurnState());
            }

            m_Model.PrintValues();
        }

        public void MakeEnemyTurn() {
            if (application.gameContext.state.stateName == GameStateName.enemyTurn) {
                Debug.Log("get cell for difficulty: " + application.difficulty);
                var emptyCell = m_Model.GetRandomEmptyCell(application.difficulty);
                if (emptyCell != null) {
                    m_Model.SetState(emptyCell.index, Sign2CellState(m_Model.enemySign));
                }
                application.gameContext.ChangeState(new PlayerTurnState());
            }
        }
    }


    public enum ControllerType {
        UI,
        grid
    }

    public interface IController {
        void OnNotify(object sender, EventData data);
        ControllerType controllerType { get; }
    } 

    public abstract class BaseController  : TTTObject, IController {

        private readonly Dictionary<EventType, System.Action<EventData>> m_EventHandlers = new Dictionary<EventType, Action<EventData>>();

        public void OnNotify(object sender, EventData data) {
            if (m_EventHandlers.ContainsKey(data.eventType)) {
                m_EventHandlers[data.eventType](data);
            }
        }

        protected void RegisterEventHandler(EventType eventType, System.Action<EventData> handler) {
            m_EventHandlers[eventType] = handler;
        }

        public abstract ControllerType controllerType { get; }
    }
}