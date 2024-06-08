using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private List<string> allMinigames = new List<string>();
    private List<string> otherScenes = new List<string>();
    private List<string> unplayedMinigames;
    private List<string> playedMinigames = new List<string>();

    [HideInInspector]
    public string ButterflyGame = "Assets/Scenes/Vlinder/ButterFlyMiniGame.unity";
    public string FireGame = "Assets/Scenes/Vuur/FireMiniGame.unity";
    
    [HideInInspector]
    public string SelectedMiniGame;
    private Dictionary<string, AsyncOperationHandle<SceneInstance>> loadedScenes = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

    public string CurrentScene;

    private bool isQuitting = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StartCoroutine(LoadAddresses("StartScreen"));
            StartCoroutine(LoadAddresses("WaitingScreen"));
            StartCoroutine(LoadMinigameAddresses());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private IEnumerator LoadAddresses(string label)
    {
        var handle = Addressables.LoadResourceLocationsAsync(label, typeof(SceneInstance));
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var location in handle.Result)
            {
                otherScenes.Add(location.PrimaryKey);
            }
        }
        else
        {
            Debug.LogError($"Failed to load addresses for label: {label}");
        }
    }

    private IEnumerator LoadMinigameAddresses()
    {
        var handle = Addressables.LoadResourceLocationsAsync("MiniGame", typeof(SceneInstance));
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var location in handle.Result)
            {
                allMinigames.Add(location.PrimaryKey);
            }
            unplayedMinigames = new List<string>(allMinigames);
        }
        else
        {
            Debug.LogError("Failed to load minigame addresses.");
        }
    }

    public void LoadRandomMinigame()
    {
        if (unplayedMinigames == null || unplayedMinigames.Count == 0)
        {
            // All minigames have been played, reset the lists
            if (playedMinigames.Count > 0)
            {
                unplayedMinigames = new List<string>(playedMinigames);
                playedMinigames.Clear();
            }
            else
            {
                Debug.LogWarning("No minigames available to play.");
                return;
            }
        }

        // Select a random minigame from the unplayed list
        int randomIndex = Random.Range(0, unplayedMinigames.Count);
        SelectedMiniGame = unplayedMinigames[randomIndex];

        // Load the selected minigame
        LoadScenes(SelectedMiniGame);

        // Move the selected minigame to the played list
        unplayedMinigames.RemoveAt(randomIndex);
        playedMinigames.Add(SelectedMiniGame);
    }

    private IEnumerator LoadSceneAsync(string address, System.Action onSceneLoaded = null)
    {
        if (loadedScenes.ContainsKey(address) && loadedScenes[address].IsValid())
        {
            Debug.LogWarning($"Scene {address} is already loaded and handle is valid.");
            yield break;
        }

        var handle = Addressables.LoadSceneAsync(address, LoadSceneMode.Additive);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Scene {address} loaded successfully.");
            loadedScenes[address] = handle;
            CurrentScene = address;
            onSceneLoaded?.Invoke();
        }
        else
        {
            Debug.LogError($"Failed to load scene {address} with status {handle.Status}.");
        }
    }


    private IEnumerator UnloadSceneAsync(string address)
    {
        if (!loadedScenes.ContainsKey(address) || !loadedScenes[address].IsValid())
        {
            Debug.LogWarning($"Scene {address} is not loaded or handle is invalid.");
            yield break;
        }

        Debug.Log($"Handle for scene {address} is valid. Starting to unload...");

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadedScenes[address].Result.Scene);
        yield return unloadOperation;

        if (unloadOperation.isDone)
        {
            Debug.Log($"Scene {address} unloaded successfully.");

            yield return new WaitForSeconds(0.5f);

            if (loadedScenes[address].IsValid())
            {
                Addressables.Release(loadedScenes[address]);
                loadedScenes.Remove(address);
            }

            // Clear the current scene if it was the one being unloaded
            if (CurrentScene == address)
            {
                // CurrentScene = null;
            }
        }
        else
        {
            Debug.LogError($"Failed to unload scene {address}.");
        }
    }

    public void LoadScenes(string address, System.Action onSceneLoaded = null)
    {
        if (!isQuitting && gameObject.activeInHierarchy)
        {
            StartCoroutine(LoadSceneAsync(address, onSceneLoaded));
        }
        else
        {
            Debug.LogError($"Cannot start coroutine because {gameObject.name} is inactive or application is quitting!");
        }
    }


    public void UnloadScenes(string address)
    {
        if (!isQuitting && gameObject.activeInHierarchy)
        {
            StartCoroutine(UnloadSceneAsync(address));
        }
        else
        {
            Debug.LogError($"Cannot start coroutine because {gameObject.name} is inactive or application is quitting!");
        }
    }

    public void HideScene(string address)
    {
        if (loadedScenes.ContainsKey(address) && loadedScenes[address].IsValid())
        {
            Scene scene = loadedScenes[address].Result.Scene;
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                obj.SetActive(false);
            }
            Debug.Log($"Scene {address} is now hidden.");
        }
        else
        {
            Debug.LogWarning($"Scene {address} is not loaded or handle is invalid.");
        }
    }

    public void ShowScene(string address)
    {
        if (loadedScenes.ContainsKey(address) && loadedScenes[address].IsValid())
        {
            Scene scene = loadedScenes[address].Result.Scene;
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                obj.SetActive(true);
            }
            CurrentScene = address;
            Debug.Log($"Scene {address} is now visible.");
        }
        else
        {
            Debug.LogWarning($"Scene {address} is not loaded or handle is invalid.");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            // StartCoroutine(UnloadAllScenes());
        }
    }

    private IEnumerator UnloadAllScenes()
    {
        List<string> keys = new List<string>(loadedScenes.Keys);
        foreach (string address in keys)
        {
            yield return UnloadSceneAsync(address);
        }
    }
}
