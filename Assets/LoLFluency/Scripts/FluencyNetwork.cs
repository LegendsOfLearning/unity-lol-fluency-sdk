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
        public static int InputLatencyMs { get; private set; }

        void IFluencyRequest.GetFluencySessionActivity (Action<string, float> callback)
        {
        }

        void IFluencyRequest.PutFluencyTrials (Results results, Action<AckData> callback)
        {
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