using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class InputInviteCode : PopUI
    {
        public Button closeButton;
        public InputField invite_codeInput;
        public Button okButton;
        public Text timedownText;
        protected override void Awake()
        {
            base.Awake();
            closeButton.AddClickEvent(OnCloseClick);
            okButton.AddClickEvent(OnOkButtonClick);
            timedownText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputInviteCode_TimeDown), "23:59:59");
        }
        private void OnCloseClick()
        {
            UI.ClosePopPanel(this);
        }
        private void OnOkButtonClick()
        {
            string codeString = invite_codeInput.text;
            if (string.IsNullOrEmpty(codeString) || string.IsNullOrWhiteSpace(codeString))
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputInviteCode_BindFail));
            else
                Server.Instance.ConnectToServer_BindInviteCode(OnSuccessBindCallback, null, null, true, codeString);
        }
        private void OnSuccessBindCallback()
        {
            Save.data.allData.fission_info.up_user = true;
            Master.Instance.SendAdjustBindInviteCode();
            Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputInviteCode_BindSuccess));
            UI.ClosePopPanel(this);
        }
        bool hasAutoClose = false;
        public void UpdateTimedownText()
        {
            if (hasAutoClose) return;
            int totalSeconds = Save.data.allData.invita_time;
            if (totalSeconds == 0)
            {
                UI.ClosePopPanel(this);
                hasAutoClose = true;
            }
            else
                timedownText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputInviteCode_TimeDown), totalSeconds.TotalSecondsTo24hTime());
        }
        [Space(15)]
        public Text inputplaceholderText;
        public Text okText;
        public override void SetContent()
        {
            inputplaceholderText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputInviteCode_Placeholder);
            okText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputInviteCode_Ok);
        }
    }
}
