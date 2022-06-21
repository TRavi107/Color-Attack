using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{
    public GameObject bombEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(bombEffectPrefab, transform.position, Quaternion.identity, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraController.instance.CheckBound(transform.position))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("bomb"))
        {
            collision.gameObject.GetComponent<BombController>().DecreaseNumber(transform.position,GameManager.instance.cannonDamageAmount);
            Destroy(this.gameObject);
        }
    }
}
