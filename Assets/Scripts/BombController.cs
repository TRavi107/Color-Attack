using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BombController : MonoBehaviour
{
    public TMP_Text numberText;

    #region Prefabs

    [SerializeField] GameObject bombPrefabs;
    [SerializeField] GameObject hitEffects;
    [SerializeField] GameObject explosionEffects;

    #endregion

    #region Serialized Private Fields

    [SerializeField] private float flowingSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] float maxScale;
    [SerializeField] float minScale;
    [SerializeField] float scale;

    #endregion

    #region Private Fields

    int myNumber;
    int originalNumber;

    #endregion

    #region Public Fields

    #endregion

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        numberText.text = myNumber.ToString();
        scale = Random.Range(minScale, maxScale);
        transform.localScale = scale*Vector2.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraController.instance.CheckTwiceBound(transform.position))
        {
            Destroy(this.gameObject);
        }
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

    public void DecreaseNumber()
    {
        myNumber--;
        //if(rb.velocity.y < 0)
        //{
        //    rb.AddForce(10* Vector2.up, ForceMode2D.Impulse);
        //}
        GameManager.instance.AddScore(1);
        soundManager.instance.PlaySound(SoundType.hit);
        Instantiate(hitEffects, transform.position, Quaternion.identity);
        if (myNumber <= 0)
        {
            if (originalNumber >10 )
            {
                bool leftDir = true;
                for (int i = 0; i < 2; i++)
                {
                    GameObject bomb = Instantiate(bombPrefabs, transform.position, Quaternion.identity);
                    bomb.GetComponent<BombController>().SetNumber ((originalNumber+1) / 2);
                    if (leftDir)
                        bomb.GetComponent<BombController>().AddForce(Vector2.left, 2);
                    else
                        bomb.GetComponent<BombController>().AddForce(Vector2.right, 2);

                    bomb.GetComponent<BombController>().AddForce(Vector2.up, 2);
                    leftDir = !leftDir;
                }
            }
            Destroy(this.gameObject);
            GameManager.instance.AddScore(originalNumber);
            soundManager.instance.PlaySound(SoundType.explosion);
            Instantiate(explosionEffects, transform.position, Quaternion.identity);

        }
        numberText.text = myNumber.ToString();
    }

    #endregion
}
