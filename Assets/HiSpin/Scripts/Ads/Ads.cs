﻿using AdGemUnity;
using FyberPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace HiSpin
{
	public class Ads : MonoBehaviour
	{
		//ios FBID 728969364687204  Hi Spin
#if UNITY_ANDROID
		private const string IS_APP_KEY = "de040b19";
		private const int AdGem_APP_ID = 3449;
		private const string Fyber_APP_ID = "123524";
		private const string Fyber_Security_Token = "08f6b24fef833c25ab8dd9591a64b14c";
#elif UNITY_IOS
	private const string IS_APP_KEY = "debe9209";
	private const int AdGem_APP_ID = 3450;
	private const string Fyber_APP_ID = "123525";
	private const string Fyber_Security_Token = "10854f9e6c64bbd9e8b8a6145144f3c5";
#endif
		public static Ads _instance;
		[NonSerialized]
		public string adDes = string.Empty;
		public const string AppName = "A048_HiSpin";
		private void Awake()
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}

		void Start()
		{
			//Dynamic config example
			IronSourceConfig.Instance.setClientSideCallbacks(true);

			string id = IronSource.Agent.getAdvertiserId();

			//IronSource.Agent.validateIntegration();

			// SDK init
		}
		OfferWallRequester offerWallRequester;
		public void InitFyber(string userid)
		{
			Fyber.With(Fyber_APP_ID)
			  .WithUserId(userid)
			.WithSecurityToken(Fyber_Security_Token)
			  .Start();
			offerWallRequester = OfferWallRequester.Create();
			offerWallRequester.Request();
			offerWallRequester.CloseOnRedirect(false);

			IronSource.Agent.setUserId(userid);
			IronSource.Agent.init(IS_APP_KEY);
			IronSource.Agent.loadInterstitial();

			AdGem.loadOfferWallBeforeShowing = true;
			AdGem.startSession(AdGem_APP_ID, false, false, true);
			AdGem.player_id = userid;

			InitWebView();
		}
		ShowOfferwallAds ofwScripts = null;
		public bool ShowOfferwallAd(Offerwall_Co _Co)
		{
#if UNITY_EDITOR
			Debug.Log("Show " + _Co + " Offerwall");
			return CheckOfferwallAvailable(_Co);
#endif
			switch (_Co)
			{
				case Offerwall_Co.IS:
					if (IronSource.Agent.isOfferwallAvailable())
					{
						IronSource.Agent.showOfferwall();
						return true;
					}
					break;
				case Offerwall_Co.AdGem:
					if (AdGem.offerWallReady)
					{
						AdGem.showOfferWall();
						return true;
					}
					break;
				case Offerwall_Co.Fyber:
					if (ofwScripts == null)
						ofwScripts = transform.GetComponent<ShowOfferwallAds>();
					if (ofwScripts.ofwAd != null)
					{
						ofwScripts.ofwAd.Start();
						ofwScripts.ofwAd = null;
						offerWallRequester = OfferWallRequester.Create();
						offerWallRequester.Request();
						offerWallRequester.CloseOnRedirect(false);
						return true;
					}
					break;
				default:
					break;
			}
			return false;
		}
		public bool CheckOfferwallAvailable(Offerwall_Co _Co)
		{
			switch (_Co)
			{
				case Offerwall_Co.IS:
					return IronSource.Agent.isOfferwallAvailable();
				case Offerwall_Co.AdGem:
					return AdGem.offerWallReady;
				case Offerwall_Co.Fyber:
					if (ofwScripts == null)
						ofwScripts = transform.GetComponent<ShowOfferwallAds>();
					return ofwScripts.ofwAd != null;
				default:
					return false;
			}
		}
		public bool ShowRewardVideo(Action rewardedCallback, int clickAdTime, string des, Action failCallback)
		{
			adDes = des;
			rewardCallback = rewardedCallback;
			rewardFailCallback = failCallback;
#if UNITY_EDITOR
			Server.Instance.ConnectToServer_WatchRvEvent(rewardedCallback, null, null, true);
			TaskAgent.TriggerTaskEvent(PlayerTaskTarget.WatchRvOnce, 1);
			Debug.Log("RV:【" + des + "】");
			return true;
#endif
#if UNITY_IOS
		if (!Save.data.isPackB)
		{
			rewardCallback?.Invoke();
			return true;
		}
#endif
			if (IronSource.Agent.isRewardedVideoAvailable())
			{
				IronSource.Agent.showRewardedVideo();
				return true;
			}
			else
			{
				StartCoroutine(WaitLoadAD(true, clickAdTime));
				return false;
			}
		}
		float interstialLasttime = 0;
		public void ShowInterstialAd(Action callback, string des)
		{
			popCallback = callback;
			adDes = des;
#if UNITY_EDITOR
			callback?.Invoke();
			Debug.Log("IV:【" + des + "】");
			return;
#endif
#if UNITY_IOS
		if (!Save.data.isPackB) 
		{
			callback?.Invoke();
			return;
		}
#endif
			if (timer - interstialLasttime < 60)
			{
				callback?.Invoke();
				return;
			}
			if (IronSource.Agent.isInterstitialReady())
			{
				interstialLasttime = timer;
				IronSource.Agent.showInterstitial();
			}
			else
			{
				callback?.Invoke();
				Master.Instance.SendAdjustPlayAdEvent(false, false, adDes);
			}
		}
		void OnApplicationPause(bool isPaused)
		{
			IronSource.Agent.onApplicationPause(isPaused);
		}
		public GameObject adLoadingTip;
		IEnumerator WaitLoadAD(bool isRewardedAd, int clickAdTime)
		{
			adLoadingTip.SetActive(true);
			StringBuilder content = new StringBuilder(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.loading));
			Text noticeText = adLoadingTip.GetComponentInChildren<Text>();
			noticeText.text = content.ToString();
			int timeOut = 6;
			while (timeOut > 0)
			{
				yield return new WaitForSeconds(Time.timeScale);
				timeOut--;
				content.Append('.');
				noticeText.text = content.ToString();
				if (isRewardedAd && IronSource.Agent.isRewardedVideoAvailable())
				{
					IronSource.Agent.showRewardedVideo();
					adLoadingTip.SetActive(false);
					yield break;
				}
			}
			adLoadingTip.SetActive(false);
			Master.Instance.SendAdjustPlayAdEvent(false, true, adDes);
			if (clickAdTime >= 2)
			{
				rewardFailCallback?.Invoke();
				Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_NoAd), 2);
			}
		}
		Action rewardCallback;
		Action rewardFailCallback;
		private bool canGetReward = false;
		public void GetReward()
		{
			canGetReward = true;
		}
		public void InvokeGetRewardMethod()
		{
			if (canGetReward)
			{
				//Server.Instance.OperationData_RvEvent(rewardCallback, null);
				Server.Instance.ConnectToServer_WatchRvEvent(rewardCallback, null, null, true);
				TaskAgent.TriggerTaskEvent(PlayerTaskTarget.WatchRvOnce, 1);
				canGetReward = false;
				Save.data.totalAdTimes++;
			}
		}
		Action popCallback;
		public void InvokePopAd()
		{
			popCallback?.Invoke();
		}
		float timer = 0;
		bool isOut = false;
		private void Update()
		{
			if (!isOut)
				timer += Time.deltaTime;
		}
		private void OnApplicationFocus(bool focus)
		{
			isOut = !focus;
		}
		[Space(15)]
		public GameObject webview;
		private UniWebView uniWebView;
		private bool uniwebFinish = false;
		private bool uniwebShow = false;
		private void InitWebView()
        {
			uniWebView = webview.AddComponent<UniWebView>();
			uniWebView.Frame = new Rect(0, 0, Screen.width, Screen.height);
			uniWebView.OnPageFinished += OnWebViewLoadFinish;
			uniWebView.OnShouldClose += OnWebViewClosed;
			uniWebView.OnPageErrorReceived += OnWebViewLoadFail;
			uniWebView.Load("https://2ds.io/links/13a20d04");
        }
		private void OnWebViewLoadFinish(UniWebView webView, int statusCode, string url)
        {
			if (statusCode == 200)
				uniwebFinish = true;
        }
		private bool OnWebViewClosed(UniWebView webView)
        {
			webView.Hide();
			uniwebShow = false;
			return false;
        }
		private void OnWebViewLoadFail(UniWebView webView, int errorCode, string errorMessage)
		{
			StartCoroutine("WaitForRequest");
		}
		private IEnumerator WaitForRequest()
        {
			yield return new WaitForSeconds(1);
			uniWebView.Load("https://2ds.io/links/13a20d04");
		}
		public void ShowWebView()
		{
			float topHeight = UI.GetMenuTopHeight();
			uniWebView.Frame = new Rect(0, topHeight, Screen.width, Screen.height - topHeight);
			uniWebView.Show();
			uniwebShow = true;
			UI.OnWebView(true);
        }
		public bool WebViewAvailable()
        {
			return uniwebFinish;
        }
		public bool WebViewShow()
        {
			return uniwebShow;
        }
		public void HideWebView()
		{
			uniWebView.Hide();
			uniwebShow = false;
			UI.OnWebView(false);
		}
	}
	public enum Offerwall_Co
	{
		IS,
		AdGem,
		Fyber
	}
}
       