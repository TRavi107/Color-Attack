using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    #region Transform Positions
    [SerializeField] CapsuleCollider2D myCollider;

    [SerializeField] Transform firePos;
    public Transform cannonHolder;
    public Transform effectPos;
    public Transform poofPos;
    [SerializeField] List<Transform> cannonsInHolder;

    public SpriteRenderer[] cannonSprites;
    public SpriteRenderer cannonMainSprites;

    #endregion

    public float traumaAmount;
    public ShakeData cameraShakeData;
    public ShakeData cameraMIniShakeData;
    public GameObject cannonShotEffectPrefab;
    public GameObject cannonPoofEffectPrefab;

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

    [SerializeField] private float maxFirerate;

    [SerializeField] private float FirerateIncreaseAmount;
    [SerializeField] private float wheelRadius;

    #endregion

    #region Private Fields

    bool mouseClicked;
    bool ghostMode;

    #endregion

    #region MonoDevelopFunctions
    // Start is called before the first frame update
    void Start()
    {
        //InstantiateBombsPrefabs();
        StartCoroutine("FireCannons");
        ghostMode = false;
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
            Mathf.Clamp(mousePos.x, cameraController.instance.leftBound.x - 1, cameraController.instance.rightBound.x + 1);
            mousePos.x = Mathf.Clamp(mousePos.x, cameraController.instance.leftBound.x , cameraController.instance.rightBound.x );
            Vector2 target = Vector2.Lerp(transform.position, mousePos, Time.unscaledDeltaTime * movementSpeed);
            Vector2 previousPos = transform.position;
            transform.position =new( target.x,transform.position.y);
            Vector2 newPos = transform.position;
            AnimateWheels(previousPos, newPos);
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ghostMode)
            return;
        if (collision.gameObject.CompareTag("bomb"))
        {
            if (!collision.gameObject.GetComponent<BombController>().isPowerUP)
            {
                Camera.main.GetComponent<CameraShaker>().Shake(cameraShakeData);
                StartCoroutine(nameof(GhostMode));
                GameManager.instance.DecreaseLife();
            }
            
        }
        
    }

    #endregion

    #region Public Functions

    public void IncreaseFireSpeed()
    {
        firerate += FirerateIncreaseAmount;
        if (firerate < maxFirerate)
        {
            firerate = maxFirerate;
        }
    }


    #endregion

    #region Private Functions

    void AnimateWheels(Vector2 previous,Vector2 newPos)
    {
        for (int i = 1; i < cannonSprites.Length; i++)
        {
            float angle = Mathf.Rad2Deg*(newPos.x - previous.x) / (2 * Mathf.PI * wheelRadius);
            cannonSprites[i].transform.Rotate(new Vector3(0, 0, angle));
        }
    }

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
            cannons.gameObject.GetComponent<Rigidbody2D>().mass=Time.timeScale;
            StartCoroutine("cannonShake");

            cannons.transform.SetParent(null);
        }
        Instantiate(cannonPoofEffectPrefab, effectPos.position, Quaternion.identity);
        Instantiate(cannonShotEffectPrefab, effectPos.position, Quaternion.identity);
        //soundManager.instance.PlaySound(SoundType.shot);
        //cannonsInHolder = temp;
        cannonsInHolder.Clear();

    }

    IEnumerator cannonShake()
    {
        cannonMainSprites.transform.position = new Vector2(cannonMainSprites.transform.position.x, cannonMainSprites.transform.position.y - 0.02f);
        yield return new WaitForSecondsRealtime(0.05f);
        cannonMainSprites.transform.position = new Vector2(cannonMainSprites.transform.position.x, cannonMainSprites.transform.position.y + 0.02f);
        yield return new WaitForSecondsRealtime(0.05f);
        cannonMainSprites.transform.position = new Vector2(cannonMainSprites.transform.position.x, cannonMainSprites.transform.position.y + 0.01f);
        yield return new WaitForSecondsRealtime(0.05f);
        cannonMainSprites.transform.position = new Vector2(cannonMainSprites.transform.position.x, cannonMainSprites.transform.position.y - 0.01f);
        yield return new WaitForSecondsRealtime(0.05f);
        
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
            yield return new WaitForSecondsRealtime(firerate);
        }
    }

    IEnumerator GhostMode()
    {
        ghostMode = true;
        bool isTransparent = false;
        for (int i = 0; i < 20; i++)
        {
            foreach (SpriteRenderer sprite in cannonSprites)
            {
                sprite.gameObject.SetActive(isTransparent);
            }
            isTransparent = !isTransparent;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        ghostMode = false;
    }

    #endregion
}
