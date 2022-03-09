var LoLFluencyPlugin = {
	_GameIsReady: function(
        gameNamePtr,
        callbackObjectPtr,
        startDataCallbackFunctionPtr,
        sdkVersionPtr
    ) {
        const gameName = Pointer_stringify(gameNamePtr);
        const targetGameObject = Pointer_stringify(callbackObjectPtr);
        const startDataFunction = Pointer_stringify(startDataCallbackFunctionPtr);
        const sdkVersion = Pointer_stringify(sdkVersionPtr);

        console.log('GameIsReady() from JS');
        console.log('LoL fluency UNITY SDK version: ' + sdkVersion);
        console.log('Sending data to GameObject' + targetGameObject);

        const EVENT = {
            RECEIVED: {
                START: 'start',
            }
        };

        window.addEventListener('message', function(msg) {
            console.log('[PARENT => UNITY]', msg);

            switch (msg.data.messageName) {
                case EVENT.RECEIVED.START:
                    SendMessage(
                        targetGameObject,
                        startDataFunction,
                        msg.data.payload
                    );
                    break;
                default:
                    console.log('Unhandled message: ' + msg);
            }
        });

        // Calls init on parent (gameframe)
        parent.postMessage(
        {
            message: 'init',
            payload: JSON.stringify({
                gameName: gameName,
                sdkVersion: sdkVersion,
            }),
        }, '*');
    },

    _PostWindowMessage: function(_messageName, _jsonPayload) {
        const messageName = Pointer_stringify(_messageName);
        const jsonPayload = Pointer_stringify(_jsonPayload);
        const payload = {
            message: messageName,
            payload: jsonPayload,
        };
        parent.postMessage(payload, '*');
    },
}

mergeInto(LibraryManager.library, LoLFluencyPlugin);
