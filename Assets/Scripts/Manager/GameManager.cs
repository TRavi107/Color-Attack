using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RamailoGames;

[System.Serializable]
public struct SpawnPos
{
    public Transform spawnPosition;
    [Range(0, 1000)]
    public int force;
}

public struct Level
{
    public int level;
    public float firerate;
    public int damageAmmount;
}

public class GameManager : MonoBehaviour
{
    public bool drawGizmos;

    #region Tmp Text

    public TMP_Text GameOverScoreText;
    public TMP_Text GameOverhighscoreText;
    public TMP_Text congratulationText;
    public TMP_Text scoreText;
    public TMP_Text gamePlayhighscoreText;

    #endregion

    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion

    #region Transforms

    public Transform lifeContainer;
    public Image expImage;
    public Transform clockImage;
    #endregion

    #region Prefabs

    [SerializeField] GameObject bombPrefab;
    [SerializeField] private GameObject[] powerUps;
    #endregion

    #region List of objects
    [SerializeField] private SpawnPos[] spawnPos;
    #endregion


    #region Private Serialized Fields

    [SerializeField] float spawnDuration;
    [SerializeField] int currentLevel;
    [SerializeField] int lifes;
    [SerializeField] int score;
    [Range(0,100)]
    [SerializeField] int powerUpSpawnChance;

    #endregion

    #region Private Fields

    bool paused;
    int remainingBombs;
    bool ghostMode;
    float startTime;
    int numberInBall = 10;

    #endregion

    #region Public Fields
    public int cannonDamageAmount;
    public Canvas effectCanvas;

    public GameObject ComboObjPrefab;

    private GameObject comboSpwaned;

