using UnityEngine;
using UnityEngine.UI;

namespace SomeDev.Test
{
    public class AssessmentManager : MonoBehaviour
    {
        [SerializeField] Text text;
        [SerializeField] Button send;

        void Start ()
        {
            var value = "FACTS";
            foreach (var fact in FluencyGameInit.AssessmentStartData.facts)
            {
                value += $"\n{fact.a} {fact.op} {fact.b}";
            }
            text.text = value;

            send.onClick.AddListener(SendResults);
        }

        void SendResults ()
        {
            foreach (var fact in FluencyGameInit.AssessmentStartData.facts)
            {
                FluencyGameInit.AssessmentResults.AddResult(fact.a, fact.b, fact.Operation, Random.Range(fact.a, fact.b), System.DateTime.UtcNow, Random.Range(100, 300));
            }
            LoL.Fluency.LoLFluencySDK.SendResults(FluencyGameInit.AssessmentResults);
        }
    }
}