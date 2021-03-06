using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace LoL.Fluency
{
    public class LoLFluencySDK : MonoBehaviour
    {
        const string _TestTypingData = @"{""gameType"":""TYPING"",""version"":""1.0.0""}";
        const string _TestAssessData = @"{""gameType"":""ASSESS"",""version"":""1.0.0"",""facts"":[{""a"":1,""b"":2,""op"":""ADD""},{""a"":4,""b"":1,""op"":""ADD""},{""a"":11,""b"":10,""op"":""SUB""},{""a"":2,""b"":3,""op"":""ADD""},{""a"":2,""b"":2,""op"":""SUB""},{""a"":6,""b"":1,""op"":""SUB""},{""a"":6,""b"":5,""op"":""SUB""},{""a"":1,""b"":6,""op"":""ADD""},{""a"":7,""b"":6,""op"":""SUB""},{""a"":1,""b"":1,""op"":""ADD""},{""a"":14,""b"":7,""op"":""SUB""},{""a"":0,""b"":1,""op"":""ADD""},{""a"":1,""b"":1,""op"":""SUB""},{""a"":0,""b"":7,""op"":""ADD""},{""a"":7,""b"":7,""op"":""SUB""},{""a"":0,""b"":9,""op"":""ADD""},{""a"":9,""b"":0,""op"":""ADD""},{""a"":9,""b"":9,""op"":""SUB""},{""a"":4,""b"":4,""op"":""ADD""},{""a"":8,""b"":4,""op"":""SUB""},{""a"":5,""b"":5,""op"":""ADD""},{""a"":10,""b"":5,""op"":""SUB""},{""a"":6,""b"":6,""op"":""ADD""},{""a"":12,""b"":6,""op"":""SUB""},{""a"":8,""b"":8,""op"":""ADD""}]}";
        const string _TestEstablishData = @"{""gameType"":""ESTABLISH"",""version"":""1.0.0"",""concept"":""ADDITION"",""factFamilies"":[{""factFamily"":[{""a"":9,""b"":9,""op"":""ADD""},{""a"":18,""b"":9,""op"":""SUB""}]},{""factFamily"":[{""a"":8,""b"":8,""op"":""ADD""},{""a"":16,""b"":8,""op"":""SUB""}]}],""facts"":[{""a"":9,""b"":9,""op"":""ADD""},{""a"":3,""b"":1,""op"":""ADD""},{""a"":16,""b"":8,""op"":""SUB""},{""a"":10,""b"":9,""op"":""SUB""},{""a"":18,""b"":9,""op"":""SUB""},{""a"":7,""b"":0,""op"":""ADD""},{""a"":9,""b"":9,""op"":""ADD""},{""a"":10,""b"":9,""op"":""SUB""},{""a"":10,""b"":1,""op"":""SUB""},{""a"":16,""b"":8,""op"":""SUB""},{""a"":6,""b"":1,""op"":""ADD""},{""a"":4,""b"":3,""op"":""SUB""},{""a"":18,""b"":9,""op"":""SUB""},{""a"":0,""b"":0,""op"":""SUB""},{""a"":2,""b"":1,""op"":""SUB""},{""a"":16,""b"":8,""op"":""SUB""},{""a"":8,""b"":1,""op"":""ADD""},{""a"":5,""b"":2,""op"":""SUB""},{""a"":10,""b"":1,""op"":""SUB""},{""a"":9,""b"":9,""op"":""ADD""},{""a"":6,""b"":3,""op"":""SUB""},{""a"":10,""b"":9,""op"":""SUB""},{""a"":10,""b"":1,""op"":""SUB""},{""a"":18,""b"":9,""op"":""SUB""},{""a"":2,""b"":2,""op"":""ADD""},{""a"":3,""b"":1,""op"":""SUB""},{""a"":8,""b"":8,""op"":""ADD""}],""targetFacts"":[{""a"":9,""b"":9,""op"":""ADD""},{""a"":18,""b"":9,""op"":""SUB""},{""a"":16,""b"":8,""op"":""SUB""}]}";
        const string _TestPracticeData = @"{""gameType"":""PRACTICE"",""version"":""1.0.0"",""facts"":[{""a"":9,""b"":9,""op"":""ADD""},{""a"":18,""b"":9,""op"":""SUB""},{""a"":16,""b"":8,""op"":""SUB""},{""a"":2,""b"":1,""op"":""SUB""},{""a"":1,""b"":0,""op"":""ADD""},{""a"":3,""b"":1,""op"":""SUB""},{""a"":4,""b"":3,""op"":""SUB""},{""a"":6,""b"":1,""op"":""ADD""},{""a"":10,""b"":9,""op"":""SUB""},{""a"":1,""b"":8,""op"":""ADD""},{""a"":8,""b"":1,""op"":""ADD""},{""a"":10,""b"":1,""op"":""SUB""},{""a"":0,""b"":0,""op"":""ADD""},{""a"":2,""b"":2,""op"":""ADD""},{""a"":4,""b"":2,""op"":""SUB""},{""a"":8,""b"":1,""op"":""SUB""},{""a"":2,""b"":0,""op"":""SUB""},{""a"":9,""b"":1,""op"":""SUB""},{""a"":6,""b"":3,""op"":""SUB""},{""a"":7,""b"":7,""op"":""ADD""},{""a"":0,""b"":0,""op"":""SUB""},{""a"":5,""b"":3,""op"":""SUB""},{""a"":5,""b"":2,""op"":""SUB""},{""a"":1,""b"":7,""op"":""ADD""},{""a"":0,""b"":2,""op"":""ADD""},{""a"":7,""b"":0,""op"":""ADD""},{""a"":2,""b"":1,""op"":""ADD""},{""a"":3,""b"":1,""op"":""ADD""}],""targetFacts"":[{""a"":9,""b"":9,""op"":""ADD""},{""a"":18,""b"":9,""op"":""SUB""},{""a"":16,""b"":8,""op"":""SUB""}]}";

#if UNITY_WEBGL
        [DllImport("__Internal")]
        static extern void _GameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion, string sdkParams);
        [DllImport("__Internal")]
        static extern void _PostWindowMessage (string msg, string payload);
#else
        static void _GameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion, string sdkParams)
        {
            // no op
        }

        static void _PostWindowMessage (string msg, string payload)
        {
            // no op
        }
