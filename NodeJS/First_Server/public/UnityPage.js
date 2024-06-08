document.addEventListener("DOMContentLoaded", function() {
    initializeWebSocket(
        handleWebSocketMessage,
        onOpen,
        onClose,
        null, // Geen specifieke actie nodig bij het openen van de verbinding
        () => console.log('Verbinding gesloten'), // Log de sluiting van de verbinding
        (error) => console.error('WebSocket fout:', error),
    );
    Unity(); // Laad de Unity WebGL game
      // Voeg een vertraging van 1 seconde toe aan de JoinLobby-aanroep
      setTimeout(() => {
        JoinLobby();
    }, 1000); // 1000 milliseconden = 1 seconde
});

function onOpen() {
    console.log('Verbinding geopend');
    hideReconnectWidget();
}

function onClose() {
    console.log('Verbinding gesloten');
    showReconnectWidget();
}

//function for sending messages to unity
function sendMessageToUnity(message) {
    if (typeof unityInstance !== 'undefined') {
        //make sure the first string is the name of a gameobject in the unity scene (preferably the main scene)
        console.log('Bericht verzonden naar Unity:', message);
        unityInstance.SendMessage('GameManager', 'ReceiveMessageFromJavaScript', message);
    }
}

// Unity WebGL Loader 
function Unity() {
    var buildUrl = "/UnityBuild"; // Update the path to match your server setup
    var version = "v17"; // Update the version as needed
    var loaderUrl = buildUrl + "/Build/Build_" + version + ".loader.js";
    var config = {
        dataUrl: buildUrl + "/Build/Build_" + version + ".data",
        frameworkUrl: buildUrl + "/Build/Build_" + version + ".framework.js",
        codeUrl: buildUrl + "/Build/Build_" + version + ".wasm",
        streamingAssetsUrl: buildUrl + "/StreamingAssets",
        companyName: "AdamProductions",
        productName: "ItsInGame",
        productVersion: "1.0"
    };

    var container = document.querySelector(".unity-container");
    var canvas = document.querySelector("#unity-canvas");
    var loadingBar = document.querySelector("#unity-loading-bar");
    var progressBarFull = document.querySelector("#unity-progress-bar-full");
    var warningBanner = document.querySelector("#unity-warning");

    // Shows a temporary message banner/ribbon for a few seconds, or
    // a permanent error message on top of the canvas if type=='error'.
    function unityShowBanner(msg, type) {
        function updateBannerVisibility() {
            warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
        }
        var div = document.createElement('div');
        div.innerHTML = msg;
        warningBanner.appendChild(div);
        if (type === 'error') div.style = 'background: red; padding: 10px;';
        else {
            if (type === 'warning') div.style = 'background: yellow; padding: 10px;';
            setTimeout(function() {
                warningBanner.removeChild(div);
                updateBannerVisibility();
            }, 5000);
        }
        updateBannerVisibility();
    }

    var script = document.createElement("script");
    script.src = loaderUrl;
    script.onload = () => {
        createUnityInstance(canvas, config, (progress) => {
            progressBarFull.style.width = 100 * progress + "%";
        }).then((unityInstance) => {
            loadingBar.style.display = "none";
            window.unityInstance = unityInstance; // Make unityInstance globally accessible
        }).catch((message) => {
            alert(message);
        });
    };
    document.body.appendChild(script);
}

function JoinLobby(){
    if (socket.readyState === WebSocket.OPEN) {
        const message = { lobbyStatus: 'inLobby', message: 'Deze client is gemarkeerd als: zit in de lobby' };
        console.log('socket is open');
        socket.send(JSON.stringify(message));
        // window.location.href = 'UnityPage.html'; // Navigeer naar de game pagina
    } else {
        console.log('WebSocket is niet open.');
    }
}

function showReconnectWidget() {
    var widget = document.getElementById('reconnect-widget');
    widget.classList.remove('hidden');
    widget.style.display = 'flex'; // Toon de widget
}

function hideReconnectWidget() {
    var widget = document.getElementById('reconnect-widget');
    widget.classList.add('hidden');
    widget.style.display = 'none'; // Verberg de widget
}