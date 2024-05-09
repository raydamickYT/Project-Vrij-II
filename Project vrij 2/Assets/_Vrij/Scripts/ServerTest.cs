// using UnityEngine;



// public class ServerTest : MonoBehaviour
// {
//     private HubConnection hubConnection;

//     async void Start()
//     {
//         hubConnection = new HubConnectionBuilder()
//             .WithUrl("https://yourserverurl/gameHub")
//             .Build();

//         hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
//         {
//             Debug.Log($"Message received: {message}");
//         });

//         try
//         {
//             await hubConnection.StartAsync();
//             Debug.Log("Connection started");
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"Error in starting connection: {ex.Message}");
//         }
//     }

//     void OnDestroy()
//     {
//         hubConnection.StopAsync();
//         hubConnection.DisposeAsync();
//     }
// }
