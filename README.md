# unity-lol-fluency-sdk
The SDK bridge for the Unity fluency client games and the fluency game player.

After the fluency game player gets the user session data, it would wait for the `init` message from the client game and respond with a `start` msg.

```
Received game message: init {"gameName":"FluencySDK","sdkVersion":"1.0.0"}

Sent game message to iframe:
{
    "messageName": "start",
    "payload": "{\"facts\":[{\"a\":1,\"b\":2,\"op\":\"SUB\"}],\"gameType\":\"assessment\",\"version\":\"1.0.0\"}"
}
```

The SDK can be given to 3rd party or internal devs creating an game type for the fluency session. ( `ASSESSMENT`, `PRACTICE`, `PLAY` )
The game created by the dev could implement all of the game types or any subset.
The data fed from the fluency game player will dictate what type of game the client should show.

If the version of the data coming from the fluency game player is ahead of the client, the client will report an error in the console and not load.
If the game type coming from the fluency game player is not supported by the client, the client will report an error in the console and not load.

```
[LoLFluencySDK] Client and data are not compatible
Client version: 1.0.0
Data version: 2.0.0
Requested game type: ASSESSMENT
Client supported game types: ASSESSMENT PRACTICE PLAY
```

Below is an example of a client that is supporting all 3 game types
```
public static AssessmentData AssessmentStartData { get; private set; }
public static PracticeData PracticeStartData { get; private set; }
public static PlayData PlayStartData { get; private set; }

public static IResultable AssessmentResults;
public static IResultable PracticeResults;
public static IResultable PlayResults;

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
```

Below is a client only supporting the `PLAY` game type
```
public static PlayData PlayStartData { get; private set; }

public static IResultable PlayResults;

void Start ()
{
    PlayResults = LoLFluencySDK.InitPlay(OnPlayData);

    LoLFluencySDK.GameIsReady();
}

void OnPlayData (PlayData playData)
{
    PlayStartData = playData;
    // Load a scene or use this scene to start the experience.
    SceneManager.LoadSceneAsync("PlayScene");
}
```

# Testing
These testing urls point to a "wrapper" that mimics the fluency player.

Tests are using query params to mimic the player session data. When the client loads it'll display the data. The send results will log the trails array to the console.

Assessment

https://legends-of-learning-dev.s3.amazonaws.com/test/LoLFluencySDK/index.html?gameType=assessment&version=1.0.0&data={%22facts%22:[{%22a%22:1,%22b%22:2,%22op%22:%22SUB%22}]}

Practice

https://legends-of-learning-dev.s3.amazonaws.com/test/LoLFluencySDK/index.html?gameType=practice&version=1.0.0&data={"concept":"MULTIPLICATION","facts":[{"a":1,"b":2,"op":"ADD"}],"target_facts":[{"a":2,"b":4,"op":"MUL"}]}

Play

https://legends-of-learning-dev.s3.amazonaws.com/test/LoLFluencySDK/index.html?gameType=play&version=1.0.0&data={%22facts%22:[{%22a%22:1,%22b%22:2,%22op%22:%22DIV%22}],%22target_facts%22:[{%22a%22:2,%22b%22:4,%22op%22:%22MUL%22}]} (edited) 
