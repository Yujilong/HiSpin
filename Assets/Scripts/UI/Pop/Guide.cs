using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Guide : PopUI
    {
        public Button bgButton;
        public Text tipText;
        public Image guideImage;
        Vector3 topTipLocalPos = new Vector3(0, 79.11f, 0);
        Vector3 downTipLocalPos = new Vector3(0, 189.19f, 0);
        protected override void Awake()
        {
            base.Awake();
            bgButton.AddClickEvent(OnBgClick);
        }
        int guideStep = 0;
        private void OnBgClick()
        {
            if (canGotoNextGuide)
                guideStep++;
        }
        protected override void BeforeShowAnimation(params int[] args)
        {
            guideStep = 1;
            canGotoNextGuide = false;
            guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_" + guideStep);
            guideImage.transform.localPosition = new Vector3(-46, Master.IsBigScreen ? 1920 * Master.ExpandCoe / 2f - 428 - Master.TopMoveDownOffset : 527, 0);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Guide1);
            tipText.transform.localPosition = topTipLocalPos;
            Master.Instance.SetGuideMask(guideStep);
            StartCoroutine("WaitForClick");
        }
        bool canGotoNextGuide = false;
        IEnumerator WaitForClick()
        {
            yield return new WaitForSeconds(1);
            canGotoNextGuide = true;
            while (guideStep == 1)
            {
                yield return null;
            }
            canGotoNextGuide = false;
            guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_" + guideStep);
            guideImage.transform.localPosition = new Vector3(20, -1920 * Master.ExpandCoe / 2f + 471, 0);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Guide2);
            tipText.transform.localPosition = downTipLocalPos;
            Master.Instance.SetGuideMask(guideStep);
            yield return new WaitForSeconds(1);
            canGotoNextGuide = true;
            while (guideStep == 2)
            {
                yield return null;
            }
            canGotoNextGuide = false;
            guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_" + guideStep);
            guideImage.transform.localPosition = new Vector3(30, -1920 * Master.ExpandCoe / 2f + 471, 0);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Guide3);
            tipText.transform.localPosition = downTipLocalPos;
            Master.Instance.SetGuideMask(guideStep);
            yield return new WaitForSeconds(1);
            canGotoNextGuide = true;
            while (guideStep == 3)
            {
                yield return null;
            }
            Master.Instance.SetGuideMask(guideStep);
            UI.ClosePopPanel(this);
        }
    }
}
