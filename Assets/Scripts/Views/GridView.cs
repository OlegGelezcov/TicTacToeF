namespace TTT {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Whole game field, represent View for cell collections
    /// </summary>
    public class GridView : TTTBehaviour, IGridView {

        //cells
        [SerializeField]
        private CellView[] m_CellViews;


        //we setup textures here in tiny project, in large games - it bad idea
        [SerializeField]
        private Texture m_XTexture;

        [SerializeField]
        private Texture m_OTexture;


        void Start() {
            foreach(var cell in m_CellViews) {
                cell.Setup(this);
            }
        }

        //Visually change state of cell, set texture on Quad object or diactivate when no texture
        public void SetState(CellItem cellItem) {
            if(cellItem.state == CellState.xFilled ) {
                m_CellViews[cellItem.index].SetTexture(m_XTexture);
            } else if(cellItem.state == CellState.oFilled ) {
                m_CellViews[cellItem.index].SetTexture(m_OTexture);
            } else {
                m_CellViews[cellItem.index].SetTexture(null);
            }
        }

        public void OnCellClicked(CellView cellView) {
            application.SendEvent(this, new CellViewClickedEventData(cellView));
        }

        //When sequence of cells completed we create "highlight" animation effect
        public void ShowWinSequence(CheckWinResult result ) {
            if(result != null ) {
                StartCoroutine(CorShowWinSequence(result.winSequence));
            }
        }

        private IEnumerator CorShowWinSequence(int[] indices) {
            foreach(var index in indices ) {
                var animator = m_CellViews[index].GetComponent<Animator>();
                if(animator) {
                    animator.SetTrigger("scale");
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }


    public interface IGridView {
        void SetState(CellItem cellItem);
    }

}