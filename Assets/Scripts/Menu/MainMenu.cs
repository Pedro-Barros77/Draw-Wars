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
    /// Chamado ao passar com o mouse em cima do botão
    /// </summary>
    /// <param name="btn">O GameObject contendo o Componente Botão</param>
    public void OnButtonHover(GameObject btn)
    {
        if (btn.GetComponent<Button>().interactable)
            btn.GetComponent<Animator>().SetTrigger("Highlighted");
    }

    /// <summary>
    /// Chamado ao tirar o mouse de cima do botão
    /// </summary>
    /// <param name="btn">O GameObject contendo o Componente Botão</param>
    public void OnButtonLeave(GameObject btn)
    {
        if (btn.GetComponent<Button>().interactable)
            btn.GetComponent<Animator>().SetTrigger("Normal");
    }

    /// <summary>
    /// Define se está conectado ou não ao servidor, para realizar alterações estéticas
    /// </summary>
    /// <param name="connected">True se está conectado, False se não</param>
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
    /// Chamado sempre que o GameObject deste script é ativado (SetActive(true))
    /// </summary>
    private void OnEnable()
    {
        if (isConnected)
            SetConnected(true);
    }
}
