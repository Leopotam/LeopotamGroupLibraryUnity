// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2019 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LeopotamGroup.Common;
using LeopotamGroup.Localization;
using UnityEngine;
using UnityEngine.Networking;

namespace LeopotamGroup.Analytics {
    /// <summary>
    /// Simple GoogleAnalytics manager. Supports tracking of events, screens.
    /// </summary>
    sealed class GoogleAnalyticsManager : MonoBehaviourService<GoogleAnalyticsManager> {
        [SerializeField]
        string _trackerId;

        /// <summary>
        /// Is TrackerID filled ans manager ready to send data.
        /// </summary>
        public bool IsInited { get { return !string.IsNullOrEmpty (_trackerId); } }

        /// <summary>
        /// Get device identifier, replacement for SystemInfo.deviceUniqueIdentifier.
        /// </summary>
        public string DeviceHash {
            get {
                if (!string.IsNullOrEmpty (_deviceHash)) {
                    return _deviceHash;
                }
                _deviceHash = PlayerPrefs.GetString (DeviceHashKey, null);
                if (!string.IsNullOrEmpty (_deviceHash)) {
                    return _deviceHash;
                }

                // Dont care about floating point regional format for double.
                var userData = string.Format ("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                    SystemInfo.graphicsDeviceVendor, SystemInfo.graphicsDeviceVersion,
                    SystemInfo.deviceModel,
                    SystemInfo.deviceName, SystemInfo.operatingSystem,
                    SystemInfo.processorCount,
                    SystemInfo.systemMemorySize, Application.systemLanguage,
                    (DateTime.UtcNow - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
                var data = new MD5CryptoServiceProvider ().ComputeHash (Encoding.UTF8.GetBytes (userData));
                var sb = new StringBuilder ();
                for (var i = 0; i < data.Length; i++) {
                    sb.Append (data[i].ToString ("x2"));
                }
                _deviceHash = sb.ToString ();
                PlayerPrefs.SetString (DeviceHashKey, _deviceHash);
#if UNITY_EDITOR
                Debug.Log ("[GA] New device hash generated: " + _deviceHash);
#endif
                return _deviceHash;
            }
        }

        const string AnalyticsUrl = "http://www.google-analytics.com/collect?v=1&tid={0}&cid={1}&sr={2}x{3}&an={4}&av={5}&z=";

        const string DeviceHashKey = "_deviceHash";

        readonly Queue<string> _requests = new Queue<string> (64);

        string _requestUrl;

        string _deviceHash;

        protected override void OnCreateService () {
            DontDestroyOnLoad (gameObject);
        }

        protected override void OnDestroyService () { }

        IEnumerator Start () {
            _requestUrl = null;

            // Wait for additional init.
            yield return null;

#if UNITY_EDITOR
            if (string.IsNullOrEmpty (_trackerId)) {
                Debug.LogWarning ("GA.TrackerID not defined");
            }
#endif
            if (!string.IsNullOrEmpty (_trackerId)) {
                _requestUrl = string.Format (AnalyticsUrl, _trackerId, DeviceHash, Screen.width,
                    Screen.height, Application.identifier, Application.version);
            }

            string url = null;
            string data;

            while (true) {
                if (_requests.Count > 0) {
                    data = _requests.Dequeue ();

                    // If tracking id defined and url inited.
                    if (!string.IsNullOrEmpty (_requestUrl)) {
                        url = string.Format ("{0}{1}&{2}&ul={3}",
                            _requestUrl, UnityEngine.Random.Range (1, 99999), data, Localizer.Language);
                    }
                }

                if (url != null) {
#if UNITY_EDITOR
                    Debug.Log ("[GA REQUEST] " + url);
#endif

                    using (var req = UnityWebRequest.Get (url)) {
                        req.SetRequestHeader("user-agent", "");
                        yield return req.SendWebRequest ();
                    }
                    url = null;
                } else {
                    yield return null;
                }
            }
        }

        void EnqueueRequest (string url) {
            _requests.Enqueue (url);
        }

        /// <summary>
        /// Track current screen.
        /// </summary>
        public void TrackScreen () {
            TrackScreen (Service<ScreenManager>.Get ().Current);
        }

        /// <summary>
        /// Track screen with custom name.
        /// </summary>
        /// <param name="screenName">Custom screen name.</param>
        public void TrackScreen (string screenName) {
            // Old version of screen tracking: EnqueueRequest (string.Format ("t=screenview&cd={0}", UnityWebRequest.EscapeURL (screenName)));
            EnqueueRequest (string.Format ("t=pageview&dp={0}", UnityWebRequest.EscapeURL (screenName)));
        }

        /// <summary>
        /// Track event.
        /// </summary>
        /// <param name="category">Category name.</param>
        /// <param name="action">Action name.</param>
        public void TrackEvent (string category, string action) {
            EnqueueRequest (string.Format ("t=event&ec={0}&ea={1}", UnityWebRequest.EscapeURL (category), UnityWebRequest.EscapeURL (action)));
        }

        /// <summary>
        /// Track event.
        /// </summary>
        /// <param name="category">Category name.</param>
        /// <param name="action">Action name.</param>
        /// <param name="label">Label name.</param>
        /// <param name="value">Value.</param>
        public void TrackEvent (string category, string action, string label, string value) {
            EnqueueRequest (string.Format ("t=event&ec={0}&ea={1}&el={2}&ev={3}",
                UnityWebRequest.EscapeURL (category),
                UnityWebRequest.EscapeURL (action),
                UnityWebRequest.EscapeURL (label),
                UnityWebRequest.EscapeURL (value)
            ));
        }

        /// <summary>
        /// Track transaction for e-commerce, in-app purchases.
        /// </summary>
        /// <param name="transactionId">Transaction ID, will be truncated up to 100 symbols.</param>
        /// <param name="productName">Product name.</param>
        /// <param name="sku">Product code.</param>
        /// <param name="price">Product price.</param>
        /// <param name="currency">ISO currency code, 3 letters. USD by default</param>
        public void TrackTransaction (string transactionId, string productName, string sku, decimal price, string currency = "USD") {
            transactionId = (transactionId.Length <= 100) ? transactionId : transactionId.Substring (0, 100);
            EnqueueRequest (string.Format ("t=transaction&ti={0}&tr={1}&cu={2}&ts=0&tt=0",
                UnityWebRequest.EscapeURL (transactionId),
                price,
                UnityWebRequest.EscapeURL (currency)
            ));
            EnqueueRequest (string.Format ("t=item&ti={0}&in={1}&ic={2}&ip={3}&iq=1&cu={4}",
                UnityWebRequest.EscapeURL (transactionId),
                UnityWebRequest.EscapeURL (productName),
                UnityWebRequest.EscapeURL (sku),
                price,
                UnityWebRequest.EscapeURL (currency)
            ));
        }

        /// <summary>
        /// Track exception event.
        /// </summary>
        /// <param name="description">Description of exception.</param>
        /// <param name="isFatal">Is exception fatal.</param>
        public void TrackException (string description, bool isFatal) {
            EnqueueRequest (string.Format ("t=exception&exd={0}&exf={1}", UnityWebRequest.EscapeURL (description), isFatal ? 1 : 0));
        }
    }
}