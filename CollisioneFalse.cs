using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisioneFalse : MonoBehaviour
{
    public GameObject componenti;
    private GameController pippo;
   
    private void Start()
    {
        pippo = componenti.GetComponent<GameController>();

    }
    void OnTriggerEnter2D(Collider2D Col)
    {
        if (Col.gameObject.name == "Cursor")
        {
            pippo.collisione = "false";
            pippo.UserSelectFalse();
        }

    }


}
