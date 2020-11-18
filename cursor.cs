using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor : MonoBehaviour
{
    //i valori vengono dati dall'interfaccia di unity
    public float horizontalSpeed;
    public float verticalSpeed;
    private GameObject Coord;
    private Coordinate pippo;
    public Transform cursorObj;
    public Canvas game;
    public bool allowCursorMove = true;

    void CheckCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

 
    void Start()
    {
        Cursor.visible = true;
        CheckCursorLock();
        //prendo le coordinate
        Coord = GameObject.Find("SceneManager");
        pippo = Coord.GetComponent<Coordinate>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckCursorLock();
        }
        print("x: " + pippo.x+ " y: "+pippo.y);
        float h = horizontalSpeed *pippo.x;
        //metto -1 così se guardo su il cursore sale e se guardo giù scende
        float v = verticalSpeed *-1* pippo.y;
        print("h: " + h + " v: " + v);
        Vector3 delta = new Vector3(h, v, 0);
        if (allowCursorMove)
        {
 
            cursorObj.position += delta;
         
        }
    }
   
}
