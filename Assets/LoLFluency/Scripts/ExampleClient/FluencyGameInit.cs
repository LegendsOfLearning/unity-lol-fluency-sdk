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
        [SerializeField] FluencySessionPracticeConcept forceTestConcept;

        // Start is called before the first frame update
        void Start ()
        {
            EmbeddedFluencyPlayer.Init(new FluencyNetwork());

            // Register for all game types your build can support.
            LoLFluencySDK.Init(OnGameStart, SetUserSettings, GameType.TYPING, GameType.ASSESS, GameType.ESTABLISH, GameType.PRACTICE);

            // Let the sdk know to alert the Fluency Player that the client is ready.
            LoLFluencySDK.GameIsReady(new GameOptions { ForceTestConcept = forceTestConcept });
        }

        void SetUserSettings (UserSettings userSettings)
        {
            if (userSettings == null)
                return;

            Debug.Log($"Received user settings from Fluency Player: {JsonUtility.ToJson(userSettings)}");
        }

        void OnGameStart (GameType gameType)
        {
            switch (gameType)
            {
                case GameType.TYPING:
                    SceneManager.LoadSceneAsync("TypingScene");
                    break;
                case GameType.ASSESS:
                    SceneManager.LoadSceneAsync("AssessScene");
                    break;
                case GameType.ESTABLISH:
                    SceneManager.LoadSceneAsync("EstablishScene");
                    break;
                case GameType.PRACTICE:
                    SceneManager.LoadSceneAsync("PracticeScene");
                    break;
            }
        }
    }
}