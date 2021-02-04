using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Cashou_Cash : BaseUI,ILanguage
    {
        public RectTransform topRect;
        public Button backButton;
        public Text cashText;
        public List<Button> all_cashoutButtons;
        public Image firstIcon;
        protected override void Awake()
        {
            base.Awake();
            backButton.AddClickEvent(OnBackButtonClick);
            foreach (var cashout in all_cashoutButtons)
                cashout.AddClickEvent(OnCashouButtonClick);
            if (Language_M.isJapanese)
                firstIcon.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout_Cash, "paypay");
            else if (Language_M.isKorean)
                firstIcon.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout_Cash, "naverpay");
            if (Master.IsBigScreen)
                topRect.sizeDelta += new Vector2(0, Master.TopMoveDownOffset);
        }
        private void OnBackButtonClick()
        {
            UI.CloseCurrentBasePanel();
        }
        private void OnCashouButtonClick()
        {
            Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_CashoutGiftCaution));
        }
        protected override void BeforeShowAnimation(params int[] args)
        {
            cashText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), (Save.data.allData.user_panel.user_doller_live / Cashout_Gold.CashToDollerRadio).GetCashShowString());
        }
        [Space(15)]
        public Text titleText;
        public List<Text> all_cashoutText;
        public override void SetContent()
        {
            titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.REDEEM);
            string cashoutNumString;
            if (Language_M.isJapanese)
                cashoutNumString = "20000";
            else if (Language_M.isKorean)
                cashoutNumString = "200000";
            else
                cashoutNumString = "200";
            string dollar = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), cashoutNumString);
            foreach (var cashout in all_cashoutText)
                cashout.text = dollar;
        }
    }
}
