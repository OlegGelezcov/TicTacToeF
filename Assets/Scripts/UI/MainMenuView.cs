namespace TTT {
    using UnityEngine;
    using UnityEngine.UI;

    public class MainMenuView : BaseUIView {

        [SerializeField]
        private Button m_PlayButton;

        [SerializeField]
        private Toggle m_EasyToggle;

        [SerializeField]
        private Toggle m_MediumToggle;

        [SerializeField]
        private Toggle m_HardToggle;


        public override void Setup() {
            base.Setup();

            m_PlayButton.onClick.RemoveAllListeners();
            m_PlayButton.onClick.AddListener(() => {
                SendEvent(new UIActionData(uiType, UIActionName.playClicked));
            });

            m_EasyToggle.onValueChanged.RemoveAllListeners();
            m_EasyToggle.onValueChanged.AddListener((isOn) => {
                if(isOn ) {
                    SendEvent(new UIActionData(uiType, UIActionName.easySelected));
                }
            });

            m_MediumToggle.onValueChanged.RemoveAllListeners();
            m_MediumToggle.onValueChanged.AddListener((isOn) => {
                if(isOn) {
                    SendEvent(new UIActionData(uiType, UIActionName.mediumSelected));
                }
            });

            m_HardToggle.onValueChanged.RemoveAllListeners();
            m_HardToggle.onValueChanged.AddListener((isOn) => {
                if(isOn) {
                    SendEvent(new UIActionData(uiType, UIActionName.hardSelected));
                }
            });
        }

        public override UIType uiType {
            get {
                return UIType.mainMenu;
            }
        }
    }

}