using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HiSpin
{
    public class CardItem : MonoBehaviour
    {
        public Image head_iconImage;
        public Text idText;
        public Text numText;
        public CanvasGroup onCg;
        public RectTransform cash_numRect;
        public RectTransform cash_iconRect;
        public GameObject paypal_iconGo;
        public void Init(int head_icon_index, string id, int cashNum)
        {
            head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_icon_index);
            idText.text = id;
            numText.text = Save.data.isPackB ? string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), cashNum.GetCashShowString()) : cashNum.GetCashShowString();
            paypal_iconGo.SetActive(Save.data.isPackB);
            if (Language_M.isJapanese)
                paypal_iconGo.GetComponent<Image>().sprite = Sprites.GetSprite(SpriteAtlas_Name.StartBetting, "paypay");
            else if(Language_M.isKorean)
                paypal_iconGo.GetComponent<Image>().sprite = Sprites.GetSprite(SpriteAtlas_Name.StartBetting, "naverpay");
            StartCoroutine(AutoOn());
            StartCoroutine(AutoDealyOrder());
        }
        public void SetOff()
        {
            onCg.alpha = 0;
        }
        IEnumerator AutoOn()
        {
            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime;
                onCg.alpha = progress;
                yield return null;
            }
        }
        IEnumerator AutoDealyOrder()
        {
            yield return null;
            if (!Save.data.isPackB)
            {
                cash_numRect.localPosition = new Vector3(0, cash_numRect.localPosition.y);
                yield break;
            }
            float totalWidth = cash_iconRect.sizeDelta.x + cash_numRect.sizeDelta.x + 10;
            float x = totalWidth / 2;
            cash_numRect.localPosition = new Vector3(x - cash_numRect.sizeDelta.x / 2, cash_numRect.localPosition.y);
            cash_iconRect.localPosition = new Vector3(-(x - cash_iconRect.sizeDelta.x / 2), cash_iconRect.localPosition.y);
        }
    }
}
