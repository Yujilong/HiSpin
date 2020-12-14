using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI
{
    public class UI_LoadingPanel : UI_PanelBase
    {
        public Slider progress_slider;
        public Text progress_text;
        public Text tip;
        protected override void Awake()
        {
            base.Awake();
            tip.gameObject.SetActive(false);
            PanelType = UI_Panel.UI_PopPanel.LoadingPanel;
            StartCoroutine(Loading());
        }
        const float maxloadingWaitTime = 5;
        IEnumerator Loading()
        {
            progress_slider.value = 0;
            progress_text.text = "0%";
            float progress = 0;
            float speed = 1;
            while (progress < 1)
            {
                yield return null;
                float deltatime = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
                progress += deltatime * speed;
                progress = Mathf.Clamp(progress, 0, 1);
                progress_slider.value = progress;
                progress_text.text = (int)(progress * 100) + "%";
            }
            UIManager.ClosePopPanelByID(UI_ID);
            UIManager.ReleasePanel(this);
            GameManager.Instance.WhenLoadingGameEnd();
        }
        protected override IEnumerator Show()
        {
            _CanvasGroup.alpha = 1;
            _CanvasGroup.blocksRaycasts = true;
            yield return null;
        }
        protected override IEnumerator Close()
        {
            _CanvasGroup.alpha = 0;
            _CanvasGroup.blocksRaycasts = false;
            yield return null;
        }
    }
}
