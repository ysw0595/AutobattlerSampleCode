using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{ 
    public void Restart()
    {
        GameManager.instance.RestartPref();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMain()
    {
        GameManager.instance.putEsc = false;
        SceneManager.LoadScene("MainScene");
    }

    public void MenuText(bool putEsc)
    {
        gameObject.SetActive(putEsc);
    }
}
