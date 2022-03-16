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
        public static InstructData InstructStartData { get; private set; }
        public static RetrieveData RetrieveStartData { get; private set; }

        // Start is called before the first frame update
        void Start ()
        {
            // Register for all game types your build can support.
            LoLFluencySDK.InitAssess(OnAssessStart);
            LoLFluencySDK.InitInstruct(OnInstructStart);
            LoLFluencySDK.InitRetrieve(OnRetrieveStart);

            // Let the sdk know to alert the Fluency Player that the client is ready.
            LoLFluencySDK.GameIsReady();

            // If you don't use a static variable for your start data, you can get the starting data from the sdk.
            // var retrieveStartData = LoLFluencySDK.GetStartData<RetrieveData>();

            // NOTE: if you try to get starting data for a game type that isn't valid for that session, you'll get null.
            // i.e. Fluency Player starts a game type: INSTRUCT session and unity client is requesting LoLFluencySDK.GetStartData<AssessData>()
            // So make sure the game sessions are separated in your unity client if your build can support multiple game types.
        }

        void OnAssessStart (AssessData assessData)
        {
            AssessStartData = assessData;
            SceneManager.LoadSceneAsync("AssessScene");
        }

        void OnInstructStart (InstructData instructData)
        {
            InstructStartData = instructData;
            SceneManager.LoadSceneAsync("InstructScene");
        }

        void OnRetrieveStart (RetrieveData retrieveData)
        {
            RetrieveStartData = retrieveData;
            SceneManager.LoadSceneAsync("RetrieveScene");
        }
    }
}