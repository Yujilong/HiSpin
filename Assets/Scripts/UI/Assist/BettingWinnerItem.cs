using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HiSpin
{
    public class BettingWinnerItem : MonoBehaviour
    {
        public Image head_iconImage;
        public GameObject cash_iconGo;
        public Text nameText;
        public Text prize_cash_num_Text;
        public void Init(int head_id, string name, int cashNum)
        {
            head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_id);
            nameText.text = name;
            bool isPackB = Save.data.isPackB;
            prize_cash_num_Text.text = isPackB ? string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), cashNum.GetCashShowString()) : cashNum.GetCashShowString();
            cash_iconGo.SetActive(isPackB);
            if (Language_M.isJapanese)
                cash_iconGo.GetComponent<Image>().sprite = Sprites.GetSprite(SpriteAtlas_Name.Betting, "paypay");
        }
    }
}
