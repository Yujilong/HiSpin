using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class EnterCashoutTask : PopUI,ILanguage
    {
        public Image topImage;
        public Text cashText;
        public Slider ad_taskSlider;
        public Text ad_task_progressText;
        public Slider active_taskSlider;
        public Text active_task_progressText;
        public Button closeButton;
        public Button cashoutButton;
        protected override void Awake()
        {
            base.Awake();
            closeButton.AddClickEvent(OnCloseButtonClick);
            cashoutButton.AddClickEvent(OnCashoutButtonClick);
            if (Language_M.isJapanese)
                topImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.EnterCashoutTask, "top_japan");
            else if (Language_M.isKorean)
                topImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.EnterCashoutTask, "top_korea");
        }
        private void OnCloseButtonClick()
        {
            UI.ClosePopPanel(this);
        }
        private void OnCashoutButtonClick()
        {
            if (ad_taskSlider.value == 1 || active_taskSlider.value == 1)
            {
                Save.data.hasUnlockCashout = true;
                UI.ClosePopPanel(this);
                UI.ShowBasePanel(BasePanel.Cashout_Cash);
            }
            else 
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_EnterCashoutCaution));
        }
        const int needAd = 100;
        const int needDay = 7;
        protected override void BeforeShowAnimation(params int[] args)
        {
            cashText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), (Save.data.allData.user_panel.user_doller_live / Cashout_Gold.CashToDollerRadio).GetCashShowString());
            int currentAd = Save.data.totalAdTimes;
            int currentDay = Save.data.activeTimes;
            ad_taskSlider.value = currentAd / (float)needAd;
            active_taskSlider.value = currentDay / (float)needDay;
            ad_task_progressText.text = currentAd + "/" + needAd;
            active_task_progressText.text = currentDay + "/" + needDay;
        }
        [Space(15)]
        public Text ad_task_desText;
        public Text acitve_task_desText;
        public Text tipText;
        public Text cashoutText;
        public override void SetContent()
        {
            ad_task_desText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.EnterCashoutTask_Des1);
            acitve_task_desText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.EnterCashoutTask_Des2);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.EnterCashoutTask_Tip);
            cashoutText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);
        }
    }
}
