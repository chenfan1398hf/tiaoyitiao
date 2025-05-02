using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public int boxId = 0;
    public GameObject txObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitBox(int index)
    {
        boxId = index;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("´¥·¢");
            GameManager.instance.OpenDatiPanel(boxId, this.gameObject, txObj);
        }
    }
}
