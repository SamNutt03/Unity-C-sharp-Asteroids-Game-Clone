using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockets : MonoBehaviour{
    void Start(){}

    void Update(){}

    private void OnCollisionEnter2D(Collision2D collision){

        if (collision.gameObject.tag == "Bullet"){
            FindObjectOfType<GameManager>().SetScore(FindObjectOfType<GameManager>().score + 1);
            FindObjectOfType<GameManager>().missilesExplode(this);
        }
    }
}
