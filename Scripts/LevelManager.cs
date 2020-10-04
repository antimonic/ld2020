using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Scene[] levels;
    public int currentLevel = 0;
    public static LevelManager Instance;

    public void NextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene("Level" + (currentLevel + 1));
    }
    private void Awake()
    {
        if (Instance)
            Destroy(this);

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
