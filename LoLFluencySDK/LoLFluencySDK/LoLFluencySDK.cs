using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LoL.Fluency
{
    public class LoLFluencySDK : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void _GameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion);
        [DllImport("__Internal")]
        private static extern void _PostWindowMessage (string msg, string payload);

        static void _EditorGameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion)
        {
            Debug.Log(gameObjectName + " : " + functionName);
            string json;
            switch (UnityEngine.Random.Range(0, 5))
            {
                case 1:
                    json = @"{""gameType"":""PRACTICE"",""version"":""1.0.0"",""concept"":""MULTIPLICATION"",""facts"":[{""a"":1,""b"":2,""op"":""ADD""}],""target_facts"":[{""a"":2,""b"":4,""op"":""MUL""}]}";
                    break;
                case 2:
                    json = @"{""gameType"":""PLAY"",""version"":""1.0.0"",""facts"":[{""a"":1,""b"":2,""op"":""DIV""}],""target_facts"":[{""a"":2,""b"":4,""op"":""MUL""}]}";
                    break;
                case 3:
                    json = @"{""gameType"":""TESTING FAIL"",""version"":""1.0.0"",""facts"":[{""a"":1,""b"":2,""op"":""SUB""}]}";
                    break;
                case 4:
                    json = @"{""gameType"":""PLAY"",""version"":""2.0.0"",""facts"":[{""a"":1,""b"":2,""op"":""SUB""}]}";
                    break;
                default:
                    json = @"{""gameType"":""ASSESSMENT"",""version"":""1.0.0"",""facts"":[{""a"":1,""b"":2,""op"":""SUB""}]}";
                    break;
            }
            _Instance.ReceiveData(json);
        }

        static void _EditorPostWindowMessage (string msg, string payload)
        {
            Debug.Log("Post message: " + msg + " : " + payload);
        }

        static LoLFluencySDK _Instance;
        static LoLFluencySDK Create ()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("__" + nameof(LoLFluencySDK) + "__").AddComponent<LoLFluencySDK>();
                _Instance._fluencyClientInfo = new FluencyClientInfo();
                DontDestroyOnLoad(_Instance);
            }

            return _Instance;
        }

        FluencyClientInfo _fluencyClientInfo;

        Action<AssessmentData> _onAssessmentData;
        Action<PracticeData> _onPracticeData;
        Action<PlayData> _onPlayData;

        public static IResultable InitAssessment (Action<AssessmentData> onAssessmentData)
        {
            if (onAssessmentData == null)
            {
                Debug.LogError("[LoLFluencySDK] " + nameof(onAssessmentData) + " callback must be set.");
                return null;
            }

            var sdk = Create();
            sdk._onAssessmentData = onAssessmentData;
            sdk._fluencyClientInfo._gameType.value |= GameType.ASSESSMENT;
            sdk._fluencyClientInfo._gameType.isSet = true;
            return new AssessmentResult();
        }

        public static IResultable InitPractice (Action<PracticeData> onPracticeData)
        {
            if (onPracticeData == null)
            {
                Debug.LogError("[LoLFluencySDK] " + nameof(onPracticeData) + " callback must be set.");
                return null;
            }

            var sdk = Create();
            sdk._onPracticeData = onPracticeData;
            sdk._fluencyClientInfo._gameType.value |= GameType.PRACTICE;
            sdk._fluencyClientInfo._gameType.isSet = true;
            return new PracticeResult();
        }

        public static IResultable InitPlay (Action<PlayData> onPlayData)
        {
            if (onPlayData == null)
            {
                Debug.LogError("[LoLFluencySDK] " + nameof(onPlayData) + " callback must be set.");
                return null;
            }

            var sdk = Create();
            sdk._onPlayData = onPlayData;
            sdk._fluencyClientInfo._gameType.value |= GameType.PLAY;
            sdk._fluencyClientInfo._gameType.isSet = true;
            return new PlayResult();
        }

        public static void GameIsReady ()
        {
            if (_Instance == null)
            {
                Debug.LogError("[LoLFluencySDK] Call Init before calling GameIsReady.");
                return;
            }

            // Call into jslib.
            Action<string, string, string, string> gameIsReady;
            if(Application.isEditor)
                gameIsReady = _EditorGameIsReady;
            else
                gameIsReady = _GameIsReady;

            gameIsReady(Application.productName, _Instance.gameObject.name, nameof(ReceiveData), FluencyClientInfo.Version);
        }

        public static void SendResults (IResultable resultable)
        {
            // Call into jslib.
            var json = JsonUtility.ToJson(resultable);

            Action<string, string> postWindowMessage;
            if(Application.isEditor)
                postWindowMessage = _EditorPostWindowMessage;
            else
                postWindowMessage = _PostWindowMessage;

            postWindowMessage("results", json);
        }

        public void ReceiveData (string json)
        {
            var expectedClientInfo = JsonUtility.FromJson<FluencyClientInfo>(json);
            if (!expectedClientInfo.Compatible(_fluencyClientInfo.GameType))
            {
                var error = "[LoLFluencySDK] Client and data are not compatible";
                error += "\nClient version: " + FluencyClientInfo.Version;
                error += "\nData version: " + expectedClientInfo.version;
                error += "\nRequested game type: " + expectedClientInfo.GameType.ToString();
                error += "\nClient supported game types:";
                if (_fluencyClientInfo._gameType.value.HasFlag(GameType.ASSESSMENT))
                    error += " " + GameType.ASSESSMENT.ToString();
                if (_fluencyClientInfo._gameType.value.HasFlag(GameType.PRACTICE))
                    error += " " + GameType.PRACTICE.ToString();
                if (_fluencyClientInfo._gameType.value.HasFlag(GameType.PLAY))
                    error += " " + GameType.PLAY.ToString();
                Debug.LogError(error);
                return;
            }

            switch (expectedClientInfo.GameType)
            {
                case GameType.ASSESSMENT:
                    var assessmentData = JsonUtility.FromJson<AssessmentData>(json);
                    _onAssessmentData(assessmentData);
                    break;
                case GameType.PRACTICE:
                    var practiceData = JsonUtility.FromJson<PracticeData>(json);
                    _onPracticeData(practiceData);
                    break;
                case GameType.PLAY:
                    var playData = JsonUtility.FromJson<PlayData>(json);
                    _onPlayData(playData);
                    break;
            }
        }
    }

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
        public TEnum Value(string strValue, bool replace = false)
        {
            if(isSet && !replace)
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
    internal class FluencyClientInfo
    {
        public const string Version = "1.0.0";

        public string gameType = default;
        public string version = default;

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
        public FluencyFact[] target_facts;
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
        public FluencyFact[] target_facts;
        public FluencyFact[] facts;
    }

    // results data

    public interface IResultable
    {
        void AddResult (int a, int b, FluencyFactOperation operation, int answer, DateTime startTime, int latencyMS);
    }

    [Serializable]
    internal class ResultBase : IResultable
    {
        public List<FluencyTrialInput> trials;

        public void AddResult (int a, int b, FluencyFactOperation operation, int answer, DateTime startTime, int latencyMS)
        {
            if (trials == null)
                trials = new List<FluencyTrialInput>();

            trials.Add(new FluencyTrialInput
            {
                a = a,
                b = b,
                op = operation.ToString(),
                answer = answer,
                start_time = startTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                latency_ms = latencyMS
            });
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
        public string start_time;
        public int latency_ms;
        public int answer;
    }
}