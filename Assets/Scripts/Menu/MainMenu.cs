using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public MenuController controller;

    [SerializeField]
    private Button btnPlay, btnGallery, btnOptions, btnAbout;
    [SerializeField] private GameObject loading;

    bool isConnected;

    private void Awake()
    {
        SetConnected(false);
    }

    public void Play()
    {
        controller.SetMenu("EnterRoom");
    }

    public void OnButtonHover(GameObject btn)
    {
        if (btn.GetComponent<Button>().interactable)
            btn.GetComponent<Animator>().SetTrigger("Highlighted");
    }
    public void OnButtonLeave(GameObject btn)
    {
        if (btn.GetComponent<Button>().interactable)
            btn.GetComponent<Animator>().SetTrigger("Normal");
    }

    public void SetConnected(bool connected)
    {
        isConnected = connected;
        if (connected)
        {
            btnPlay.GetComponent<Animator>().SetTrigger("Normal");
            btnGallery.GetComponent<Animator>().SetTrigger("Normal");
            btnOptions.GetComponent<Animator>().SetTrigger("Normal");
            btnAbout.GetComponent<Animator>().SetTrigger("Normal");

            loading.SetActive(false);
        }
        else
        {
            btnPlay.GetComponent<Animator>().SetTrigger("Disabled");
            btnGallery.GetComponent<Animator>().SetTrigger("Disabled");
            btnOptions.GetComponent<Animator>().SetTrigger("Disabled");
            btnAbout.GetComponent<Animator>().SetTrigger("Disabled");

            loading.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (isConnected)
            SetConnected(true);
    }
}