#endif
        static void _TestGameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion, string sdkParams)
        {
            if (_FluencyClientInfo.GameType.HasFlag(GameType.TYPING))
            {
                _TestGameType = GameType.NONE;
            }
            else if (_FluencyClientInfo.GameType.HasFlag(GameType.ASSESS))
            {
                _TestGameType = GameType.TYPING;
            }
            else if (_FluencyClientInfo.GameType.HasFlag(GameType.ESTABLISH))
            {
                _TestGameType = GameType.ASSESS;
            }
            else if (_FluencyClientInfo.GameType.HasFlag(GameType.PRACTICE))
            {
                _TestGameType = GameType.ESTABLISH;
            }

            SendTestStartData();

            SendLocalLanguageJson();
        }

        static GameType _TestGameType;
        static string GetTestSessionData ()
        {
            switch (_TestGameType)
            {
                case GameType.NONE:
                    _TestGameType = GameType.TYPING;
                    return _TestTypingData; 
                case GameType.TYPING:
                    _TestGameType = GameType.ASSESS;
                    return _TestAssessData;
                case GameType.ASSESS:
                    _TestGameType = GameType.ESTABLISH;
                    return _TestEstablishData;
                case GameType.ESTABLISH:
                    _TestGameType = GameType.PRACTICE;
                    return _TestPracticeData;
                case GameType.PRACTICE: // Keep playing.
                default:
                    return null;
            }
        }

        static void SendTestStartData ()
        {
            // Get session data progresses the _TestGameType to the next game state.
            string json = GetTestSessionData();

            json = JsonUtility.ToJson(new KeyValueData { key = "start", value = json });
            _Instance.ReceiveData(json);
        }

        static void _TestPostWindowMessage (string msg, string payload)
        {
            Debug.Log("Post window message: " + msg + " : " + payload);
            string json;
            switch (msg)
            {
                case "loadState":
                    json = PlayerPrefs.GetString(_PlayerPrefsKey);
                    break;
                case "saveState":
                    msg = "saveStateResult";
                    json = JsonUtility.ToJson(new SaveStateResults { success = true });
                    break;
                case "results":
                    SendTestStartData();
                    return;
                default: // Start doesn't have a return receiver.
                    return;
            }

            json = JsonUtility.ToJson(new KeyValueData { key = msg, value = json });
            _Instance.ReceiveData(json);
        }

        static void SendLocalLanguageJson ()
        {
            var streamingLangPath = System.IO.Path.Combine(Application.streamingAssetsPath, "language.json");
            if (System.IO.File.Exists(streamingLangPath))
            {
                var json = System.IO.File.ReadAllText(streamingLangPath);
                _Instance.ReceiveData(JsonUtility.ToJson(new KeyValueData { key = "language", value = json }));
            }
        }

        static LoLFluencySDK _Instance;
        static LoLFluencySDK CreateSDK ()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject($"__{nameof(LoLFluencySDK)}__").AddComponent<LoLFluencySDK>();
                _FluencyClientInfo = new FluencyClientInfo();
                DontDestroyOnLoad(_Instance.gameObject);
            }

            return _Instance;
        }

        void OnDestroy ()
        {
            _Instance = null;
            _FluencyClientInfo = null;
            _OnGameStart = null;
            _OnLoadState = null;
            _OnSaveStateResults = null;
            _GameLanguage = null;
            _SessionResults = null;
            _SessionStartData = null;
            Game_Is_Ready = null;
            Post_Message = null;
        }

        static FluencyClientInfo _FluencyClientInfo;

        static Action<GameType> _OnGameStart;
        static Action<string> _OnLoadState;
        static Action<bool> _OnSaveStateResults;

        static Dictionary<string, string> _GameLanguage;
        static IResultable _SessionResults;
        static ISessionStartData _SessionStartData;
        const string _EmptyJSON = "{}";

        static Action<string, string, string, string, string> Game_Is_Ready;
        static Action<string, string> Post_Message;

        public static LoLFluencySDK InitEmbeddedPlayer (
            Action<string, string, string, string, string> embeddedGameIsReady,
            Action<string, string> embeddedPostMessage)
        {
            Game_Is_Ready = embeddedGameIsReady;
            Post_Message = embeddedPostMessage;
            return CreateSDK();
        }

        public static void Init (Action<GameType> onGameStart, params GameType[] supportedGameTypes)
        {
            if (onGameStart == null)
            {
                Debug.LogError("[LoLFluencySDK] " + nameof(onGameStart) + " callback must be set.");
                return;
            }

            if (Game_Is_Ready == null)
            {
                if (Debug.isDebugBuild)
                    Game_Is_Ready = _TestGameIsReady;
                else
                    Game_Is_Ready = _GameIsReady;
            }

            if (Post_Message == null)
            {
                if (Debug.isDebugBuild)
                    Post_Message = _TestPostWindowMessage;
                else
                    Post_Message = _PostWindowMessage;
            }

            CreateSDK();
            _OnGameStart = onGameStart;
            foreach (var gameType in supportedGameTypes)
            {
                _FluencyClientInfo._gameType.value |= gameType;
            }

            _FluencyClientInfo._gameType.isSet = true;
        }

        /// <summary>
        /// Inform the Fluency Player that the client has initialized all game types it supports and is ready to start the session.
        /// </summary>
        /// <param name="options"></param>
        public static void GameIsReady (GameOptions options = null)
        {
            if (_Instance == null)
            {
                Debug.LogError("[LoLFluencySDK] Call Init before calling GameIsReady.");
                return;
            }

            // Call into jslib.
            var sdkOptions = new SDKOptions
            {
                gameOptions = options
            };

            sdkOptions.supportedReceiverKeys = new string[_Receivers.Keys.Count];
            _Receivers.Keys.CopyTo(sdkOptions.supportedReceiverKeys, 0);
            var sdkOptionsJson = JsonUtility.ToJson(sdkOptions);

            Game_Is_Ready(Application.productName, _Instance.gameObject.name, nameof(ReceiveData), FluencyClientInfo.Version, sdkOptionsJson);
        }

        public static TData GetStartData<TData> () where TData : class, ISessionStartData
        {
            if (_Instance == null)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning($"[LoLFluencySDK] Using testing start data for {typeof(TData)}.");
                    Game_Is_Ready = _TestGameIsReady;
                    Post_Message = _TestPostWindowMessage;
                    var gameType = GameType.PRACTICE;
                    var dataType = typeof(TData);
                    if (dataType == typeof(TypingData))
                        gameType |= GameType.TYPING | GameType.ASSESS | GameType.ESTABLISH;
                    else if (dataType == typeof(AssessData))
                        gameType |= GameType.ASSESS | GameType.ESTABLISH;
                    else if (dataType == typeof(EstablishData))
                        gameType |= GameType.ESTABLISH;

                    Init(gt => { }, gameType);
                    GameIsReady();
                }
                else
                {
                    Debug.LogError("[LoLFluencySDK] Trying to get start data before GameIsReady.");
                    return null;
                }
            }

            if (_SessionStartData is TData startData)
            {
                return startData;
            }

            Debug.LogError("[LoLFluencySDK] Trying to get invalid start data for current session game type: " + _FluencyClientInfo.playerGameType.ToString());
            return null;
        }

        static void PostWindowMessage (string msg, string json)
        {
            var postWindowMessage = Post_Message ?? _PostWindowMessage;
            postWindowMessage(msg, json);
        }

        /// <summary>
        /// Send session results to the Fluency Player.
        /// <para>
        /// For game type PRACTICE, results are also sent on an interval.
        /// </para>
        /// </summary>
        public static void SendResults ()
        {
            if (_SessionResults == null)
            {
                Debug.LogError("[LoLFluencySDK] results not sent. Did you call GameIsReady first?");
                return;
            }

            // Call into jslib.
            var json = _SessionResults.SerializeAndClear();
            // Empty results.
            if (string.IsNullOrEmpty(json))
                return;

            PostWindowMessage("results", json);
        }

        /// <summary>
        /// Add an answer result to the session.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="operation"></param>
        /// <param name="answer"></param>
        /// <param name="startTime"></param>
        /// <param name="latencyMS"></param>
        /// <returns></returns>
        public static bool AddResult (int a, int b, FluencyFactOperation operation, int? answer, DateTime startTime, int latencyMS)
        {
            if (_SessionResults == null)
            {
                Debug.LogError("[LoLFluencySDK] results not set. Did you call GameIsReady first?");
                return false;
            }

            _SessionResults.AddResult(a, b, operation, answer, startTime, latencyMS);
            return true;
        }

        /// <summary>
        /// Get localized text for a language key.
        /// Use default if not found or not initialized by the Fluency Player.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetLanguageText (string key, string defaultValue = null)
        {
            if (_Instance == null || _GameLanguage == null)
            {
                Debug.Log("[LoLFluencySDK] game language not set from fluency player");
                return defaultValue;
            }

            if (!_GameLanguage.TryGetValue(key, out defaultValue))
            {
                Debug.LogWarning("[LoLFluencySDK] language key not found: " + key);
            }

            return defaultValue;
        }

        public static void SpeakText (string key)
        {
            PostWindowMessage("speakText", $@"{{ ""key"": ""{key}"" }}");
        }

        public static void CancelSpeakText ()
        {
            PostWindowMessage("speakTextCancel", _EmptyJSON);
        }

        public static void LoadGameState<T> (Action<T> onLoaded) where T : class
        {
            SetOnStateLoadedCallback(onLoaded);

            RequestLoadStateData();
        }

        static void RequestLoadStateData ()
        {
            if (_Instance._timerRoutine != null)
            {
                _Instance.StopCoroutine(_Instance._timerRoutine);
            }
            _Instance._timerRoutine = _Instance.StartCoroutine(_Instance._LoadDataTimeOuter());

            PostWindowMessage("loadState", _EmptyJSON);
        }

        static void SetOnStateLoadedCallback<T> (Action<T> onLoaded) where T : class
        {
            _OnLoadState = json =>
            {
                if (string.IsNullOrEmpty(json) || !json.StartsWith("{") || json == _EmptyJSON)
                {
                    // Try for local state. Don't save remote if found.
                    var localState = GetLocal();
                    onLoaded(localState?.data);
                    return;
                }

                var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
                System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

                var remoteStateData = JsonUtility.FromJson<State<T>>(json);
                var remoteTimestamp = new DateTime(remoteStateData.timestamp, DateTimeKind.Utc);

                try
                {
                    var localState = GetLocal();
                    if (!(localState == null))
                    {
                        var localTimestamp = new DateTime(localState.timestamp, DateTimeKind.Utc);
                        if (localTimestamp > remoteTimestamp)
                        {
                            // Local data is more recent than remote data. Send to remote.
                            SaveGameState(localState);
                            remoteStateData = localState;
                        }
                    }
                }
                catch (Exception e)
                {
                    // In case player prefs fails for some reason (iframe in safari).
                    Debug.LogWarning("Playerprefs state read failed: " + e);
                }

                onLoaded(remoteStateData.data);
                System.Globalization.CultureInfo.CurrentCulture = currentCulture;
            };

            State<T> GetLocal ()
            {
                if (_PlayerPrefsKey != null && PlayerPrefs.HasKey(_PlayerPrefsKey))
                {
                    var localJson = PlayerPrefs.GetString(_PlayerPrefsKey);
                    return JsonUtility.FromJson<State<T>>(localJson);
                }

                return null;
            }
        }

        private Coroutine _timerRoutine;
        private readonly WaitForSeconds _loadStateTimeoutTime = new WaitForSeconds(10f);
        private readonly WaitForSeconds _sendResultsTimeoutTime = new WaitForSeconds(10f);

        private IEnumerator _LoadDataTimeOuter ()
        {
            yield return _loadStateTimeoutTime;
            _timerRoutine = null;
            Debug.Log("Remote state load timed out, continuing load with null data.");
            _OnLoadState?.Invoke(null);
        }

        private IEnumerator _SendResultOnInterval ()
        {
            // Auto send results on play game type only.
            if (_FluencyClientInfo.playerGameType != GameType.PRACTICE)
                yield break;

            while (true)
            {
                yield return _sendResultsTimeoutTime;
                SendResults();
            }
        }

        public static void SaveGameState<T> (T data, Action<bool> onSaveStateResult = null) where T : class
        {
            _OnSaveStateResults = onSaveStateResult;

            var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            var state = new State<T>()
            {
                data = data,
                timestamp = DateTime.UtcNow.Ticks
            };

            var json = JsonUtility.ToJson(state);
            try
            {
                if (_PlayerPrefsKey != null)
                {
                    PlayerPrefs.SetString(_PlayerPrefsKey, json);
                }
            }
            catch (Exception e)
            {
                // In case player prefs fails for some reason (iframe in safari).
                Debug.LogWarning("Playerprefs state write failed: " + e);
            }

            System.Globalization.CultureInfo.CurrentCulture = currentCulture;
            PostWindowMessage("saveState", json);
        }

        static long _GetUserId
        {
            get
            {
                if (_FluencyClientInfo?.userId == default)
                    return -1;

                return _FluencyClientInfo.userId;
            }
        }

        static string _playerPrefsKey;
        static string _PlayerPrefsKey
        {
            get
            {
                if (string.IsNullOrEmpty(_playerPrefsKey))
                {
                    _playerPrefsKey = string.Format("{0}_{1}",
                        _FluencyClientInfo.playerGameType.ToString().ToLower(),
                        _GetUserId);
                }

                return _playerPrefsKey;
            }
        }

        static Dictionary<string, Action<string>> _Receivers = new Dictionary<string, Action<string>>
        {
            ["start"] = ReceiveStartData,
            ["loadState"] = ReceiveLoadStateData,
            ["language"] = ReceiveLanguageJson,
            ["saveStateResult"] = ReceiveSaveStateResult,
            ["useTestData"] = ReceiveUseTestData,
        };

        // called from jslib.
        public void ReceiveData (string json)
        {
            if (string.IsNullOrEmpty(json) || !json.StartsWith("{") || json == _EmptyJSON)
            {
                Debug.LogError("[LoLFluencySDK] Received incorrect json from fluency player: " + json);
                return;
            }

            var msgData = JsonUtility.FromJson<KeyValueData>(json);
            var key = msgData.key;

            if ((key == "useTestData" && !Debug.isDebugBuild) || !_Receivers.TryGetValue(key, out var receiver))
            {
                Debug.LogWarning("[LoLFluencySDK] Receiver not supported in sdk: " + key);
                return;
            }

            receiver?.Invoke(msgData.value);
        }

        // message receivers

        static void ReceiveUseTestData (string _)
        {
            if (!Debug.isDebugBuild)
                return;

            Game_Is_Ready = _TestGameIsReady;
            Post_Message = _TestPostWindowMessage;
            GameIsReady();
        }

        static void ReceiveStartData (string json)
        {
            if (string.IsNullOrEmpty(json) || !json.StartsWith("{") || json == _EmptyJSON)
            {
                _OnGameStart?.Invoke(GameType.NONE);
                return;
            }

            var expectedClientInfo = JsonUtility.FromJson<FluencyClientInfo>(json);
            if (!expectedClientInfo.Compatible(_FluencyClientInfo.GameType))
            {
                var error = "[LoLFluencySDK] Client and data are not compatible";
                error += "\nClient version: " + FluencyClientInfo.Version;
                error += "\nData version: " + expectedClientInfo.version;
                error += "\nRequested game type: " + expectedClientInfo.GameType.ToString();
                error += "\nClient supported game types:";
                if (_FluencyClientInfo._gameType.value.HasFlag(GameType.TYPING))
                    error += " " + GameType.TYPING.ToString();
                if (_FluencyClientInfo._gameType.value.HasFlag(GameType.ASSESS))
                    error += " " + GameType.ASSESS.ToString();
                if (_FluencyClientInfo._gameType.value.HasFlag(GameType.ESTABLISH))
                    error += " " + GameType.ESTABLISH.ToString();
                if (_FluencyClientInfo._gameType.value.HasFlag(GameType.PRACTICE))
                    error += " " + GameType.PRACTICE.ToString();
                Debug.LogError(error);
                return;
            }

            // Assign the gametype sent from the player to the client info for ref.
            _FluencyClientInfo.playerGameType = expectedClientInfo.GameType;
            switch (expectedClientInfo.GameType)
            {
                case GameType.TYPING:
                    _SessionResults = new TypingResult();
                    var typingData = JsonUtility.FromJson<TypingData>(json);
                    _SessionStartData = typingData;
                    break;
                case GameType.ASSESS:
                    _SessionResults = new AssessResult();
                    var assessData = JsonUtility.FromJson<AssessData>(json);
                    _SessionStartData = assessData;
                    break;
                case GameType.ESTABLISH:
                    _SessionResults = new EstablishResult();
                    var EstablishData = JsonUtility.FromJson<EstablishData>(json);
                    _SessionStartData = EstablishData;
                    break;
                case GameType.PRACTICE:
                    _SessionResults = new PracticeResult();
                    var practiceData = JsonUtility.FromJson<PracticeData>(json);
                    _SessionStartData = practiceData;

                    // Invoke SendResults on interval.
                    _Instance.StartCoroutine(_Instance._SendResultOnInterval());
                    break;
            }

            _OnGameStart(expectedClientInfo.GameType);
        }

        static void ReceiveLoadStateData (string json)
        {
            if (_Instance._timerRoutine != null)
            {
                _Instance.StopCoroutine(_Instance._timerRoutine);
            }

            _OnLoadState?.Invoke(json);
        }

        static void ReceiveLanguageJson (string json)
        {
            var language = JsonUtility.FromJson<Language>(json);
            _GameLanguage = new Dictionary<string, string>();
            foreach (var entry in language.entries)
            {
                _GameLanguage[entry.key] = entry.value;
            }
        }

        static void ReceiveSaveStateResult (string json)
        {
            var result = JsonUtility.FromJson<SaveStateResults>(json);
            _OnSaveStateResults?.Invoke(result.success);
        }
    }

    [Flags]
    public enum GameType
    {
        NONE = 0,
        ASSESS = 1 << 0,
        ESTABLISH = 1 << 1,
        PRACTICE = 1 << 2,
        TYPING = 1 << 3,
    }

    // internal models

    internal struct UnityStringEnum<TEnum> where TEnum : struct
    {
        public bool isSet;
        public TEnum value;
        public TEnum Value (string strValue, bool replace = false)
        {
            if (isSet && !replace)
                return value;

            if (string.IsNullOrEmpty(strValue))
            {
                Debug.LogError($"{typeof(TEnum)} string value not set.");
                return default;
            }

            if (Enum.TryParse<TEnum>(strValue, true, out var result))
            {
                value = result;
                isSet = true;
            }
            else
            {
                Debug.LogError($"{strValue} value not found in {typeof(TEnum)} enum.");
            }

            return result;
        }
    }

    [Serializable]
    internal class State<T>
    {
        public long timestamp;
        public T data;
    }

    [Serializable]
    internal class SDKOptions
    {
        public string[] supportedReceiverKeys = default;
        public GameOptions gameOptions = default;
    }

    [Serializable]
    internal class SaveStateResults
    {
        public bool success = default;
    }

    [Serializable]
    internal class Language
    {
        public KeyValueData[] entries = default;
    }

    [Serializable]
    internal class KeyValueData
    {
        public string key = default;
        public string value = default;
    }

    [Serializable]
    internal class FluencyClientInfo
    {
        public const string Version = "1.0.0";

        [NonSerialized]
        public GameType playerGameType = default;
        public string gameType = default;
        public string version = default;
        public long userId = default;

        public UnityStringEnum<GameType> _gameType;
        public GameType GameType
        {
            get
            {
                return _gameType.Value(gameType);
            }
        }

        public bool Compatible (GameType clientGameType)
        {
            var expectedVersion = new Version(version);
            var clientVersion = new Version(Version);

            return clientVersion.CompareTo(expectedVersion) > -1 && (GameType & clientGameType) != 0;
        }
    }

    // start data.

    [Serializable]
    public class GameOptions
    {

    }

    public enum FluencyFactOperation
    {
        ADD,
        DIV,
        MUL,
        SUB,
        MATCH, // Typing test
    }

    [Serializable]
    public class FluencyFact
    {
        public int a;
        public int b;
        public string op;

        UnityStringEnum<FluencyFactOperation> _operation;
        public FluencyFactOperation Operation
        {
            get
            {
                return _operation.Value(op);
            }
        }
    }

    [Serializable]
    public class FactFamilies
    {
        public FluencyFact[] factFamily;
    }

    public enum FluencySessionPracticeConcept
    {
        NONE,
        ADDITION,
        ADD_TEN,
        ADD_ZERO,
        DOUBLE,
        MULTIPLICATION,
        SQUARE,
        TIMES_TWO,
        TIMES_TEN,
        TIMES_ZERO,
        TIMES_ELEVEN,
    }

    public interface ISessionStartData { }

    [Serializable]
    public class TypingData : ISessionStartData
    {
        public int latencyThresholdMs = 5_000;
        public int minCorrect = 10;
        public int startRange;
        public int endRange = 19;
    }

    [Serializable]
    public class AssessData : ISessionStartData
    {
        public FluencyFact[] facts;
    }

    [Serializable]
    public class EstablishData : ISessionStartData
    {
        public string concept;
        public FactFamilies[] factFamilies;
        public FluencyFact[] targetFacts;
        public FluencyFact[] facts;

        UnityStringEnum<FluencySessionPracticeConcept> _concept;
        public FluencySessionPracticeConcept Concept
        {
            get
            {
                return _concept.Value(concept);
            }
        }
    }

    [Serializable]
    public class PracticeData : ISessionStartData
    {
        public FluencyFact[] targetFacts;
        public FluencyFact[] facts;
    }

    // results data

    internal interface IResultable
    {
        void AddResult (int a, int b, FluencyFactOperation operation, int? answer, DateTime startTime, int latencyMS);
        string SerializeAndClear ();
    }

    [Serializable]
    internal class ResultBase : IResultable
    {
        public List<FluencyTrialInput> trials;

        public void AddResult (int a, int b, FluencyFactOperation operation, int? answer, DateTime startTime, int latencyMs)
        {
            if (trials == null)
                trials = new List<FluencyTrialInput>();

            var trial = new FluencyTrialInput
            {
                a = a,
                b = b,
                op = operation.ToString(),
                answer = answer ?? int.MinValue,
                startTime = startTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                latencyMs = latencyMs
            };

            trials.Add(trial);
        }

        public string SerializeAndClear ()
        {
            if (trials == null || trials.Count == 0)
                return null;

            var json = JsonUtility.ToJson(this)
                .Replace(int.MinValue.ToString(), "null");

            trials.Clear();
            return json;
        }
    }

    [Serializable]
    internal class TypingResult : IResultable
    {
        public int inputLatencyMs;
        List<int> _trials;

        public void AddResult (int a, int b, FluencyFactOperation operation, int? answer, DateTime startTime, int latencyMS)
        {
            if (_trials == null)
                _trials = new List<int>();

            _trials.Add(latencyMS);
        }

        public string SerializeAndClear ()
        {
            if (_trials == null || _trials.Count == 0)
                return null;

            var total = 0;
            foreach (var trial in _trials)
            {
                total += trial;
            }

            inputLatencyMs = total / _trials.Count;

            var json = JsonUtility.ToJson(this);
            _trials.Clear();
            return json;
        }
    }

    [Serializable]
    internal class AssessResult : ResultBase
    {

    }

    [Serializable]
    internal class EstablishResult : ResultBase
    {

    }

    [Serializable]
    internal class PracticeResult : ResultBase
    {

    }

    [Serializable]
    internal class FluencyTrialInput : FluencyFact
    {
        public string startTime;
        public int latencyMs;
        public int answer;
    }
}