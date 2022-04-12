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
        controller.SetMenu(MenuController.Pages.RoomSelector);
    }

    /// <summary>
    /// Chamado ao passar com o mouse em cima do bot�o
    /// </summary>
    /// <param name="btn">O GameObject contendo o Componente Bot�o</param>
    public void OnButtonHover(GameObject btn)
    {
        if (btn.GetComponent<Button>().interactable)
            btn.GetComponent<Animator>().SetTrigger("Highlighted");
    }

    /// <summary>
    /// Chamado ao tirar o mouse de cima do bot�o
    /// </summary>
    /// <param name="btn">O GameObject contendo o Componente Bot�o</param>
    public void OnButtonLeave(GameObject btn)
    {
        if (btn.GetComponent<Button>().interactable)
            btn.GetComponent<Animator>().SetTrigger("Normal");
    }

    /// <summary>
    /// Define se est� conectado ou n�o ao servidor, para realizar altera��es est�ticas
    /// </summary>
    /// <param name="connected">True se est� conectado, False se n�o</param>
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

    /// <summary>
    /// Chamado sempre que o GameObject deste script � ativado (SetActive(true))
    /// </summary>
    private void OnEnable()
    {
        if (isConnected)
            SetConnected(true);
    }
}
