using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class PlayManager : MonoBehaviour
    {
        [SerializeField] Text text;
        [SerializeField] Button send;

        void Start ()
        {
            var value = "FACTS";
            Debug.Log(FluencyGameInit.PlayStartData);
            Debug.Log(FluencyGameInit.PlayStartData.facts);
            foreach (var fact in FluencyGameInit.PlayStartData.facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            value += "\n\nTARGET FACTS";
            foreach (var fact in FluencyGameInit.PlayStartData.target_facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            text.text = value;

            send.onClick.AddListener(SendResults);
        }

        void SendResults ()
        {
            foreach (var fact in FluencyGameInit.PlayStartData.target_facts)
            {
                FluencyGameInit.PlayResults.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }
            LoL.Fluency.LoLFluencySDK.SendResults(FluencyGameInit.PlayResults);
        }
    }
}