using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct SpawnPos
{
    public Transform spawnPosition;
    [Range(0, 1000)]
    public int force;
}

public class GameManager : MonoBehaviour
{
    public bool drawGizmos;

    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion

    #region Transforms

    #endregion

    #region Prefabs

    [SerializeField] GameObject bombPrefab;

    #endregion

    #region List of objects
    [SerializeField] private SpawnPos[] spawnPos;
    #endregion

    #region Private Serialized Fields

    [SerializeField] float spawnDuration;
    [SerializeField] int currentLevel;

    #endregion

    #region Private Fields

    bool paused;
    int remainingBombs;

    #endregion

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        IncreaseLevel();
    }

    // Update is called once per frame
    void Update()
    {
        BombController[] bombsScene = FindObjectsOfType<BombController>();
        if(bombsScene.Length == 0)
        {
            if(remainingBombs > 0)
                SpawnBombs();
            else
            {
                UpgradeCannon();
            }
        }
    }

    #endregion

    #region Private Functions

    void UpgradeCannon()
    {
        PauseGame();
        UIManager.instance.SwitchCanvas(UIPanelType.levelUp);
        IncreaseLevel();
    }

    void SpawnBombs()
    {
        int spawnIndex = Random.Range(0, spawnPos.Length);
        int dir = Random.Range(0, 2);
        int force = Random.Range(2, 5);
        GameObject bombs = Instantiate(bombPrefab, spawnPos[spawnIndex].spawnPosition.position, Quaternion.identity);
        int number = Random.Range(1, currentLevel + 1);
        bombs.GetComponent<BombController>().SetNumber(number);
        bombs.GetComponent<BombController>().AddForce(dir == 0 ? Vector2.left : Vector2.right, force);
        remainingBombs--;
    }

    void IncreaseLevel()
    {
        currentLevel += 1;
        remainingBombs = currentLevel + 1;
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
