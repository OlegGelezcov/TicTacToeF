namespace TTT {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CellView : TTTBehaviour {

        [SerializeField]
        private int m_Index;

        [SerializeField]
        private MeshRenderer m_XOMesh;

        private GridView m_GridView;

        private bool m_IsActive = true;

        public int index {
            get {
                return m_Index;
            }
        }

        public void Setup(GridView parent) {
            m_GridView = parent;
        }

        public void SetTexture(Texture tex) {
            if (tex != null) {
                if (!m_XOMesh.gameObject.activeSelf) {
                    m_XOMesh.gameObject.SetActive(true);
                }
                m_XOMesh.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);
            } else {
                m_XOMesh.gameObject.SetActive(false);
                m_IsActive = true;
            }
        }

        public void ResetView() {
            m_XOMesh.gameObject.SetActive(false);
        }

        public void Deactivate() {
            m_IsActive = false;
        }

        void Update() {
            if(Input.GetMouseButtonUp(0)) {
                if(IsClicked(Input.mousePosition) && m_IsActive) {
                    m_GridView.OnCellClicked(this);
                }
            }
        }

        private bool IsClicked(Vector2 screenPosition) {
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                if(hit.collider.gameObject == gameObject ) {
                    return true;
                }
            }
            return false;
        }
    }

}