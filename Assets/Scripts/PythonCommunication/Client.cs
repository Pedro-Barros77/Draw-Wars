using System;
using System.Threading.Tasks;
using UnityEngine;

public class Client : MonoBehaviour
{
    private Requester _Requester;
    public float lastResult = 0;
    public bool hasResult = false;

    private void Awake()
    {
        _Requester = new Requester();
    }

    private void OnDestroy()
    {
        _Requester.Stop();
    }

    public void SendImages(byte[] img1, byte[] img2)
    {
        _Requester = new Requester();
        string byte1 = Convert.ToBase64String(img1);
        string byte2 = Convert.ToBase64String(img2);

        _Requester.MessagesToSend = new string[] { byte1, byte2 };
        _Requester.Start();
    }
}