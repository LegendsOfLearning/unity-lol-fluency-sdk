using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class PracticeManager : MonoBehaviour
    {
        [SerializeField] Text text;
        [SerializeField] Button send;

        void Start()
        {
            var value = FluencyGameInit.PracticeStartData.concept.ToString();
            value += "\n\nFACTS";
            foreach (var fact in FluencyGameInit.PracticeStartData.facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            value += "\n\nTARGET FACTS";
            foreach (var fact in FluencyGameInit.PracticeStartData.target_facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            text.text = value;

            send.onClick.AddListener(SendResults);
        }

        void SendResults ()
        {
            foreach (var fact in FluencyGameInit.PracticeStartData.target_facts)
            {
                FluencyGameInit.PracticeResults.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }
            LoL.Fluency.LoLFluencySDK.SendResults(FluencyGameInit.PracticeResults);
        }
    }
}