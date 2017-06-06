namespace TTT {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public interface IGridModel {
        void SetState(int index, CellState state);
        CellState GetState(int index);
    }

    /// <summary>
    /// Model for Game MVC
    /// </summary>
    public class GridModel  : TTTObject,  IGridModel {

        private const int kCellCount = 9;

        //indices of cells to win conditions
        // 0  1  2
        // 3  4  5
        // 6  7  8
        private readonly List<int[]> m_WinSequences = new List<int[]> {
            new int[] { 0, 1, 2},
            new int[] { 3, 4, 5},
            new int[] { 6, 7, 8},
            new int[] { 0, 4, 8},
            new int[] { 2, 4, 6},
            new int[] { 0, 3, 6},
            new int[] { 1, 4, 7},
            new int[] { 2, 5, 8}
        };

        //each CellItem has view CellView
        private CellItem[] m_Cells;

        //remember results for current and previous rounds
        private RoundResultType m_PrevRoundResultType = RoundResultType.playerWin;
        private RoundResultType m_RoundResultType = RoundResultType.playerWin;

        //sign for player X or O
        private SignType m_PlayerSign;

        public void PrintValues() {
            Debug.Log(string.Format("Player Sign: {0}, Enemy Sign: {1}, Prev Round: {2}, Current Round: {3}", playerSing, enemySign, prevRoundResultType, roundResultType));
        }

        public GridModel() {
            m_Cells = new CellItem[kCellCount];
            for (int i = 0; i < m_Cells.Length; i++) {
                m_Cells[i] = new CellItem(i);
            }
        }


        //Called when round started
        public void PrepareRound() {
            //setup player symbol with description in tech. task
          
            if(roundResultType == RoundResultType.playerWin) {
                m_PlayerSign = SignType.X;
            } else if(roundResultType == RoundResultType.enemyWin ){
                m_PlayerSign = SignType.O;
            } else if(roundResultType == RoundResultType.deadHeat ) {
                if(prevRoundResultType == RoundResultType.playerWin ) {
                    m_PlayerSign = SignType.O;
                } else if(prevRoundResultType == RoundResultType.enemyWin ) {
                    m_PlayerSign = SignType.X;
                } else {
                    m_PlayerSign = SignType.X;
                }
            }

            //clear cells and views
            Debug.LogFormat("select player sign: {0}", m_PlayerSign);
            for(int i = 0; i < kCellCount; i++ ) {
                m_Cells[i].SetState(CellState.empty);
                application.SendEvent(this, new CellItemChangedEventData(m_Cells[i]));
            }
        }

        public void SetRoundResult(RoundResultType roundResult) {
            m_PrevRoundResultType = roundResultType;
            m_RoundResultType = roundResult;
        }

        #region Props
        private int countOfEmptyCells {
            get {
                int count = 0;
                foreach (var cell in m_Cells) {
                    if (cell.state == CellState.empty) {
                        count++;
                    }
                }
                return count;
            }
        }


        public SignType playerSing {
            get {
                return m_PlayerSign;
            }
        }

        public SignType enemySign {
            get {
                return (playerSing == SignType.X) ? SignType.O : SignType.X;
            }
        }

        public RoundResultType roundResultType {
            get {
                return m_RoundResultType;
            }
        }

        public RoundResultType prevRoundResultType {
            get {
                return m_PrevRoundResultType;
            }
        } 
        #endregion


        #region Helper funcs

        /// <summary>
        /// At different difficulties used different AI algorithms
        /// </summary>
        public CellItem GetRandomEmptyCell(GameDifficulty difficulty) {

            switch (difficulty) {
                case GameDifficulty.easy: {
                        return GetEasyCellItem();
                    }
                case GameDifficulty.medium: {
                        return GetMediumCellItem();
                    }
                case GameDifficulty.hard: {
                        return GetHardCellItem();
                    }
                default: {
                        return GetEasyCellItem();
                    }
            }

        }

        private CellItem GetEasyCellItem() {
            List<CellItem> emptyCells = new List<CellItem>();
            foreach (var cellItem in m_Cells) {
                if (cellItem.state == CellState.empty) {
                    emptyCells.Add(cellItem);
                }
            }
            if (emptyCells.Count > 0) {
                return emptyCells[UnityEngine.Random.Range(0, emptyCells.Count - 1)];
            }
            return null;
        }

        private CellItem GetMediumCellItem() {
            if (UnityEngine.Random.value < 0.5f) {
                var index = GetEmptyCellIndexOfLongestPlayerSequence();
                Debug.Log("medium index: " + index);
                if (index >= 0) {
                    return m_Cells[index];
                }
            }
            return GetEasyCellItem();
        }

        private CellItem GetHardCellItem() {
            if (UnityEngine.Random.value < 0.5f) {
                var index = GetEmptyCellIndexOfLongestPlayerSequence();
                Debug.Log("hard index " + index);
                if (index >= 0) {
                    return m_Cells[index];
                }
            }
            return GetMediumCellItem();
        }

        //get index of empty cell (used in AI on hard difficulty)
        private int GetEmptyCellIndexOfLongestPlayerSequence() {
            List<SequnceInfo> analyzeResult = new List<SequnceInfo>();
            foreach (var seq in m_WinSequences) {
                var info = AnalyzeSequence(seq);
                if (info.isOnlyPlayer) {
                    analyzeResult.Add(AnalyzeSequence(seq));
                }
            }

            if (analyzeResult.Count > 0) {
                analyzeResult.Sort((a, b) => {
                    return a.countOfFilled.CompareTo(b.countOfFilled);
                });

                var longestPlayerInfo = analyzeResult[analyzeResult.Count - 1];
                int resultIndex = -1;
                foreach (var index in longestPlayerInfo.sequence) {
                    if (IsEmptyCell(m_Cells[index])) {
                        resultIndex = index;
                        break;
                    }
                }
                return resultIndex;
            }
            return -1;
        }

        //Collect cells info about how fill player those sequence
        private SequnceInfo AnalyzeSequence(int[] seq) {

            bool hasPlayer = false;
            bool hasEnemy = false;
            int filledCount = 0;
            SequnceInfo info = new SequnceInfo { sequence = seq };
            foreach (int index in seq) {
                if (IsPlayerCell(m_Cells[index])) {
                    hasPlayer = true;
                    filledCount++;
                } else if (IsEnemyCell(m_Cells[index])) {
                    hasEnemy = true;
                    filledCount++;
                }
            }
            info.countOfFilled = filledCount;
            if (hasPlayer && !hasEnemy) {
                info.isOnlyPlayer = true;
            }
            return info;
        }

        private RoundResultType CellState2RoundResultType(CellState state) {
            if (state == CellState.xFilled) {
                if (playerSing == SignType.X) {
                    return RoundResultType.playerWin;
                } else {
                    return RoundResultType.enemyWin;
                }
            } else if (state == CellState.oFilled) {
                if (playerSing == SignType.O) {
                    return RoundResultType.playerWin;
                } else {
                    return RoundResultType.enemyWin;
                }
            }
            return RoundResultType.deadHeat;
        }

        private bool CheckToWin(out CheckWinResult winResult) {
            winResult = null;

            if (countOfEmptyCells == 0) {
                winResult = new CheckWinResult(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, CellState.empty, RoundResultType.deadHeat);
            }

            foreach (var sequence in m_WinSequences) {
                if (CheckToWinSequence(sequence)) {
                    winResult = new CheckWinResult(sequence, m_Cells[sequence[0]].state, CellState2RoundResultType(m_Cells[sequence[0]].state));
                    return true;
                }
            }
            return false;
        }



        private bool CheckToWinSequence(int[] sequence) {
            return (m_Cells[sequence[0]].state == m_Cells[sequence[1]].state) &&
                (m_Cells[sequence[1]].state == m_Cells[sequence[2]].state) &&
                (m_Cells[sequence[0]].state != CellState.empty);
        }

        private bool IsEnemyCell(CellItem cellItem) {
            return (!IsPlayerCell(cellItem)) && (!IsEmptyCell(cellItem));
        }

        private bool IsEmptyCell(CellItem cellItem) {
            return cellItem.state == CellState.empty;
        }

        private bool IsPlayerCell(CellItem cellItem) {
            if (cellItem.state == CellState.xFilled) {
                if (playerSing == SignType.X) {
                    return true;
                }
            } else if (cellItem.state == CellState.oFilled) {
                if (playerSing == SignType.O) {
                    return true;
                }
            }
            return false;
        } 
        #endregion

        #region IGridModel
        //public event Action<IGridModel, CellItem> CellItemChanged;
        //public event System.Action<IGridModel, CheckWinResult> WinResult;

        public CellState GetState(int index) {
            if (index < kCellCount) {
                return m_Cells[index].state;
            }
            throw new System.IndexOutOfRangeException(string.Format("invalid cell index: {0}", index));
        }

        public void SetState(int index, CellState state) {
            if (index < kCellCount) {
                var oldState = m_Cells[index].state;
                m_Cells[index].SetState(state);

                if ((oldState != state)  ) {
                    application.SendEvent(this, new CellItemChangedEventData(m_Cells[index]));

                    CheckWinResult winResult;
                    CheckToWin(out winResult);
                    application.SendEvent(this, new WinResultEventData(winResult));

                }
            } else {
                throw new System.IndexOutOfRangeException(string.Format("invalid cell index: {0}", index));
            }
        } 
        #endregion


    }

    public enum SignType : byte { X, O }


    /// <summary>
    /// Hold info about round win or lose state
    /// </summary>
    public class CheckWinResult {
        public int[] winSequence { get; private set; }
        public CellState winState { get; private set; }
        public RoundResultType roundResultType { get; private set; }
        
        public CheckWinResult(int[] winSequence, CellState winState, RoundResultType roundResultType) {
            this.winSequence = winSequence;
            this.winState = winState;
            this.roundResultType = roundResultType;
        }
    }

    /// <summary>
    /// Sates of cells
    /// </summary>
    public enum CellState : byte {
        empty,
        xFilled,
        oFilled
    }

    public class CellItem {

        public int index { get; private set; }
        public CellState state { get; private set; }

        public CellItem(int index) {
            this.index = index;
            this.state = CellState.empty;
        }

        public void SetState(CellState newState ) {
            this.state = newState;
        }
    }

    public class SequnceInfo {
        public int[] sequence;
        public int countOfFilled = 0;
        public bool isOnlyPlayer = false;
    }

    public enum RoundResultType {
        playerWin,
        enemyWin,
        deadHeat
    }

}