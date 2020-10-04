using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<Recorder.Recording> robotRecordings;
    private bool isPlayingBack = false;
    private bool completed = false;
    private int amountCompleted;
    private List<GameObject> playbacks;
    private int finishedPlayingBack = 0;
    public static GameManager Instance;


    [Header("Settings")]
    public float MouseSensitivity = 1;

    [Header("GameRefs")]
    [HideInInspector]
    public PlayerRobot currentPlayer;
    public GhostController ghost;
    public Text finishText;
    public Goal goal;
    public Transform winCam;

    [HideInInspector]
    public float timeSinceStart = 0;
    [HideInInspector]
    public bool gameRunning = false;

    [Header("Prefabs")]
    public GameObject ballPrefab;
    public GameObject playerRobotPrefab;
    public GameObject robotPrefab;

    [Header("UI")]
    public Text timer;
    public Image[] counters;
    public RectTransform winScreen;
    private Color originalColors;

    public void NextLevel()
    {
        LevelManager.Instance.NextLevel();
    }


    public void PlaySimRound()
    {
        foreach(Image im in counters)
        {
            im.color = originalColors;
        }
        amountCompleted = 0;
        StartGlobalPlayback();
        gameRunning = true;
        timeSinceStart = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance)
            Destroy(this);
        winScreen.gameObject.SetActive(false);
        robotRecordings = new List<Recorder.Recording>();
        playbacks = new List<GameObject>();
        Instance = this;
        finishText.enabled = false;
        originalColors = counters[0].color;
    }

    private void Update()
    {
        if (gameRunning)
        {
            timeSinceStart += Time.deltaTime;
        }else if (isPlayingBack)
        {
            timeSinceStart += Time.deltaTime;
        }
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeSinceStart / 60);
        int seconds = Mathf.FloorToInt(timeSinceStart % 60);
        int milliseconds = Mathf.FloorToInt((timeSinceStart * 100) % 60);
        timer.text = (minutes < 10 ? "0" + minutes.ToString() : minutes.ToString()) + ":" +
             (seconds < 10 ? "0" + seconds.ToString() : seconds.ToString()) + ":" +
             (milliseconds < 10 ? "0" + milliseconds.ToString() : milliseconds.ToString());
    }

    private void StopPlayback()
    {
        goal.DestroyBall();
        if(playbacks.Count > 0)
        {
            foreach(GameObject player in playbacks)
            {
                Destroy(player);
            }
            playbacks = new List<GameObject>();
        }
        finishedPlayingBack = 0;
    }

    public void StartGlobalPlayback()
    {
        if (isPlayingBack)
        {
            StopPlayback();
        }
        isPlayingBack = true;
        timeSinceStart = 0;
        foreach(Recorder.Recording rr in robotRecordings)
        {
            GameObject robot = GameObject.Instantiate(robotPrefab);
            playbacks.Add(robot);
            robot.GetComponent<Playback>().SetData(rr);
            robot.GetComponent<Playback>().StartPlayback();
        }
        goal.StartRound();
    }

    public void PlayerFinishedRecording()
    {
        gameRunning = false;
        Recorder.Recording rr = new Recorder.Recording();
        rr = currentPlayer.GetComponent<Recorder>().GetRecording();
        robotRecordings.Add(rr);
        ghost.Possess();
        Destroy(currentPlayer.gameObject);
        currentPlayer = null;
        ghost.transform.position = winCam.position;
        StartGlobalPlayback();
    }

    public void OnPlaybackFinished()
    {
        finishedPlayingBack++;
        if (finishedPlayingBack == robotRecordings.Count && !gameRunning)
        {
            StartGlobalPlayback();
        }
    }

    public void GoalComplete()
    {
        if (gameRunning)
        {
            return;
        }
        if(amountCompleted < counters.Length)
        {
        counters[amountCompleted].color = Color.green;
        }
        amountCompleted++;
        if(amountCompleted == 3)
        {
            winScreen.gameObject.SetActive(true);
            ghost.DisableMouse();
        }
    }
}
