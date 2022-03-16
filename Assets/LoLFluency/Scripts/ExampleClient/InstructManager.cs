using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class InstructManager : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Text text;
        [SerializeField] Button send;
        [SerializeField] Button save;
        [SerializeField] Button load;
        GameState _gameState;

        void Start ()
        {
            title.text = LoL.Fluency.LoLFluencySDK.GetLanguageText("inInstruct", "IN INSTRUCT");
            var value = FluencyGameInit.InstructStartData.concept.ToString();
            value += "\n\nFACTS";
            foreach (var fact in FluencyGameInit.InstructStartData.facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            value += "\n\nTARGET FACTS";
            if (!(FluencyGameInit.InstructStartData.targetFacts is null))
            {
                foreach (var fact in FluencyGameInit.InstructStartData.targetFacts)
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
            foreach (var fact in FluencyGameInit.InstructStartData.facts)
            {
                LoL.Fluency.LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }

            if (!(FluencyGameInit.InstructStartData.targetFacts is null))
            {
                foreach (var fact in FluencyGameInit.InstructStartData.targetFacts)
                {
                    LoL.Fluency.LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
                }
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