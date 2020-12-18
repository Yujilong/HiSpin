using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_RateusPanel : UI_PopPanelBase
    {
        public Button noButton;
        public Button yesButton;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.RateusPanel;
            noButton.onClick.AddListener(OnNoClick);
            yesButton.onClick.AddListener(OnYesClick);
        }
        private void OnNoClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
        }
        private void OnYesClick()
        {
            GameManager.PlayButtonClickSound();
#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + HiSpin.Master.PackageName);
#elif UNITY_IOS
        var url = string.Format(
           "itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review",
            HiSpin.Master.AppleId);
        Application.OpenURL(url);
#endif
            UIManager.ClosePopPanel(this);
        }
        protected override void OnEndClose()
        {
            GameManager.ShowNextPanel();
        }
        [Space(15)]
        public Text titleText;
        public Text tipText;
        public Text noText;
        public Text yesText;
        public override void SetContent()
        {
            titleText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rateus_WindowTitle);
            tipText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rateus_Tip);
            noText.text = "1-4 " + HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rateus_Stars);
            yesText.text = "5 " + HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rateus_Stars);
        }
    }
}
