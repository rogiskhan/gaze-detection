using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisioneTrue : MonoBehaviour
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
            pippo.collisione = "true";
            pippo.UserSelectTrue();
    }

}


}
