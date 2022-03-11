using LoL.Fluency;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SomeDev.Test
{
    [System.Serializable]
    public class GameState
    {
        public int score;
        public int hitCount;
        public int retry;
    }

    public class FluencyGameInit : MonoBehaviour
    {
        public static AssessmentData AssessmentStartData { get; private set; }
        public static PracticeData PracticeStartData { get; private set; }
        public static PlayData PlayStartData { get; private set; }

        // Start is called before the first frame update
        void Start ()
        {
            LoLFluencySDK.InitAssessment(OnAssessmentData);
            LoLFluencySDK.InitPractice(OnPracticeData);
            LoLFluencySDK.InitPlay(OnPlayData);

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