using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    private string receivedGesture = "";

    public PlayerController playerController; // Reference to PlayerController

    void Start()
    {
        // Initialize UDP listener
        udpClient = new UdpClient(5008); // Match the port from Python
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5005);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                receivedGesture = Encoding.UTF8.GetString(data);
            }
            catch { }
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(receivedGesture))
        {
            Debug.Log("Received Gesture: " + receivedGesture); // Print in Unity console

            // Map received gestures to player actions
            switch (receivedGesture.Trim())
            {
                case "open":
                    playerController.MoveRight();
                    break;
                case "close":
                    playerController.StopMoving();
                    break;
                case "side":
                    playerController.MoveLeft();
                    break;
                case "index":
                    playerController.Jump(playerController.jumpForce);

                    break;
                case "thumbsdown":
                    playerController.Slide();
                    break;
            }

            receivedGesture = ""; // Reset after processing
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }
}
