using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject backgroundContainer; // Dit is het object waarin je de achtergrond wilt veranderen
    public GameObject Target;
    public GameObject[] backgroundPrefabs; // Array van verschillende achtergrondprefabs

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hoi");
        if (other.CompareTag("Player"))
        {
            int backgroundIndex = GetBackgroundIndexFromUser(); // Haal het gewenste achtergrondindex op van de gebruiker
            if (backgroundIndex >= 0 && backgroundIndex < backgroundPrefabs.Length)
            {
                ChangeBackground(backgroundIndex);
            }
            else
            {
                Debug.LogWarning("Ongeldige achtergrondindex opgegeven.");
            }
        }
    }

    private int GetBackgroundIndexFromUser()
    {
        // Hier kun je de logica toevoegen om de gewenste achtergrondindex van de gebruiker op te halen.
        // Dit kan bijvoorbeeld via een UI-element, een inputveld, of een andere manier die je kiest.
        // Voor deze voorbeeldcode gaan we ervan uit dat de gebruiker altijd een geldige index geeft.
        // Je kunt deze functie aanpassen aan jouw specifieke behoeften.

        return 0; // Vervang dit met de logica om de gewenste index op te halen.
    }

    private void ChangeBackground(int index)
    {
        Vector2 TempPos = new Vector2(0, 0);
        // Verwijder de huidige achtergrond als er een is
        foreach (Transform child in backgroundContainer.transform)
        {
            TempPos = child.localPosition;
            Destroy(child.gameObject);
        }

        // Instantieer de nieuwe achtergrond
        GameObject newBackground = Instantiate(backgroundPrefabs[index], backgroundContainer.transform);
        foreach (Transform child in newBackground.transform)
        {
            child.GetComponent<Parallex>().cam = Target;
        }
        newBackground.transform.localPosition = TempPos; // Zorg ervoor dat de nieuwe achtergrond op de juiste positie staat
    }
}
