using UnityEngine;

namespace LoL.Fluency
{
    public class EmbeddedFluencyPlayer : MonoBehaviour
    {
        internal static EmbeddedFluencyPlayer _Instance { get; private set; }
        LoLFluencySDK _SDK;
        IFluencyRequest _fluencyRequest;
        UserSettings _userSettings;
        internal float _progress;

        internal static EmbeddedFluencyPlayer Init (IFluencyRequest fluencyRequest, UserSettings userSettings = null)
        {
            if (_Instance == null)
            {
                _Instance = new GameObject($"__{nameof(EmbeddedFluencyPlayer)}__").AddComponent<EmbeddedFluencyPlayer>();
                _Instance._Init(fluencyRequest, userSettings);
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

        internal void _Init (IFluencyRequest fluencyRequest, UserSettings userSettings)
        {
            _fluencyRequest = fluencyRequest;
            _userSettings = userSettings;

            // _SDK = LoLFluencySDK.InitEmbeddedPlayer(GameIsReady, PostWindowMessage);
            // Will use embedded test data.
            _SDK = LoLFluencySDK.InitEmbeddedPlayer(null, null);
            SendLanguage();
        }

        void GameIsReady (string gameName, string gameObjectName, string functionName, string sdkVersion, string sdkParams)
        {
            Debug.Log($"{gameName} {gameObjectName} {functionName} {sdkVersion} {sdkParams}");

            SendUserSettings(_userSettings);

            SendLanguage();

            // Make REST call for user data.
            _fluencyRequest.GetFluencySessionActivity(OnSessionActivity);
        }

        void SendUserSettings (UserSettings userSettings)
        {
            if (userSettings == null)
                return;

            var json = JsonUtility.ToJson(new KeyValueData { key = "userSettings", value = JsonUtility.ToJson(userSettings) });
            _SDK.ReceiveData(json);
        }

        void SendLanguage ()
        {
#if UNITY_EDITOR
            var file = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.streamingAssetsPath, "language.json"));
            var langObj = Newtonsoft.Json.Linq.JObject.Parse(file).GetValue("en");
            var lang = langObj.ToObject<SerializableDictionary<string, string>>();
            var json = JsonUtility.ToJson(new KeyValueData { key = "language", value = JsonUtility.ToJson(lang) });
            _SDK.ReceiveData(json);
#endif
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
                case "updateUserSettings":
                    ProcessUserSettings(payload);
                    break;
            }
        }

        void ProcessResults (string payload)
        {
            var results = JsonUtility.FromJson<Results>(payload);
            _fluencyRequest.PutFluencyTrials(results, OnResultsProcess);
        }

        void ProcessUserSettings (string payload)
        {
            var practiceGameUserSettings = JsonUtility.FromJson<UserSettings>(payload);
            // Override current settings with new userSettings.
            Debug.Log("User settings sent from practice game: " + payload);
        }

        void OnResultsProcess (AckData data)
        {
            Debug.Log($"Results saved: {data.success}");
            // Make REST call for user data.
            _fluencyRequest.GetFluencySessionActivity(OnSessionActivity);
        }
    }
}