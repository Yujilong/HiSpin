using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Slots : BaseUI
    {
        public List<SlotItem> allSlotsItems = new List<SlotItem>();
        public Button giftButton;
        public Button cashoutButton;
        public Button signButton;
        public GameObject sign_rpGo;
        public Image sign_progress_fillImage;
        public Text sign_progressText;
        protected override void Awake()
        {
            base.Awake();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            giftButton.AddClickEvent(OnGiftButtonClick);
            cashoutButton.AddClickEvent(OnCashoutButtonClick);
            signButton.AddClickEvent(OnSignButtonClick);
            RectTransform allRect = transform.GetChild(0) as RectTransform;
            if (Master.IsBigScreen)
            {
                allRect.localPosition -= new Vector3(0, Master.TopMoveDownOffset, 0);
                allRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
            }
            allRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
            if (Language_M.isJapanese)
                signButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Slots, "sign_japanese");
        }
        private void OnGiftButtonClick()
        {
            if (Ads._instance.WebViewAvailable())
                Ads._instance.ShowWebView();
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_NoGiftAd));
        }
        private void OnSignButtonClick()
        {
            if (string.IsNullOrEmpty(Save.data.allData.user_panel.user_paypal) || string.IsNullOrWhiteSpace(Save.data.allData.user_panel.user_paypal))
            {
                if (Language_M.isJapanese)
                    UI.ShowPopPanel(PopPanel.Sign);
                else
                    UI.ShowPopPanel(PopPanel.GetNewPlayerReward, 1);
            }
            else
                UI.ShowPopPanel(PopPanel.Sign);
        }
        private void OnCashoutButtonClick()
        {
            UI.ShowBasePanel(BasePanel.Cashout_Cash);
        }
        public void RefreshSlotsCardState()
        {
            int slotsCount = allSlotsItems.Count;
            int netCount = Save.data.allData.lucky_status.white_lucky.Count;
            if (slotsCount != netCount)
            {
                Debug.LogError("老虎机数量匹配错误");
                return;
            }
            for (int i = 0; i < slotsCount; i++)
            {
                int index = i;
                bool isFree = Save.data.allData.lucky_status.white_lucky[i] == 0;
                allSlotsItems[i].Init(isFree, index);
            }
        }
        protected override void BeforeShowAnimation(params int[] args)
        {
            UpdateTimedownText(Master.time);
            RefreshSlotsCardState();
            if (Save.data.allData.user_panel.lucky_count == 3 && !Save.data.hasRateus)
            {
                Save.data.hasRateus = true;
                UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Rateus);
            }
            UpdateSignState();
            if (Save.data.isPackB)
            {
                giftButton.gameObject.SetActive(true);
                StartCoroutine("AutoShakeGift");
            }
            else
                giftButton.gameObject.SetActive(false);
        }
        protected override void BeforeCloseAnimation()
        {
            StopCoroutine("AutoShakeGift");
        }
        public void UpdateSignState()
        {
            if (!Save.data.isPackB)
            {
                signButton.gameObject.SetActive(false);
            }
            else
            {
                var signData = Save.data.allData.check_task;
                if (signData.cur_day < 14)
                    signButton.gameObject.SetActive(true);
                else if (signData.cur_day == 14)
                    signButton.gameObject.SetActive(signData.flag_task);
                else
                    signButton.gameObject.SetActive(false);
                if (signData.cur_day < 14)
                    sign_rpGo.SetActive(signData.flag_task && signData.task_list[signData.cur_day] == 0);
                else
                    sign_rpGo.SetActive(true);
                int hasSignDay = 0;
                foreach (var state in signData.task_list)
                    if (state == 1)
                        hasSignDay++;
                if (signData.cur_day < 14)
                {
                    sign_progress_fillImage.fillAmount = hasSignDay / 15f;
                    sign_progressText.text = hasSignDay + "/15";
                }
                else
                {
                    sign_progress_fillImage.fillAmount = hasSignDay + 1 / 15f;
                    sign_progressText.text = (hasSignDay + 1) + "/15";
                }
            }
            //cashoutButton.gameObject.SetActive(Save.data.isPackB);
        }
        public void UpdateTimedownText(string time)
        {
            time_downText.text = nextslotsin + "\n" + time;
        }
        public void OnChangePackB()
        {
            signButton.gameObject.SetActive(true);
        }
        [Space(15)]
        public Text time_downText;
        private string nextslotsin;
        public override void SetContent()
        {
            nextslotsin = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Slots_NextSlotsIn);
        }
        [Header("引导位置")]
        public RectTransform guide_signRect;
        public RectTransform GetGuide_SignRect()
        {
            return guide_signRect;
        }
        bool isPause = false;
        public override void Pause()
        {
            isPause = true;
        }
        public override void Resume()
        {
            if (!isPause) return;
            isPause = false;
            UpdateSignState();
        }
        private IEnumerator AutoShakeGift() 
        {
            Transform giftTrans = giftButton.transform;
            giftTrans.localEulerAngles = Vector3.zero;
            float shakeScale = 30;
            float shakeSpeed = 300;
            float tempShakeSpeed = -shakeSpeed;
            int turn = 2;
            float nextInterval = 2;
            while (true)
            {
                int tempTurn = 0;
                float lastZ;
                float currentZ;
                while (tempTurn < turn)
                {
                    yield return null;
                    float z = giftTrans.localEulerAngles.z;
                    if (z > 180)
                        z -= 360;
                    if (z <= -shakeScale)
                        tempShakeSpeed = shakeSpeed;
                    else if (z >= shakeScale)
                        tempShakeSpeed = -shakeSpeed;
                    lastZ = giftTrans.localEulerAngles.z;
                    giftTrans.Rotate(new Vector3(0, 0, tempShakeSpeed * Time.deltaTime));
                    currentZ = giftTrans.localEulerAngles.z;
                    if (lastZ < 180 && currentZ >= 180)
                        tempTurn++;
                }
                giftTrans.localEulerAngles = Vector3.zero;
                yield return new WaitForSeconds(nextInterval);
            }
        }
    }
}