    #endregion

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        //IncreaseLevel();
        ghostMode = false;
        score = 0;
        startTime = Time.unscaledTime;
        numberInBall = 10;
        StartCoroutine(nameof(SpawnBombsCouroutine));
        ScoreAPI.GameStart((bool s) => {
        });
    }

    // Update is called once per frame
    void Update()
    {
        BombController[] bombsScene = FindObjectsOfType<BombController>();
        if (bombsScene.Length == 0)
        {
            if (remainingBombs > 0)
                SpawnBombs();
            else
            {
                UpgradeCannon();
            }
        }
        if (clockImage.gameObject.activeInHierarchy)
        {
            clockImage.GetChild(0).transform.Rotate(new Vector3(0, 0, -120 *Time.unscaledDeltaTime));
        }
    }



    #endregion

    #region Public Functions

    public void DecreaseLife()
    {
        lifes--;
        if (lifes <= 0)
        {
            GameOver();
        }
        Destroy(lifeContainer.GetChild(lifeContainer.childCount - 1).gameObject);
    }
    public void SlowTime(float amount,float duration)
    {
        clockImage.gameObject.SetActive(true);
        clockImage.gameObject.transform.rotation = Quaternion.identity;
        Time.timeScale = amount;
        StartCoroutine(nameof(restoreTime), duration);
    }

    IEnumerator restoreTime(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        clockImage.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    #endregion

    #region Private Functions
    public void ShowCombo(Vector3 pos, string combotext)
    {
        StopCoroutine("AutoDisableCombo");
        if (comboSpwaned == null)
            comboSpwaned = Instantiate(ComboObjPrefab, effectCanvas.transform);
        EnableCombo();
        comboSpwaned.transform.position = pos;
        comboSpwaned.GetComponent<TMP_Text>().text = combotext;
        StartCoroutine("AutoDisableCombo");
    }
    void DisableCombo()
    {
        if (comboSpwaned != null)
            comboSpwaned.SetActive(false);
    }
    void EnableCombo()
    {
        if (comboSpwaned != null)
            comboSpwaned.SetActive(true);
    }

    IEnumerator AutoDisableCombo()
    {
        yield return new WaitForSecondsRealtime(1);
        DisableCombo();
        // Code to execute after the delay
    }
    void GameOver()
    {
        PauseGame();
        UIManager.instance.SwitchCanvas(UIPanelType.GameOver);
        UIManager.instance.SwitchCanvas(UIPanelType.GameOver);
        //fruitsCutText.text = "Fruits Cut :  " + fruitscut.ToString();
        GameOverScoreText.text = "Score:            " + score.ToString();
        //MaxComboText.text = "Max Combo:  " + maxCombo.ToString();
        int playTime = (int)(Time.unscaledTime - startTime);
        ScoreAPI.SubmitScore(score, playTime, (bool s, string msg) => { });
        GetHighScore();
    }

    void UpgradeCannon()
    {
        CannonController.instance.IncreaseFireSpeed();
        IncreaseLevel();
    }

    void SpawnBombs()
    {
        
        int dir = Random.Range(0, 2);
        int force = Random.Range(2, 5);
        int spawnIndex = Random.Range(0, spawnPos.Length);
        

        if (Random.Range(0, 100) < powerUpSpawnChance && currentLevel>=3)
        {
            if (GameObject.Find("slowBomb") == null)
            {
                int powerUpIndex = Random.Range(0, powerUps.Length);
                GameObject powerUp = Instantiate(powerUps[powerUpIndex], spawnPos[spawnIndex].spawnPosition.position, Quaternion.identity);
                powerUp.GetComponent<BombController>().SetNumber(numberInBall/2);
                powerUp.GetComponent<BombController>().AddForce(dir == 0 ? Vector2.left : Vector2.right, force);
                powerUp.GetComponent<BombController>().isPowerUP = true;
            }
        }
        else
        {
            GameObject bombs = Instantiate(bombPrefab, spawnPos[spawnIndex].spawnPosition.position, Quaternion.identity);
            bombs.GetComponent<BombController>().SetNumber(numberInBall);
            bombs.GetComponent<BombController>().AddForce(dir == 0 ? Vector2.left : Vector2.right, force);
        }
        remainingBombs--;

    }

    void IncreaseLevel()
    {
        currentLevel += 1;
        remainingBombs = currentLevel;
        if (currentLevel == 1)
        {
            numberInBall = 10;
            cannonDamageAmount = 1;
        }
        else
        {
            numberInBall *= 2;
            cannonDamageAmount +=2;
        }


        StopCoroutine(nameof(SpawnBombsCouroutine));
        StartCoroutine(nameof(SpawnBombsCouroutine));
    }

    public void PauseGame()
    {
        //UIManager.instance.DisableCombo();
        //setHighScore(pausehighscoreText);
        //int min = (int)gameTimer / 60;
        //int sec = (int)gameTimer % 60;
        //pauseMenugameTimerText.text = min.ToString() + ":" + sec.ToString();
        Time.timeScale = 0;
        paused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        paused = false;
    }

    void setHighScore(TMP_Text highscroreTextUI)
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                if (score >= d.high_score)
                {
                    highscroreTextUI.text = score.ToString();

                }
                else
                {
                    highscroreTextUI.text = d.high_score.ToString();
                }

            }
        });
    }
    public void AddScore(int amount)
    {
        score += amount ;
        scoreText.text = score.ToString();
        setHighScore(gamePlayhighscoreText);
    }


    void GetHighScore()
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                if (score >= d.high_score)
                {
                    GameOverhighscoreText.text = "High Score : " + score.ToString();
                    congratulationText.gameObject.SetActive(true);

                }
                else
                {
                    GameOverhighscoreText.text = "High Score : " + d.high_score.ToString();
                }

            }
        });

    }

    #endregion

    #region Coroutines

    IEnumerator SpawnBombsCouroutine()
    {
        while (remainingBombs > 0)
        {
            if (!paused)
            {
                SpawnBombs();
            }
            yield return new WaitForSecondsRealtime(spawnDuration);
        }
    }

    #endregion
}
