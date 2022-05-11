using UnityEngine;

namespace LoL.Fluency
{
    public class EmbeddedFluencyPlayer : MonoBehaviour
    {
        internal static EmbeddedFluencyPlayer _Instance { get; private set; }
        LoLFluencySDK _SDK;
        IFluencyRequest _fluencyRequest;
        internal float _progress;

        internal static EmbeddedFluencyPlayer Init (IFluencyRequest fluencyRequest)
        {
            if (_Instance == null)
            {
                _Instance = new GameObject($"__{nameof(EmbeddedFluencyPlayer)}__").AddComponent<EmbeddedFluencyPlayer>();
                _Instance._Init(fluencyRequest);
                DontDestroyOnLoad(_Instance.gameObject);
            }

            return _Instance;
        }

        internal static void Destroy ()
        {
            if (_Instance == null)
                return;

            Destroy(_Instance._SDK.gameObject);
            Destroy(_Instance.gameObject);
        }

        internal static float GetProgress
        {
            get
            {
                if (_Instance == null)
                    return 0;

                return _Instance._progress; 
            }
        }

        void OnDestroy ()
        {
            _Instance = null;
            _SDK = null;
            _fluencyRequest = null;
        }

        internal void _Init (IFluencyRequest fluencyRequest)
        {
            _fluencyRequest = fluencyRequest;
            _SDK = LoLFluencySDK.InitEmbeddedPlayer(GameIsReady, PostWindowMessage);
        }

        void GameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion, string sdkParams)
        {
            Debug.Log($"{gameName} {gameObjectName} {functionName} {sdkVersion} {sdkParams}");

            // Make REST call for user data.
            _fluencyRequest.GetFluencySessionActivity(OnSessionActivity);
        }

        void OnSessionActivity (string data, float blueLightPercentage)
        {
            LoadGameType(data);
            _progress = blueLightPercentage;
        }

        void LoadGameType (string data)
        {
            var json = JsonUtility.ToJson(new KeyValueData { key = "start", value = data });
            _SDK.ReceiveData(json);
        }

        void PostWindowMessage (string msg, string payload)
        {
            Debug.Log($"{msg} {payload}");

            switch (msg)
            {
                case "results":
                    ProcessResults(payload);
                    break;
            }
        }

        void ProcessResults (string payload)
        {
            var results = JsonUtility.FromJson<Results>(payload);
            _fluencyRequest.PutFluencyTrials(results, OnResultsProcess);
        }

        void OnResultsProcess (AckData data)
        {
            Debug.Log($"Results saved: {data.success}");
            // Make REST call for user data.
            _fluencyRequest.GetFluencySessionActivity(OnSessionActivity);
        }
    }
}