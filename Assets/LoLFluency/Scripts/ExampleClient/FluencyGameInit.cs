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
        // Start is called before the first frame update
        void Start ()
        {
            EmbeddedFluencyPlayer.Init(new FluencyNetwork(), Debug.isDebugBuild);

            // Register for all game types your build can support.
            LoLFluencySDK.Init(OnGameStart, GameType.TYPING, GameType.ASSESS, GameType.ESTABLISH, GameType.PRACTICE);

            // Let the sdk know to alert the Fluency Player that the client is ready.
            LoLFluencySDK.GameIsReady();
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