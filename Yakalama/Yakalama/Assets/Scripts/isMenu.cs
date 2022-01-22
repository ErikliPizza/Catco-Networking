using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isMenu : MonoBehaviour
{
    public GameObject leavebtn;
    bool statu;
    void Start()
    {
        statu = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            statu = !statu;
            leavebtn.SetActive(statu);
        }
    }
}
