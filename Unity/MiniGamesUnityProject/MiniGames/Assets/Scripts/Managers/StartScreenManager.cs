using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        GameManager.Instance.OnStartButtonPressed();
    }
}
