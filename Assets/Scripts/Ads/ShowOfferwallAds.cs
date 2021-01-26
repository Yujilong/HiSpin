using FyberPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HiSpin
{
    public class ShowOfferwallAds : MonoBehaviour
    {
        public Ad ofwAd = null;
        void OnEnable()
        {

            FyberCallback.AdAvailable += OnAdAvailable;
            FyberCallback.AdNotAvailable += OnAdNotAvailable;
            FyberCallback.RequestFail += OnRequestFail;
        }
        #region fyber offerwall
        private void OnAdAvailable(Ad ad)
        {
            // store ad response
            if (ad.AdFormat == AdFormat.OFFER_WALL)
                ofwAd = ad;
        }

        private void OnAdNotAvailable(AdFormat adFormat)
        {
            // discard previous stored response
            if (adFormat == AdFormat.OFFER_WALL)
                ofwAd = null;
        }

        private void OnRequestFail(RequestError error)
        {
            // process error
            Debug.Log("OnRequestError: " + error.Description);
        }
        #endregion
        void OnDisable()
        {
            FyberCallback.AdAvailable -= OnAdAvailable;
            FyberCallback.AdNotAvailable -= OnAdNotAvailable;
            FyberCallback.RequestFail -= OnRequestFail;
        }
    }
}
