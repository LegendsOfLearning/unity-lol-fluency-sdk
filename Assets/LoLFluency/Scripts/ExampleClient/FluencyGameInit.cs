using LoL.Fluency;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SomeDev.Test
{
    public class FluencyGameInit : MonoBehaviour
    {
        public static AssessmentData AssessmentStartData { get; private set; }
        public static PracticeData PracticeStartData { get; private set; }
        public static PlayData PlayStartData { get; private set; }

        public static IResultable AssessmentResults;
        public static IResultable PracticeResults;
        public static IResultable PlayResults;

        // Start is called before the first frame update
        void Start ()
        {
            AssessmentResults = LoLFluencySDK.InitAssessment(OnAssessmentData);
            PracticeResults = LoLFluencySDK.InitPractice(OnPracticeData);
            PlayResults = LoLFluencySDK.InitPlay(OnPlayData);

            LoLFluencySDK.GameIsReady();
        }

        void OnAssessmentData (AssessmentData assessmentData)
        {
            AssessmentStartData = assessmentData;
            SceneManager.LoadSceneAsync("AssessmentScene");
        }

        void OnPracticeData (PracticeData practiceData)
        {
            PracticeStartData = practiceData;
            SceneManager.LoadSceneAsync("PracticeScene");
        }

        void OnPlayData (PlayData playData)
        {
            PlayStartData = playData;
            SceneManager.LoadSceneAsync("PlayScene");
        }
    }
}