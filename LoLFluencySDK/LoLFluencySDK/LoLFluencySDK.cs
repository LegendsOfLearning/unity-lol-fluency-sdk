using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LoL.Fluency
{
    public class LoLFluencySDK : MonoBehaviour
    {
        [DllImport("__Internal")]
        static extern void _GameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion, string sdkParams);
        [DllImport("__Internal")]
        static extern void _PostWindowMessage (string msg, string payload);

        static void _EditorGameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion, string sdkParams)
        {
            string json;
            if (_FluencyClientInfo.GameType.HasFlag(GameType.PLAY))
            {
                json = @"{""gameType"":""PLAY"",""version"":""1.0.0"",""facts"":[{""a"":4,""b"":2,""op"":""DIV""}],""targetFacts"":[{""a"":2,""b"":4,""op"":""MUL""}]}";
            }
            else if (_FluencyClientInfo.GameType.HasFlag(GameType.PRACTICE))
            {
                json = @"{""gameType"":""PRACTICE"",""version"":""1.0.0"",""concept"":""MULTIPLICATION"",""facts"":[{""a"":1,""b"":2,""op"":""MUL""}],""targetFacts"":[{""a"":2,""b"":4,""op"":""MUL""}]}";
            }
            else if (_FluencyClientInfo.GameType.HasFlag(GameType.ASSESSMENT))
            {
                json = @"{""gameType"":""ASSESSMENT"",""version"":""1.0.0"",""facts"":[{""a"":3,""b"":2,""op"":""SUB""}]}";
            }
            else
            {
                json = @"{""gameType"":""TESTING FAIL"",""version"":""1.0.0"",""facts"":[{""a"":3,""b"":2,""op"":""SUB""}]}";
            }

            json = JsonUtility.ToJson(new KeyValueData { key = "start", value = json });
            _Instance.ReceiveData(json);
        }

        static void _EditorPostWindowMessage (string msg, string payload)
        {
            Debug.Log("Post window message: " + msg + " : " + payload);
            string json = "{}";
            switch (msg.ToLower())
            {
                case "loadstate":
                    json = PlayerPrefs.GetString(_PlayerPrefsKey, json);
                    break;
                case "language":
                    var entries = new KeyValueData[]
                        {
                            new KeyValueData { key = "title", value = "LoL Fluency SDK" },
                            new KeyValueData { key = "start", value = "Start Game" },
                            new KeyValueData { key = "highscore", value = "High Score" },
                        };

                    json = JsonUtility.ToJson(new Language { entries = entries });
                    break;
                case "savestate":
                    msg = "saveStateResult";
                    json = JsonUtility.ToJson(new SaveStateResults { success = true });
                    break;
                default: // Start doesn't have a return receiver.
                    return;
            }

            json = JsonUtility.ToJson(new KeyValueData { key = msg, value = json });
            _Instance.ReceiveData(json);
        }

        static LoLFluencySDK _Instance;
        static LoLFluencySDK CreateSDK ()
        {
            if (_Instance is null)
            {
                _Instance = new GameObject("__" + nameof(LoLFluencySDK) + "__").AddComponent<LoLFluencySDK>();
                _FluencyClientInfo = new FluencyClientInfo();
                DontDestroyOnLoad(_Instance);
            }

            return _Instance;
        }

        static FluencyClientInfo _FluencyClientInfo;

        static Action<AssessmentData> _OnAssessmentData;
        static Action<PracticeData> _OnPracticeData;
        static Action<PlayData> _OnPlayData;
        static Action<string> _OnLoadState;
        static Action<bool> _OnSaveStateResults;

        static Dictionary<string, string> _GameLanguage;
        static IResultable _SessionResults;

        public static void InitAssessment (Action<AssessmentData> onAssessmentData)
        {
            if (onAssessmentData is null)
            {
                Debug.LogError("[LoLFluencySDK] " + nameof(onAssessmentData) + " callback must be set.");
                return;
            }

            CreateSDK();
            _OnAssessmentData = onAssessmentData;
            _FluencyClientInfo._gameType.value |= GameType.ASSESSMENT;
            _FluencyClientInfo._gameType.isSet = true;
        }

        public static void InitPractice (Action<PracticeData> onPracticeData)
        {
            if (onPracticeData is null)
            {
                Debug.LogError("[LoLFluencySDK] " + nameof(onPracticeData) + " callback must be set.");
                return;
            }

            CreateSDK();
            _OnPracticeData = onPracticeData;
            _FluencyClientInfo._gameType.value |= GameType.PRACTICE;
            _FluencyClientInfo._gameType.isSet = true;
        }

        public static void InitPlay (Action<PlayData> onPlayData)
        {
            if (onPlayData is null)
            {
                Debug.LogError("[LoLFluencySDK] " + nameof(onPlayData) + " callback must be set.");
                return;
            }

            CreateSDK();
            _OnPlayData = onPlayData;
            _FluencyClientInfo._gameType.value |= GameType.PLAY;
            _FluencyClientInfo._gameType.isSet = true;
        }

        public static void GameIsReady (GameOptions options = null)
        {
            if (_Instance is null)
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

            Action<string, string, string, string, string> gameIsReady;
            if (Application.isEditor)
                gameIsReady = _EditorGameIsReady;
            else
                gameIsReady = _GameIsReady;

            gameIsReady(Application.productName, _Instance.gameObject.name, nameof(ReceiveData), FluencyClientInfo.Version, sdkOptionsJson);
        }

        static void PostWindowMessage (string msg, string json)
        {
            Action<string, string> postWindowMessage;
            if (Application.isEditor)
                postWindowMessage = _EditorPostWindowMessage;
            else
                postWindowMessage = _PostWindowMessage;

            postWindowMessage(msg, json);
        }

        public static void SendResults ()
        {
            if (_SessionResults is null)
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

        public static bool AddResult (int a, int b, FluencyFactOperation operation, int answer, DateTime startTime, int latencyMS)
        {
            if (_SessionResults is null)
            {
                Debug.LogError("[LoLFluencySDK] results not set. Did you call GameIsReady first?");
                return false;
            }

            _SessionResults.AddResult(a, b, operation, answer, startTime, latencyMS);
            return true;
        }

        public static string GetLanguageText (string key, string defaultValue = null)
        {
            if (_Instance is null || _GameLanguage is null)
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
            PostWindowMessage("speakTextCancel", "{}");
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

            PostWindowMessage("loadState", "{}");
        }

        static void SetOnStateLoadedCallback<T> (Action<T> onLoaded) where T : class
        {
            _OnLoadState = json =>
            {
                if (string.IsNullOrEmpty(json) || !json.StartsWith("{"))
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
                    if (!(localState is null))
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
            if (_FluencyClientInfo.playerGameType != GameType.PLAY)
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
            ["loadstate"] = ReceiveLoadStateData,
            ["language"] = ReceiveLanguageJson,
            ["savestateresult"] = ReceiveSaveStateResult,
        };

        // called from jslib.
        public void ReceiveData (string json)
        {
            var msgData = JsonUtility.FromJson<KeyValueData>(json);
            if (msgData == null)
            {
                Debug.LogError("[LoLFluencySDK] Received incorrect json from fluency player: " + json);
                return;
            }

            var key = msgData.key.ToLower();
            if (!_Receivers.TryGetValue(key, out var receiver))
            {
                Debug.LogWarning("[LoLFluencySDK] Receiver not implemented in sdk: " + key);
                return;
            }

            receiver?.Invoke(msgData.value);
        }

        // message receivers

        static void ReceiveStartData (string json)
        {
            if (string.IsNullOrEmpty(json) || !json.StartsWith("{"))
            {
                _OnAssessmentData?.Invoke(null);
                _OnPracticeData?.Invoke(null);
                _OnPlayData?.Invoke(null);
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
                if (_FluencyClientInfo._gameType.value.HasFlag(GameType.ASSESSMENT))
                    error += " " + GameType.ASSESSMENT.ToString();
                if (_FluencyClientInfo._gameType.value.HasFlag(GameType.PRACTICE))
                    error += " " + GameType.PRACTICE.ToString();
                if (_FluencyClientInfo._gameType.value.HasFlag(GameType.PLAY))
                    error += " " + GameType.PLAY.ToString();
                Debug.LogError(error);
                return;
            }

            // Assign the gametype sent from the player to the client info for ref.
            _FluencyClientInfo.playerGameType = expectedClientInfo.GameType;
            switch (expectedClientInfo.GameType)
            {
                case GameType.ASSESSMENT:
                    _SessionResults = new AssessmentResult();
                    var assessmentData = JsonUtility.FromJson<AssessmentData>(json);
                    _OnAssessmentData(assessmentData);
                    break;
                case GameType.PRACTICE:
                    _SessionResults = new PracticeResult();
                    var practiceData = JsonUtility.FromJson<PracticeData>(json);
                    _OnPracticeData(practiceData);
                    break;
                case GameType.PLAY:
                    _SessionResults = new PlayResult();
                    var playData = JsonUtility.FromJson<PlayData>(json);
                    _OnPlayData(playData);

                    // Invoke SendResults on interval.
                    _Instance.StartCoroutine(_Instance._SendResultOnInterval());
                    break;
            }
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

    // internal models

    [Flags]
    internal enum GameType
    {
        NONE = 0,
        ASSESSMENT = 1 << 0,
        PRACTICE = 1 << 1,
        PLAY = 1 << 2,
    }

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
        ADD, DIV, MUL, SUB
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

    [Serializable]
    public class AssessmentData
    {
        public FluencyFact[] facts;
    }

    [Serializable]
    public class PracticeData
    {
        public string concept;
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
    public class PlayData
    {
        public FluencyFact[] targetFacts;
        public FluencyFact[] facts;
    }

    // results data

    internal interface IResultable
    {
        void AddResult (int a, int b, FluencyFactOperation operation, int answer, DateTime startTime, int latencyMS);
        string SerializeAndClear ();
    }

    [Serializable]
    internal class ResultBase : IResultable
    {
        public List<FluencyTrialInput> trials;

        public void AddResult (int a, int b, FluencyFactOperation operation, int answer, DateTime startTime, int latencyMs)
        {
            if (trials == null)
                trials = new List<FluencyTrialInput>();

            trials.Add(new FluencyTrialInput
            {
                a = a,
                b = b,
                op = operation.ToString(),
                answer = answer,
                startTime = startTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                latencyMs = latencyMs
            });
        }

        public string SerializeAndClear ()
        {
            if (trials == null || trials.Count == 0)
                return null;

            var json = JsonUtility.ToJson(this);
            trials.Clear();
            return json;
        }
    }

    [Serializable]
    internal class AssessmentResult : ResultBase
    {

    }

    [Serializable]
    internal class PracticeResult : ResultBase
    {

    }

    [Serializable]
    internal class PlayResult : ResultBase
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