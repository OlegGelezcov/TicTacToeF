namespace TTT {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Canvas))]
    public class UICanvas : Singleton<UICanvas> {

        private readonly Dictionary<UIType, BaseUIView> m_UIViews = new Dictionary<UIType, BaseUIView>();

        private static bool s_IsCreated = false;

        [SerializeField]
        private GameObject m_MainMenuPrefab;

        [SerializeField]
        private GameObject m_GameHudPrefab;

        [SerializeField]
        private GameObject m_RoundCompletePrefab;


        private Dictionary<UIType, GameObject> m_ViewPrefabs = null;

        void Awake() {
            if(!s_IsCreated ) {
                s_IsCreated = true;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
                return;
            }
        }

        public void ShowUI(UIType uiType ) {
            if(!m_UIViews.ContainsKey(uiType)) {
                var prefab = GetViewPrefab(uiType);
                if(prefab != null ) {
                    GameObject inst = Instantiate<GameObject>(prefab);
                    inst.transform.SetParent(transform, false);
                    m_UIViews.Add(uiType, inst.GetComponentInChildren<BaseUIView>());
                    inst.GetComponentInChildren<BaseUIView>().Setup();
                }
            }
        }

        public void RemoveView(UIType uiType ) {
            if(m_UIViews.ContainsKey(uiType)) {
                var inst = m_UIViews[uiType];
                m_UIViews.Remove(uiType);
                if(inst && inst.gameObject) {
                    Destroy(inst.gameObject);
                }
            }
        }

        public void RemoveViews() {
            List<UIType> uiTypes = new List<UIType>(m_UIViews.Keys);
            foreach(var uiType in uiTypes ) {
                RemoveView(uiType);
            }
        }

        public T GetView<T>(UIType type) where T : BaseUIView {
            if(m_UIViews.ContainsKey(type)) {
                return m_UIViews[type] as T;
            }
            return default(T);
        }

        private GameObject GetViewPrefab(UIType uiType ) {

            if(m_ViewPrefabs == null ) {
                m_ViewPrefabs = new Dictionary<UIType, GameObject> {
                    { UIType.mainMenu, m_MainMenuPrefab },
                    { UIType.gameHud,  m_GameHudPrefab  },
                    { UIType.roundCompleteView, m_RoundCompletePrefab }
                };
            }

            if(m_ViewPrefabs.ContainsKey(uiType)) {
                return m_ViewPrefabs[uiType];
            }
            return null;
        }
    }

    public class UIActionData {
        public UIType uiType { get; private set; }
        public UIActionName actionName { get; private set; }
        public object data { get; private set; }

        public UIActionData(UIType uiType, UIActionName actionName, object data = null ) {
            this.uiType = uiType;
            this.actionName = actionName;
            this.data = data;
        }
    }

    public enum UIActionName {
        playClicked,
        easySelected,
        mediumSelected,
        hardSelected,
        exitMenuClicked,
        playAgainClicked
    }

    public enum UIType {
        mainMenu,
        gameHud,
        roundCompleteView
    }


}