using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoL.Fluency
{
    internal interface IFluencyRequest
    {
        void GetFluencySessionActivity (Action<string, float> callback);
        void PutFluencyTrials (Results results, Action<AckData> callback);
    }

    public class FluencyNetwork : IFluencyRequest
    {
        [Flags]
        internal enum GameType
        {
            NONE = 0,
            ASSESS = 1 << 0,
            ESTABLISH = 1 << 1,
            PRACTICE = 1 << 2,
            TYPING = 1 << 3,
        }

        public static int InputLatencyMs { get; private set; }
        GameType _GameType;

        void IFluencyRequest.GetFluencySessionActivity (Action<string, float> callback)
        {
            string data = string.Empty;
            switch (_GameType)
            {
                case GameType.NONE:
                    data = @"{""gameType"":""TYPING"",""version"":""1.0.0""}";
                    _GameType = GameType.TYPING;
                    break;
                case GameType.TYPING:
                    data = @"{""gameType"":""ASSESS"",""version"":""1.0.0"",""facts"":[{""a"":3,""b"":2,""op"":""SUB""}]}";
                    _GameType = GameType.ASSESS;
                    break;
                case GameType.ASSESS:
                    data = @"{""gameType"":""ESTABLISH"",""version"":""1.0.0"",""concept"":""MULTIPLICATION"",""facts"":[{""a"":1,""b"":2,""op"":""MUL""}],""targetFacts"":[{""a"":2,""b"":4,""op"":""MUL""}]}";
                    _GameType = GameType.ESTABLISH;
                    break;
                case GameType.ESTABLISH:
                    data = @"{""gameType"":""PRACTICE"",""version"":""1.0.0"",""facts"":[{""a"":4,""b"":2,""op"":""DIV""}],""targetFacts"":[{""a"":2,""b"":4,""op"":""MUL""}]}";
                    _GameType = GameType.PRACTICE;
                    break;
                case GameType.PRACTICE: // Keep playing.
                    return;
            }

            callback?.Invoke(data, 50.0f);
        }

        void IFluencyRequest.PutFluencyTrials (Results results, Action<AckData> callback)
        {
            if (_GameType == GameType.TYPING)
            {
                InputLatencyMs = results.inputLatencyMs;
            }

            // Send results to platform.
            foreach (var t in results.trials)
            {
                Debug.Log(JsonUtility.ToJson(t));
            }

            callback?.Invoke(JsonUtility.FromJson<AckData>("{\"success\":true}"));
        }
    }

    // Mirror'd models to mimic fluency player and sdk.

    [Serializable]
    internal class KeyValueData
    {
        public string key;
        public string value;
    }

    [Serializable]
    internal class AckData
    {
        public bool success;
    }

    [Serializable]
    internal class Results
    {
        public int inputLatencyMs;
        public List<TrialResult> trials;
    }

    [Serializable]
    internal class TrialResult : FluencyFact, ISerializationCallbackReceiver
    {
        public string startTime;
        public int latencyMs;
        public int answer;
        public int inputLatencyMs;

        public void OnAfterDeserialize ()
        {
            inputLatencyMs = FluencyNetwork.InputLatencyMs;
        }

        public void OnBeforeSerialize ()
        {

        }
    }
}