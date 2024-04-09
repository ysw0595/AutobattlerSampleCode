using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] PlayerController pc;
    [SerializeField] ShowGameOver text;
    [SerializeField] MenuUI menuObj;

    public int Score = 0;

    float time;

    public bool gameOver = false;
    public bool putEsc = false;
    public bool restart = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public void RestartPref()
    {
        Score = 0;
        time = 0f;
        putEsc = false;
        gameOver = false;
        restart = true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            putEsc = !putEsc;
        }

        if (!gameOver && !putEsc)
        {
            time += Time.deltaTime;
        }
        else if(gameOver && !putEsc)
        {
            menuObj.MenuText(!putEsc);
        }
    }

    public int GetTime()
    {
        return (int)time;
    }

    public void GameOver()
    {
        gameOver = true;
        text.GameOverText();
    }
}
