using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IterationManager : Singleton<IterationManager>
{
    [Header("Settings")]
    public int IterationAmount = 1000;
    private int currentIteration = 0;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void RestartMatch()
    {
        currentIteration++;

        if (currentIteration <= IterationAmount)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
