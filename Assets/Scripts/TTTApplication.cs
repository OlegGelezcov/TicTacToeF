namespace TTT {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Game application class represent DDO-object
    /// Hold references to controllers ( UI and Grid) and FSM - context object 
    /// </summary>
    public class TTTApplication : Singleton<TTTApplication> {

        private static bool m_IsCreated = false;

        private readonly Dictionary<ControllerType, IController> m_Controllers = new Dictionary<ControllerType, IController>();
        private readonly IStateContext m_GameContext = new GameStateContext();

        void Awake() {
            if(!m_IsCreated) {
                m_IsCreated = true;
                DontDestroyOnLoad(gameObject);

                SceneManager.sceneLoaded += (scene, mode) => {
                    if(scene.name == "game") {
                        gameContext.ChangeState(new RoundStartedState());
                    } else if(scene.name == "menu") {
                        //remove grid controller in menu mode and create again in game mode
                        if(m_Controllers.ContainsKey(ControllerType.grid)) {
                            m_Controllers.Remove(ControllerType.grid);
                        }
                        gameContext.ChangeState(new MainMenuState());
                    }
                };

                m_Controllers.Clear();
                //on start create only UI controller
                m_Controllers.Add(ControllerType.UI, new UIController(new UIModel(), UICanvas.get));
                gameContext.ChangeState(new MainMenuState());

            } else {
                Destroy(gameObject);
                return;
            }
        }

        void Update() {
            gameContext.Update();

        }

        /// <summary>
        /// broadcast game events among all existing controllers
        /// </summary>
        public void SendEvent(object sender, EventData data) {
            foreach(var controller in m_Controllers) {
                controller.Value.OnNotify(sender, data);
            }
        }

        public IStateContext gameContext {
            get {
                return m_GameContext;
            }
        }

        private IController GetController(ControllerType type) {
            if(m_Controllers.ContainsKey(type)) {
                return m_Controllers[type];
            }
            throw new System.ArgumentException(string.Format("not found controller of type: {0}", type));
        }

        /// <summary>
        /// Grid controller exists only in game mode when round started
        /// </summary>
        public void StartRound() {
            if (m_Controllers.ContainsKey(ControllerType.grid)) {
                (m_Controllers[ControllerType.grid] as GridController).StartRound();
            } else {

                GridController gridController = new GridController(new GridModel(), FindObjectOfType<GridView>());
                m_Controllers.Add(ControllerType.grid, gridController);
                gridController.StartRound();
            }      
        }


        /// <summary>
        /// use coroutine when enemy turn, we insert small delay after player turn
        /// </summary>
        public void MakeEnemyTurn() {
            StartCoroutine(CorMakeEnemyTurn());
        }

        private IEnumerator CorMakeEnemyTurn() {
            yield return new WaitForSeconds(0.5f);
            (m_Controllers[ControllerType.grid] as GridController).MakeEnemyTurn();
        }

        public GameDifficulty difficulty {
            get {
                return (m_Controllers[ControllerType.UI] as UIController).difficulty;
            }
        }
    }



    /// <summary>
    /// Base class for all game events, i don't use System.EventData here
    /// </summary>
    public abstract class EventData {
        public abstract EventType eventType { get; }
    }


    /// <summary>
    /// For casting to convcrete events i MVC-controllers use event types
    /// </summary>
    public enum EventType {
        CellViewClicked,
        CellItemChanged,
        WinResult,
        GameStateChanged,
        UIActionEvent,
        UIPropertyChanged
    }


    /// <summary>
    /// Sended when player click on cell in game
    /// </summary>
    public class CellViewClickedEventData : EventData {

        public override EventType eventType {
            get {
                return EventType.CellViewClicked;
            }
        }

        public CellView cellView { get; private set; }

        public CellViewClickedEventData(CellView cellView) {
            this.cellView = cellView;
        }
    }

    /// <summary>
    /// Sended when cell representation in model changed
    /// </summary>
    public class CellItemChangedEventData : EventData {

        public CellItem cellItem { get; private set; }

        public CellItemChangedEventData(CellItem cellItem ) {
            this.cellItem = cellItem;
        }

        public override EventType eventType {
            get {
                return EventType.CellItemChanged;
            }
        }
    }

    /// <summary>
    /// Sended after each turn, hold info about round status
    /// If round don't ended yet result will be NULL
    /// </summary>
    public class WinResultEventData : EventData {

        public CheckWinResult result { get; private set; }

        public WinResultEventData(CheckWinResult result ) {
            this.result = result;
        }

        public override EventType eventType {
            get {
                return EventType.WinResult;
            }
        }
    }

    /// <summary>
    /// Sended when changed state of FSM context
    /// </summary>
    public class GameStateChangedEventData : EventData {
        public GameState state { get; private set; }

        public GameStateChangedEventData(GameState state ) {
            this.state = state;
        }

        public override EventType eventType {
            get {
                return EventType.GameStateChanged;
            }
        }
    }

    /// <summary>
    /// Sended when any UI event occured ( button click, toggle selection etc. )
    /// </summary>
    public class UIActionEventData : EventData {
        public UIActionData actionData { get; private set; }

        public UIActionEventData(UIActionData actionData ) {
            this.actionData = actionData;
        }

        public override EventType eventType {
            get {
                return EventType.UIActionEvent;
            }
        }
    }

    /// <summary>
    /// Sended when property changed in UIModel (i don't use such pattern for GridModel)
    /// </summary>
    public class UIPropertyChangedEventData : EventData {

        public UIPropertyName propertyName { get; private set; }
        public object value { get; private set; }

        public UIPropertyChangedEventData(UIPropertyName propertyName, object value ) {
            this.propertyName = propertyName;
            this.value = value;
        }

        public override EventType eventType {
            get {
                return EventType.UIPropertyChanged;
            }
        }
    }

    /// <summary>
    /// Codes for UI Model properties
    /// </summary>
    public enum UIPropertyName {
        playerScore,
        enemyScore,
        gameDifficulty
    }
}