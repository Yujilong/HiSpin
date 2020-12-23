using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class CashoutRecordItem : MonoBehaviour
    {
        public Image consume_iconImage;
        public Text consume_numText;
        public Text consume_timeText;
        public Text resultText;
        public Text cashout_numText;
        public Button helpButton;
        private void Awake()
        {
            helpButton.AddClickEvent(OnHelpButtonClick);
        }
        public void Init(CashoutType comsumeType, int consumeNum, string consumeTime, int result, int cashNum)
        {
            switch (comsumeType)
            {
                case CashoutType.PT:
                    consume_numText.text = consumeNum.GetTokenShowString() + " <size=60>" + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.PT) + "</size>";
                    consume_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout, "PT");
                    break;
                case CashoutType.Cash:
                    consume_numText.text = consumeNum.GetBigTokenString();
                    consume_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout, "Cash");
                    break;
                case CashoutType.Blue_Cash:
                    consume_numText.text = consumeNum.GetCashShowString();
                    consume_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout, "Paypal");
                    break;
                case CashoutType.FriendEvent_Cash:
                    consume_numText.text = consumeNum.GetCashShowString();
                    consume_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout, "Cash");
                    break;
                case CashoutType.Gold:
                default:
                    consume_numText.text = consumeNum.GetTokenShowString();
                    consume_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout, "Gold");
                    break;
            }
            consume_timeText.text = consumeTime;

            cashout_numText.text = "+" + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + cashNum.GetTokenShowString();
            switch (result)
            {
                case 0:
                    resultText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CashoutRecord_Reviewing);
                    helpButton.gameObject.SetActive(false);
                    break;
                case 1:
                    resultText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CashoutRecord_Succeed);
                    helpButton.gameObject.SetActive(false);
                    break;
                case 2:
                    resultText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Failed) + "    ";
                    helpButton.gameObject.SetActive(true);
                    break;
            }
        }
        private static void OnHelpButtonClick()
        {
            UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.FailHelp);
        }
    }
}
