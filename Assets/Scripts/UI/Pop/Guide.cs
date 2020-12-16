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
        private RectTransform guide_base_Rect;
        public Image guide_arrowImage;
        private RectTransform guide_arrow_Rect;
        public Image bg;
        public RectTransform bgRect;
        protected override void Awake()
        {
            base.Awake();
            guide_base_Rect = guideImage.GetComponent<RectTransform>();
            guide_arrow_Rect = guide_arrowImage.GetComponent<RectTransform>();
            bgButton.AddClickEvent(OnBgClick);
        }
        int guideStep = 0;
        private void OnBgClick()
        {
            if (canGotoNextGuide)
                guideStep++;
        }
        string Shader_Property_Pos = "_Center";
        string Shader_Property_Width = "_Width";
        string Shader_Property_Height = "_Height";
        string Shader_Property_Softness = "_Ellipse";
        protected override void BeforeShowAnimation(params int[] args)
        {
            guideStep = args[0];
            canGotoNextGuide = false;

            Vector2 localPos = SetMaskShape();

            //Master.Instance.SetGuideMask(guideStep);
            guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_base" + guideStep);
            guide_arrowImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_arrow" + guideStep);
            guide_arrowImage.transform.localPosition = localPos - new Vector2(0, 100);
            guide_base_Rect.localPosition = new Vector3(0, localPos.y - guide_arrow_Rect.rect.height - guide_base_Rect.rect.height / 2f - 100, 0);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Guide1);
            StartCoroutine("WaitForClick");
        }
        bool canGotoNextGuide = false;
        IEnumerator WaitForClick()
        {
            if (guideStep < 0)
            {
                yield return new WaitForSeconds(1);
                canGotoNextGuide = true;
                while (guideStep == -1)
                {
                    yield return null;
                }
                UI.ClosePopPanel(this);
                yield break;
            }
            yield return new WaitForSeconds(1);
            canGotoNextGuide = true;
            while (guideStep == 1)
            {
                yield return null;
            }
            canGotoNextGuide = false;

            Vector2 localPos2 = SetMaskShape();

            guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_base" + guideStep);
            guide_arrowImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_arrow" + guideStep);
            guide_arrowImage.transform.localPosition = localPos2 + new Vector2(0, guide_arrow_Rect.rect.height + 100);
            guide_base_Rect.localPosition = new Vector3(0, localPos2.y + guide_arrow_Rect.rect.height + guide_base_Rect.rect.height / 2f + 100, 0);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Guide2);
            yield return new WaitForSeconds(1);
            canGotoNextGuide = true;
            while (guideStep == 2)
            {
                yield return null;
            }
            canGotoNextGuide = false;

            Vector2 localPos3 = SetMaskShape();

            guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_base" + guideStep);
            guide_arrowImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_arrow" + guideStep);
            guide_arrowImage.transform.localPosition = localPos3 + new Vector2(0, guide_arrow_Rect.rect.height + 100);
            guide_base_Rect.localPosition = new Vector3(0, localPos3.y + guide_arrow_Rect.rect.height + guide_base_Rect.rect.height / 2f + 100, 0);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Guide3);
            yield return new WaitForSeconds(1);
            canGotoNextGuide = true;
            while (guideStep == 3)
            {
                yield return null;
            }
            UI.ClosePopPanel(this);
        }
        private Vector2 SetMaskShape()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(bgRect, UI.MenuPanel.GetGuideScreenPos(guideStep), null, out Vector2 localPos);
            Material bgMat = bg.material;
            bgMat.SetVector(Shader_Property_Pos, new Vector4(localPos.x, localPos.y));
            switch (guideStep)
            {
                case 1:
                    bgMat.SetFloat(Shader_Property_Width, 200);
                    bgMat.SetFloat(Shader_Property_Height, 100);
                    bgMat.SetFloat(Shader_Property_Softness, 3.26f);
                    break;
                case -1:
                case 2:
                case 3:
                    bgMat.SetFloat(Shader_Property_Width, 150);
                    bgMat.SetFloat(Shader_Property_Height, 150);
                    bgMat.SetFloat(Shader_Property_Softness, 2);
                    break;
            }
            return localPos;
        }
    }
}
