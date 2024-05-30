using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private List<string> allMinigames = new List<string>();
    private List<string> otherScenes = new List<string>();
    private List<string> unplayedMinigames;
    private List<string> playedMinigames = new List<string>();
    private Dictionary<string, AsyncOperationHandle<SceneInstance>> loadedScenes = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();


    private void Awake()
    {
        StartCoroutine(LoadAddresses("StartScreen"));
        StartCoroutine(LoadAddresses("WaitingScreen"));
        StartCoroutine(LoadMinigameAddresses());
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
        string selectedMinigame = unplayedMinigames[randomIndex];

        // Load the selected minigame
        LoadScenes(selectedMinigame);

        // Move the selected minigame to the played list
        unplayedMinigames.RemoveAt(randomIndex);
        playedMinigames.Add(selectedMinigame);
    }

    private IEnumerator LoadSceneAsync(string address)
    {
        if (loadedScenes.ContainsKey(address))
        {
            Debug.LogWarning($"Scene {address} is already loaded.");
            yield break;
        }


        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(address, LoadSceneMode.Additive);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Scene {address} loaded successfully.");
            loadedScenes[address] = handle;
        }
        else
        {
            Debug.LogError($"Failed to load scene {address} with status {handle.Status}.");
        }

    }

    private IEnumerator UnloadSceneAsync(string address)
    {
        if (!loadedScenes.ContainsKey(address))
        {
            Debug.LogWarning($"Scene {address} is not loaded.");
            yield break;
        }

        AsyncOperationHandle<SceneInstance> handle = loadedScenes[address];

        // Controleer of de handle geldig is voordat je de scène ontlaadt
        if (!handle.IsValid())
        {
            Debug.LogError($"Handle for scene {address} is not valid before unloading.");
            yield break;
        }

        Debug.Log($"Handle for scene {address} is valid. Starting to unload...");

        // Gebruik SceneManager om de scène te ontladen
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(handle.Result.Scene);

        yield return unloadOperation;

        // Controleer de status van de ontlaadoperatie
        if (unloadOperation.isDone)
        {
            Debug.Log($"Scene {address} unloaded successfully.");

            // Kleine vertraging toevoegen om ervoor te zorgen dat de scène volledig is ontladen
            yield return new WaitForSeconds(0.5f);

            // Manueel vrijgeven van de handle, maar alleen als het nog steeds geldig is
            if (handle.IsValid())
            {
                Addressables.Release(handle);
                loadedScenes.Remove(address);
            }
            else
            {
                Debug.LogWarning($"Handle for scene {address} is no longer valid after unloading."); //kan je misschien later weghalen
            }
        }
        else
        {
            Debug.LogError($"Failed to unload scene {address}.");
        }
    }

    public void LoadScenes(string address)
    {
        if (!loadedScenes.ContainsKey(address))
        {
            Debug.Log($"Attempting to load scene {address}.");
            StartCoroutine(LoadSceneAsync(address));
        }
        else
        {
            Debug.LogWarning($"Scene {address} is already loaded.");
        }
    }

    public void UnloadScenes(string address)
    {
        if (loadedScenes.ContainsKey(address))
        {
            Debug.Log($"Attempting to unload scene {address}.");
            StartCoroutine(UnloadSceneAsync(address));
        }
        else
        {
            Debug.LogWarning($"Cannot unload scene {address} because it is not loaded.");
        }
    }
}
