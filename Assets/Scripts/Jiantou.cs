using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jiantou : MonoBehaviour
{
    private bool isLeft = true;
    private float speed = 60f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLeft)
        {
            this.transform.localPosition -= new Vector3(speed * Time.deltaTime, 0, 0);
            if (this.transform.localPosition.x <= -60f)
            {
                isLeft = false;
            }
        }
        else
        {
            this.transform.localPosition += new Vector3(speed * Time.deltaTime, 0, 0);
            if (this.transform.localPosition.x >= 60f)
            {
                isLeft = true;
            }
        }
    }

    public int GetState()
    {
        int iRet = 0;
        if (this.transform.localPosition.x >= 20)
        {
            //Ô¶
            iRet = 1;
        }
        else if (this.transform.localPosition.x <= -20)
        {
            //½ü
            iRet = 2;
        }
        else
        {
            //ºÏÊÊ
            iRet = 3;
        }
        return iRet;
    }
}
