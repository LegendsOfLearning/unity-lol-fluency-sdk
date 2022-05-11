using LoL.Fluency;
using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class EstablishManager : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Text text;
        [SerializeField] Button send;
        [SerializeField] Button save;
        [SerializeField] Button load;
        GameState _gameState;

        EstablishData _establishStartData;

        void Start ()
        {
            _establishStartData = LoLFluencySDK.GetStartData<EstablishData>();
            title.text = LoLFluencySDK.GetLanguageText("inEstablish", "IN ESTABLISH");
            var value = _establishStartData.concept.ToString();
            value += "\n\nFACTS";
            foreach (var fact in _establishStartData.facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            value += "\n\nTARGET FACTS";
            if (!(_establishStartData.targetFacts is null))
            {
                foreach (var fact in _establishStartData.targetFacts)
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
            foreach (var fact in _establishStartData.facts)
            {
                LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }

            if (!(_establishStartData.targetFacts is null))
            {
                foreach (var fact in _establishStartData.targetFacts)
                {
                    LoLFluencySDK.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
                }
            }

            LoLFluencySDK.SendResults();
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
            Debug.Log("Received Establish load state: " + JsonUtility.ToJson(state));
        }
    }
}