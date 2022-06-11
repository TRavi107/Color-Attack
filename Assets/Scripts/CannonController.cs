using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    #region Transform Positions

    [SerializeField] Transform firePos;
    public Transform cannonHolder;
    [SerializeField] List<Transform> cannonsInHolder;

    #endregion

    #region Singleton
    public static CannonController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion

    #region Prefabs

    [SerializeField] GameObject cannonPrefab;

    #endregion

    #region Serialized Private Fields

    [SerializeField] private float cannonForce;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float firerate;
    [SerializeField] private float ammoAmount;

    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float maxFirerate;
    [SerializeField] private float maxAmmoAmount;

    [SerializeField] private float MovementSpeedIncreaseAmount;
    [SerializeField] private float FirerateIncreaseAmount;
    [SerializeField] private float AmmoAmountIncreaseAmount;

    #endregion

    #region Private Fields

    bool mouseClicked;

    #endregion

    #region Constructors

    public CannonController()
    {

    }

    public CannonController(float movementSpeed, float firerate, float ammoAmount)
    {
        this.movementSpeed = movementSpeed;
        this.firerate = firerate;
        this.ammoAmount = ammoAmount;
    }

    #endregion

    #region MonoDevelopFunctions
    // Start is called before the first frame update
    void Start()
    {
        //InstantiateBombsPrefabs();
        StartCoroutine("FireCannons");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseClicked = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseClicked = false;
            
        }

        if (mouseClicked)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos.x < transform.position.x)
            {
                MoveLeft();
            }
            else if(mousePos.x > transform.position.x)
            {
                MoveRight();
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    #endregion

    #region Public Functions

    public void IncreaseMovementSpeed()
    {
        movementSpeed += MovementSpeedIncreaseAmount;
    }

    public void IncreaseFireSpeed()
    {
        firerate += FirerateIncreaseAmount;
    }

    public void IncreaseAmmoAmount()
    {
        ammoAmount += AmmoAmountIncreaseAmount;
        InstantiateBombsPrefabs();
        _FireCannons();
    }

    #endregion

    #region Private Functions

    void MoveLeft()
    {
        Vector2 currentPos = transform.position;
        transform.position = new Vector2(currentPos.x - movementSpeed * Time.deltaTime, currentPos.y);
    }

    void MoveRight()
    {
        Vector2 currentPos = transform.position;
        transform.position = new Vector2(currentPos.x + movementSpeed * Time.deltaTime, currentPos.y);
    }

    void _FireCannons()
    {
        
        foreach (Transform cannons in cannonsInHolder)
        {
            cannons.gameObject.GetComponent<Rigidbody2D>().AddForce(cannonForce * transform.up,ForceMode2D.Force);
            cannons.transform.SetParent(null);
        }
        //cannonsInHolder = temp;
        cannonsInHolder.Clear();

    }

    void InstantiateBombsPrefabs()
    {
        bool turnLeft = true;
        if (cannonsInHolder.Count > 0)
        {
            foreach (Transform cannons in cannonsInHolder)
            {
                if (cannons != null)
                    Destroy(cannons.gameObject);
            }
            cannonsInHolder.Clear();
        }
        GameObject tempCannon;
        for (int i = 1; i <= ammoAmount; i++)
        {
            if(ammoAmount % 2 == 0)
            {
                if (turnLeft)
                {
                    Vector2 pos = new(cannonHolder.transform.position.x- i * cannonPrefab.GetComponent<CircleCollider2D>().radius , 
                        cannonHolder.transform.position.y);
                    tempCannon= Instantiate(cannonPrefab,pos,Quaternion.identity,cannonHolder);
                }
                else
                {
                    Vector2 pos = new((i-1) * cannonPrefab.GetComponent<CircleCollider2D>().radius + cannonHolder.transform.position.x , cannonHolder.transform.position.y);
                    tempCannon=Instantiate(cannonPrefab, pos, Quaternion.identity, cannonHolder);
                }
            }
            else
            {
                if (!turnLeft)
                {
                    Vector2 pos = new(cannonHolder.transform.position.x - i * cannonPrefab.GetComponent<CircleCollider2D>().radius,
                        cannonHolder.transform.position.y);
                    tempCannon=Instantiate(cannonPrefab, pos, Quaternion.identity, cannonHolder);
                }
                else
                {
                    Vector2 pos = new((i-1) * cannonPrefab.GetComponent<CircleCollider2D>().radius + cannonHolder.transform.position.x, cannonHolder.transform.position.y);
                    tempCannon=Instantiate(cannonPrefab, pos, Quaternion.identity, cannonHolder);
                }
            }
            cannonsInHolder.Add(tempCannon.transform);
            turnLeft = !turnLeft;
        }
    }

    #endregion

    #region Couroutines

    IEnumerator FireCannons()
    {
        while (true)
        {

            InstantiateBombsPrefabs();
            _FireCannons();
            yield return new WaitForSeconds(firerate);
        }
    }

    #endregion
}
