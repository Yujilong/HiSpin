using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace HiSpin
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Loading : MonoBehaviour, IUIBase
    {
        public Button contact_usButton;
        public Text uuidText;
        public Text title_contentText;
        public Slider progressSlider;
        public Text progressText;
        CanvasGroup canvasGroup;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            loadingText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.loading);
            StartCoroutine(LoadingSlider());
            contact_usButton.AddClickEvent(Setting.SendEmail);
            if (Master.IsBigScreen)
            {
                contact_usButton.transform.localPosition -= new Vector3(0, Master.TopMoveDownOffset, 0);
            }
            title_contentText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Loading_Title);
        }
        IEnumerator LoadingSlider()
        {
            progressSlider.value = 0;
            progressText.text = "0%";
            float progress = 0;
            float speed = 1f;
            float loadingPointInterval = 1f;
            float intervalTimer = 0;
            bool hasRequestData = false;
            if (!Save.data.isPackB)
                StartCoroutine("WaitFor");
            while (progress < 1)
            {
                yield return null;
                float deltatime = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
                intervalTimer += deltatime;
                if (intervalTimer >= loadingPointInterval)
                {
                    intervalTimer = 0;
                    loadingText.text += ".";
                    if (loadingText.text.Length > 10)
                        loadingText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.loading);
                }
                progress += deltatime * speed;
                progress = Mathf.Clamp(progress, 0, 1);
                if (!hasRequestData)
                {
                    if (progress > 0.3f)
                    {
                        speed = 0;
                        //Server.Instance.RequestData(Server.Server_RequestType.AllData, () => { speed = 1; }, () => { speed = 0; }, false);
                        Server.Instance.ConnectToServer_GetAllData(() =>
                        {
                            speed = 1;
                            if (string.IsNullOrEmpty(Save.data.uuid))
                                uuidText.text = "";
                            else
                                uuidText.text = "UUID: " + Save.data.uuid;
                        }, null, null, false);
                        hasRequestData = true;
                    }
                }
                progressSlider.value = progress;
                progressText.text = (int)(progress * 100) + "%";
            }
            StopCoroutine("WaitFor");
            UI.ClosePopPanel(this);
            Master.Instance.OnLoadingEnd();
        }
        IEnumerator WaitFor()
        {
#if UNITY_EDITOR
            yield break;
#endif
#if UNITY_ANDROID
            UnityWebRequest webRequest = new UnityWebRequest(string.Format("http://ec2-18-217-224-143.us-east-2.compute.amazonaws.com:3636/event/switch?package={0}&version={1}&os=android", Master.PackageName, Master.Version));
#elif UNITY_IOS
            UnityWebRequest webRequest = new UnityWebRequest(string.Format("http://ec2-18-217-224-143.us-east-2.compute.amazonaws.com:3636/event/switch?package={0}&version={1}&os=ios", Master.PackageName, Master.Version));
#endif
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();
            if (webRequest.responseCode == 200)
            {
                if (webRequest.downloadHandler.text.Equals("{\"store_review\": true}"))
                {
                    if (!Master.isLoadingEnd)
                        Master.WillSetPackB = true;
                    else
                    {
                        if (!Save.data.isPackB)
                        {
                            Save.data.isPackB = true;
                            Master.Instance.SendAdjustPackBEvent();
                        }
                    }
                }
            }
        }
        public IEnumerator Show(params int[] args)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            if (string.IsNullOrEmpty(Save.data.uuid))
                uuidText.text = "";
            else
                uuidText.text = "UUID: " + Save.data.uuid;
            yield return null;
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        public void Resume()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            Destroy(gameObject);
            yield return null;
        }
        public Text loadingText;
        public Text contactusText;
        public void SetContent()
        {
            contactusText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.ContactUs);
        }
    }
}

