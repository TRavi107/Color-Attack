using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BombController : MonoBehaviour
{
    public TMP_Text numberText;

    #region Prefabs

    [SerializeField] GameObject bombPrefabs;

    #endregion

    #region Serialized Private Fields

    [SerializeField] private float flowingSpeed;
    [SerializeField] private Rigidbody2D rb;

    #endregion

    #region Private Fields

    bool divided;
    int myNumber;
    int originalNumber;

    #endregion

    #region Public Fields



    #endregion

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        divided = false;
        numberText.text = myNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        rb.AddForce(5 * Vector2.up, ForceMode2D.Impulse);
        if (myNumber <= 0)
        {
            if (originalNumber >1 && !divided)
            {
                bool leftDir = true;
                for (int i = 0; i < 2; i++)
                {
                    GameObject bomb = Instantiate(bombPrefabs, transform.position, Quaternion.identity);
                    bomb.GetComponent<BombController>().SetNumber ((originalNumber+1) / 2);
                    bomb.GetComponent<BombController>().divided = true;
                    if (leftDir)
                        bomb.GetComponent<BombController>().AddForce(Vector2.left, 2);
                    else
                        bomb.GetComponent<BombController>().AddForce(Vector2.right, 2);

                    leftDir = !leftDir;
                }
            }
            Destroy(this.gameObject);
        }
        numberText.text = myNumber.ToString();
    }

    #endregion
}
