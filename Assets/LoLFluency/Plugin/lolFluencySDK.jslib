var LoLFluencyPlugin = {
	_GameIsReady: function(
        gameNamePtr,
        callbackObjectPtr,
        callbackFunctionPtr,
        sdkVersionPtr,
        sdkOptionsPtr
    ) {
        const gameName = Pointer_stringify(gameNamePtr);
        const targetGameObject = Pointer_stringify(callbackObjectPtr);
        const callbackFunction = Pointer_stringify(callbackFunctionPtr);
        const sdkVersion = Pointer_stringify(sdkVersionPtr);
        const sdkOptions = JSON.parse(Pointer_stringify(sdkOptionsPtr));

        console.log('GameIsReady() from JS');
        console.log('LoL fluency UNITY SDK version: ' + sdkVersion);
        console.log('Sending data to GameObject' + targetGameObject);
        console.log('SDK Options', sdkOptions);

        window.addEventListener('message', function(msg) {
            console.log('[PARENT => UNITY]', msg);

            var payload = undefined;

            if(sdkOptions.supportedReceiverKeys.includes(msg.data.messageName)) {
                payload = JSON.stringify({key: msg.data.messageName, value: msg.data.payload});
            } else {
                console.warn('Unhandled message: ', msg);
                // If we are the parent, we're running the fluency game without a player.
                // Return null payload data.
                switch(msg.data.messageName) {
                    case 'init':
                        payload = JSON.stringify({key: 'start'});
                        break;
                    case 'loadState':
                        payload = JSON.stringify({key: 'loadState'});
                        break;
                }
            }

            if(payload !== undefined) {
                SendMessage(
                    targetGameObject,
                    callbackFunction,
                    payload
                );
            }
        });

        // Calls init on parent (gameframe)
        parent.postMessage(
        {
            message: 'init',
            payload: JSON.stringify({
                gameName: gameName,
                sdkVersion: sdkVersion,
                sdkOptions: sdkOptions
            }),
        }, '*');
    },

    _PostWindowMessage: function(messageNamePtr, jsonPayloadPtr) {
        const messageName = Pointer_stringify(messageNamePtr);
        const jsonPayload = Pointer_stringify(jsonPayloadPtr);
        const payload = {
            message: messageName,
            payload: jsonPayload,
        };
        parent.postMessage(payload, '*');
    },
}

mergeInto(LibraryManager.library, LoLFluencyPlugin);
