using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private const int MATCH_DURATION = 150;
    public static GameState GameState { get; private set; }

    public float secondaryTimer;

    private float timer;

    public TextMeshProUGUI timerText;

    public RobotSpawnController spawn;

    private AudioSource player;
    [SerializeField] private AudioResource auto;
    [SerializeField] private AudioResource teleop;
    [SerializeField] private AudioResource endgame;
    [SerializeField] private AudioResource end;

    private bool triggerEnd = true;
    private bool triggerTeleop = true;
    private bool triggerEndgame = true;

    private bool isResetting = false;

    private bool countdown = true;
    public static bool canRobotMove { get; private set; }

    [SerializeField] private GameObject button;
    [SerializeField] private GameObject videoPlayer;

    public static bool isDisabled = false;

    public string[] tagsToDestroy;

    [SerializeField] private GameObject scoreCard;

    private IResettable[] resettables;

    private DriveController[] swerveControllers;


    private const int SHOW_SCORE_DELAY = 4;
    private const int AUTO_TO_TELEOP_DELAY = 3;

    public static bool endBuzzerPlaying = false;

    private void Start()
    {
        swerveControllers = FindObjectsByType<DriveController>(FindObjectsSortMode.None);

        //Find all script instances that implement the IResettable interface
        resettables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IResettable>().ToArray();

        //Set initial gamestate
        GameState = GameState.Auto;

        //Reset flags and get components
        player = GetComponent<AudioSource>();

        ResetTimer();
        canRobotMove = true;
    }

    //Methods runs once every update timestep
    private void Update()
    {
        secondaryTimer = timer;

        if (!isResetting) 
        {
            //Key binds to forcefully end match prematurely
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     ForceEndMatch();
            // }

            if (countdown) { timer -= Time.deltaTime; }

           

            if (timer <= 0f && triggerEnd)
            {
                endBuzzerPlaying = true;
                isDisabled = true;
                triggerEnd = false;
                timer = 0f;
                player.resource = end;
                player.Play();
                StartCoroutine(WaitEndgame());
                countdown = false;
                timerText.color = Color.red;
                StartCoroutine(ShowMatchScore());
            }
            else if (timer < 136f && triggerTeleop)
            {
                triggerTeleop = false;
                StartCoroutine(Wait());
            }
            else if (timer <= 30f && triggerEndgame)
            {
                triggerEndgame = false;
                GameState = GameState.Endgame;
                player.resource = endgame;
                player.Play();
            }
            UpdateTimerDisplay(timer);
        }
    }

    private IEnumerator EndBuzzerTracker() 
    {
        while (player.isPlaying) 
        {
            yield return null;
        } 
        endBuzzerPlaying = false;
    }

    private IEnumerator ShowMatchScore()
    {

        button.SetActive(true);

        //Waits for a constant delay until showing the video and/or score button
        yield return new WaitForSeconds(SHOW_SCORE_DELAY);

        //Load the player preference for showing the match win video
        if (PlayerPrefs.GetFloat("endVideo") == 1) { videoPlayer.SetActive(true); }
    }

    //Creates a pause inbetween auto and teleop, plays sounds, and set the note worths correctly
    IEnumerator Wait()
    {
        countdown = false;
        player.resource = end;
        player.Play();
        canRobotMove = false;
        isDisabled = true;

        yield return new WaitForSeconds(1);
        isDisabled = false;
        canRobotMove = true;
        countdown = true;
        player.resource = teleop;
        player.Play();
        yield return new WaitForSeconds(3);
        GameState = GameState.Teleop;
    }

    IEnumerator WaitEndgame()
    {
        countdown = false;
        canRobotMove = false;
        isDisabled = true;
        yield return new WaitForSeconds(5);
        GameState = GameState.End;
        isDisabled = false;
        canRobotMove = true;
    }

    private void ForceEndMatch() 
    {   
        StopAllCoroutines();
        triggerEnd = true;
        triggerEndgame = false;
        triggerTeleop = false;
        timer = 0;
    }

    //Resets timer to 0 and sets color back to white
    private void ResetTimer() 
    {
        player.resource = auto;
        player.Play();

        isDisabled = false;
        countdown = true;
        triggerEnd = true;
        triggerEndgame = true;
        triggerTeleop = true;
        endBuzzerPlaying = false;

        timer = MATCH_DURATION;
        timerText.color = Color.white;
    }

    public void Reset()
    {
        if (!isResetting)
        {
            ResetMatch();
        }
    }

    private void ResetMatch()
    {
        StopAllCoroutines();
        isResetting = true;

        isDisabled = false;
        canRobotMove = true;

        //Play fade animation
        LevelManager.Instance.PlayTransition("CrossFade");

        //Disable score card (if on)
        scoreCard.SetActive(false);

        //Stop any sounds
        player.Stop();

        //Reset initial gamestate
        GameState = GameState.Auto;

        if (GameObject.FindGameObjectsWithTag("MainCamera") != null)
        {
            GameObject[] Cameras = GameObject.FindGameObjectsWithTag("MainCamera");

            foreach (GameObject Camera in Cameras)
            {
                Camera.transform.parent = spawn.transform;
            }
        }

        foreach (string tagToDestroy in tagsToDestroy)
        {
            if (GameObject.FindGameObjectsWithTag(tagToDestroy) != null)
            {
                GameObject[] notes = GameObject.FindGameObjectsWithTag(tagToDestroy);

                foreach (GameObject note in notes)
                {
                    Destroy(note);
                }
            }
        }
        //Reset things on field


        StartCoroutine(spawnBots());

        button.SetActive(false);
        isResetting = false;

        //Reset the timer to prepare for the match
        ResetTimer();
    }

    //Updates the timer to correctly display the minutes and seconds left
    private void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        string currentTime = string.Format("{00}:{1:00}", minutes, seconds);

        timerText.text = currentTime;
    }

    public static void ToggleCanRobotMove()
    {
        canRobotMove = !canRobotMove;
    }

    private IEnumerator spawnBots()
    {
        yield return new WaitForSeconds(0.2f);
        spawn.Respawn();
    }
}
