using LoL.Fluency;
using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class RetrieveManager : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Text text;
        [SerializeField] Button send;
        [SerializeField] Button save;
        [SerializeField] Button load;
        GameState _gameState;

        void Start ()
        {
            title.text = LoLFluencySDK.GetLanguageText("inRetrieve", "IN RETRIEVE");
            // If you don't use a static variable, you can get the starting data from the sdk.
            // NOTE: if you try to get starting data for a game type that isn't valid for that session, you'll get null.
            // i.e. Fluency Player starts a game type: INSTRUCT session and unity client is requesting LoLFluencySDK.GetStartData<AssessData>()
            // So make sure the game sessions are separated in your unity client if your build can handle multiple game types.
            var retrieveStartData = LoLFluencySDK.GetStartData<RetrieveData>();

            var value = "FACTS";
            foreach (var fact in retrieveStartData.facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            value += "\n\nTARGET FACTS";
            if (!(retrieveStartData.targetFacts is null))
            {
                foreach (var fact in retrieveStartData.targetFacts)
                {
                    value += $"\n{fact.a} {fact.op} {fact.b}";
                }
            }
            text.text = value;

            send.onClick.AddListener(SendResults);
            save.onClick.AddListener(Save);
            load.onClick.AddListener(Load);
        }

        void SendResults ()
        {
            foreach (var fact in FluencyGameInit.RetrieveStartData.facts)
            {
                LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }

            if (FluencyGameInit.RetrieveStartData.targetFacts is null)
                return;
            foreach (var fact in FluencyGameInit.RetrieveStartData.targetFacts)
            {
                LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }
            // Send is sent on an interval for RETRIEVE.
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
            Debug.Log("Received load state: " + JsonUtility.ToJson(state));
        }
    }
}