using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public enum colorName
{
    red,
    yellow,
    orange,
    green,
    brown,
    blue,
    purple,
    random,
}



public class BombController : MonoBehaviour
{
    public TMP_Text numberText;
    public SpriteRenderer spriteRenderer;

    #region Prefabs

    [SerializeField] GameObject bombPrefabs;
    [SerializeField] GameObject hitEffects;
    [SerializeField] GameObject explosionEffects;

    [SerializeField] GameObject flarePrefab;
    [SerializeField] Transform flarePos;

    #endregion

    #region Serialized Private Fields

    [SerializeField] private float flowingSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] float maxScale;
    [SerializeField] float minScale;
    [SerializeField] float scale;

    [SerializeField] float maxRot;
    [SerializeField] float minRot;
    [SerializeField] float Rot;

    #endregion

    #region Private Fields

    int myNumber;
    int originalNumber;
    public bool isSplited;
    public bool isPowerUP;

    #endregion

    #region Public Fields

    #endregion

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        numberText.text = myNumber.ToString();
        scale = Random.Range(minScale, maxScale);
        Rot = Random.Range(minRot, maxRot);
        transform.localScale = scale*Vector2.one;
        GameObject flareobj = Instantiate(flarePrefab, flarePos.position, Quaternion.identity, this.transform);
        flareobj.transform.localScale = 0.2f * Vector2.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraController.instance != null)
        {
            if (cameraController.instance.CheckTwiceBound(transform.position))
            {
                Destroy(this.gameObject);
            }
        }
        transform.Rotate(new Vector3(0, 0, Rot*Time.deltaTime));
    }

    #endregion

    #region Private Functions

    #endregion

    #region Public Functions

    public void SetNumber(int number)
    {
        myNumber = number;
        originalNumber = number;
    }
    public void AddForce(Vector2 direction, float force)
    {
        float totalforce = (force + flowingSpeed);
        rb.AddForce(totalforce * direction, ForceMode2D.Impulse);
    }

    public void manualDestroy()
    {
        Instantiate(explosionEffects, transform.position, Quaternion.identity);
        this.gameObject.SetActive(false);
        soundManager.instance.PlaySound(SoundType.explosion);
        Invoke(nameof(ChangeScene), 0.7f);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(2);
    }

    [System.Obsolete]
    public void DecreaseNumber(Vector2 position,int damage)
    {
        myNumber -= damage;
        //if(rb.velocity.y < 0)
        //{
        //    rb.AddForce(10* Vector2.up, ForceMode2D.Impulse);
        //}
        GameManager.instance.AddScore(1);
        soundManager.instance.PlaySound(SoundType.hit);
        Instantiate(hitEffects, position, Quaternion.identity);
        if (myNumber <= 0)
        {
            if (!isPowerUP)
            {
                if (originalNumber > 10 && !isSplited)
                {
                    bool leftDir = true;
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject bomb = Instantiate(bombPrefabs, transform.position, Quaternion.identity);
                        bomb.GetComponent<BombController>().SetNumber((originalNumber + 1) / 2);
                        if (leftDir)
                            bomb.GetComponent<BombController>().AddForce(Vector2.left, 2);
                        else
                            bomb.GetComponent<BombController>().AddForce(Vector2.right, 2);

                        bomb.GetComponent<BombController>().AddForce(Vector2.up, 2);
                        bomb.GetComponent<BombController>().isSplited = true;
                        leftDir = !leftDir;
                    }
                }
            }
            else
            {
                GameManager.instance.SlowTime(0.4f, 5f);
                GameManager.instance.ShowCombo(transform.position, "Time Slowed");
            }
            Destroy(this.gameObject);
            GameManager.instance.AddScore(originalNumber);
            soundManager.instance.PlaySound(SoundType.explosion);
            GameObject effect =Instantiate(explosionEffects, transform.position, Quaternion.identity);
            Transform smoke=effect.transform.GetChild(2);
            //GameManager.instance.AddLife();

            }
            numberText.text = myNumber.ToString();
    }

    #endregion
}
