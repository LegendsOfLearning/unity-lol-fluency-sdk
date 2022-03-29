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
        public static AssessData AssessStartData { get; private set; }
        public static EstablishData EstablishStartData { get; private set; }
        public static PracticeData PracticeStartData { get; private set; }

        [SerializeField] UnityEngine.UI.Text text;
        const string _NoDataMsg = "No {0} provided!\nIs the client hosted by an LoL Fluency Player?";

        // Start is called before the first frame update
        void Start ()
        {
            // Register for all game types your build can support.
            LoLFluencySDK.InitAssess(OnAssessStart);
            LoLFluencySDK.InitEstablish(OnEstablishStart);
            LoLFluencySDK.InitPractice(OnPracticeData);

            // Let the sdk know to alert the Fluency Player that the client is ready.
            LoLFluencySDK.GameIsReady();

            // If you don't use a static variable for your start data, you can get the starting data from the sdk.
            // var practiceStartData = LoLFluencySDK.GetStartData<PracticeData>();

            // NOTE: if you try to get starting data for a game type that isn't valid for that session, you'll get null.
            // i.e. Fluency Player starts a game type: ESTABLISH session and unity client is requesting LoLFluencySDK.GetStartData<AssessData>()
            // So make sure the game sessions are separated in your unity client if your build can support multiple game types.
        }

        void OnAssessStart (AssessData assessData)
        {
            if (assessData == null)
            {
                text.text = string.Format(_NoDataMsg, nameof(AssessStartData));
                return;
            }

            AssessStartData = assessData;
            SceneManager.LoadSceneAsync("AssessScene");
        }

        void OnEstablishStart (EstablishData establishData)
        {
            if (establishData == null)
            {
                text.text = string.Format(_NoDataMsg, nameof(EstablishStartData));
                return;
            }

            EstablishStartData = establishData;
            SceneManager.LoadSceneAsync("establishScene");
        }

        void OnPracticeData (PracticeData practiceData)
        {
            if (practiceData == null)
            {
                text.text = string.Format(_NoDataMsg, nameof(PracticeStartData));
                return;
            }

            PracticeStartData = practiceData;
            SceneManager.LoadSceneAsync("PracticeScene");
        }
    }
}