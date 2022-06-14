using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceMainMenuUI : MonoBehaviour
{
    public Transform fruitPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.position =new (Camera.main.ScreenToWorldPoint(fruitPos.position).x, Camera.main.ScreenToWorldPoint(fruitPos.position).y,1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
