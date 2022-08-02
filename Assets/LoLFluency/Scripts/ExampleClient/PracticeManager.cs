using LoL.Fluency;
using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class PracticeManager : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Text text;
        [SerializeField] Button send;
        [SerializeField] Button save;
        [SerializeField] Button load;
        [SerializeField] Button userSettings;
        GameState _gameState;
        PracticeData _practiceStartData;

        void Start ()
        {
            title.text = LoLFluencySDK.GetLanguageText("inPractice", "IN PRACTICE");
            // If you don't use a static variable, you can get the starting data from the sdk.
            // NOTE: if you try to get starting data for a game type that isn't valid for that session, you'll get null.
            // i.e. Fluency Player starts a game type: INSTRUCT session and unity client is requesting LoLFluencySDK.GetStartData<AssessData>()
            // So make sure the game sessions are separated in your unity client if your build can handle multiple game types.
            _practiceStartData = LoLFluencySDK.GetStartData<PracticeData>();

            var value = "FACTS";
            foreach (var fact in _practiceStartData.facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            value += "\n\nTARGET FACTS";
            if (!(_practiceStartData.targetFacts is null))
            {
                foreach (var fact in _practiceStartData.targetFacts)
                {
                    value += $"\n{fact.a} {fact.op} {fact.b}";
                }
            }
            text.text = value;

            send.onClick.AddListener(SendResults);
            save.onClick.AddListener(Save);
            load.onClick.AddListener(Load);
            userSettings.onClick.AddListener(UpdateUserSettings);
        }

        void SendResults ()
        {
            foreach (var fact in _practiceStartData.facts)
            {
                LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }

            if (_practiceStartData.targetFacts is null)
                return;
            foreach (var fact in _practiceStartData.targetFacts)
            {
                LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }
            // Send is sent on an interval for PRACTICE.
            //LoL.Fluency.LoLFluencySDK.SendResults();
        }

        void Save ()
        {
            _gameState = new GameState
            {
                hitCount = Random.Range(2, 20),
                retry = Random.Range(0, 10),
                score = Random.Range(0, 2000)
            };
            LoLFluencySDK.SaveGameState(_gameState, OnSaveResult);
        }

        void Load ()
        {
            LoLFluencySDK.LoadGameState<GameState>(Load);
        }

        void OnSaveResult (bool success)
        {
            Debug.Log("Save result: " + success);
        }

        void Load (GameState state)
        {
            if (state == null)
            {
                Debug.Log("No data for load state.");
                return;
            }
            _gameState = state;
            Debug.Log("Received Practice load state: " + JsonUtility.ToJson(state));
        }

        void UpdateUserSettings ()
        {
            LoLFluencySDK.UpdateUserSettings(new UserSettings
            {
                musicOn = true,
                sfxOn = false,
                additionalSettings = new SerializableDictionary<string, string>
                { ["inputVolume"] = "3.4" }
            });
        }
    }
}