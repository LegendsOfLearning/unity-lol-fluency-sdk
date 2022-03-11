using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class PlayManager : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Text text;
        [SerializeField] Button send;
        [SerializeField] Button save;
        [SerializeField] Button load;
        GameState _gameState;

        void Start ()
        {
            title.text = LoL.Fluency.LoLFluencySDK.GetLanguageText("inPlay", "IN PLAY");
            var value = "FACTS";
            foreach (var fact in FluencyGameInit.PlayStartData.facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            value += "\n\nTARGET FACTS";
            foreach (var fact in FluencyGameInit.PlayStartData.targetFacts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            text.text = value;

            send.onClick.AddListener(SendResults);
            save.onClick.AddListener(Save);
            load.onClick.AddListener(Load);
        }

        void SendResults ()
        {
            foreach (var fact in FluencyGameInit.PlayStartData.facts)
            {
                LoL.Fluency.LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }
            foreach (var fact in FluencyGameInit.PlayStartData.targetFacts)
            {
                LoL.Fluency.LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }
            LoL.Fluency.LoLFluencySDK.SendResults();
        }

        void Save ()
        {
            _gameState = new GameState
            {
                hitCount = Random.Range(2, 20),
                retry = Random.Range(0, 10),
                score = Random.Range(0, 2000)
            };
            LoL.Fluency.LoLFluencySDK.SaveGameState(_gameState, OnSaveResult);
        }

        void Load ()
        {
            LoL.Fluency.LoLFluencySDK.LoadGameState<GameState>(Load);
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