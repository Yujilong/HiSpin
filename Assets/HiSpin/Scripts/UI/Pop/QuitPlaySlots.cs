using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class QuitPlaySlots : PopUI
    {
        public Button noButton;
        public Button yesButton;
        protected override void Awake()
        {
            base.Awake();
            noButton.AddClickEvent(OnNoClick);
            yesButton.AddClickEvent(OnYesClick);
        }
        private void OnNoClick()
        {
            UI.ClosePopPanel(this);
        }
        private void OnYesClick()
        {
            UI.ClosePopPanel(this);
            UI.CloseCurrentBasePanel(false, true);
        }
        [Space(15)]
        public Text titleText;
        public Text tipText;
        public Text noText;
        public Text yesText;
        public override void SetContent()
        {
            titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.QuitPlaySlots_Title);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.QuitPlaySlots_Tip);
            noText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NO);
            yesText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.YES);
        }
    }
}
