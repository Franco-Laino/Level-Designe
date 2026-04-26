using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject menu;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(true);
            Time.timeScale = 0;
        }
    }
    
    public void Inicio(string escena)
    {
        SceneManager.LoadScene(escena);
    }
    public void Salir()
    {
        Application.Quit();
    }

    
    public void Continuar()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
    }
 
}
