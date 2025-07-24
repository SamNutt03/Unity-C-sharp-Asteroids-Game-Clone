using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alienBullet : MonoBehaviour {

    //initialising basic variables controlling bullet.
    public float speed = 100.0f;
    public float alienBulletLifetime = 5.0f;
    public int shotFromAlien = 0;

    //initialising prefabs
    public Mothership mothership;

    public Rigidbody2D rigidBody;
    //fetches rigidBody of the prefab instantiated.
    private void Awake(){
        rigidBody = GetComponent<Rigidbody2D>();
    }
    
    //method for the alien shooting, this adds force in the direction of the input and then destroys the bullet after a set time (the value of variable at the top)
    public void alienShoot(Vector2 direction){
        rigidBody.AddForce(direction * this.speed);
        Destroy(this.gameObject, this.alienBulletLifetime);
    }

    //method for reactions to collisions, if the collision is with a object with the 'player' mask then the player death SFX is played and the playerDead() method in the gameManager is ran. bullet is also destroyed.
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "Player"){
            FindObjectOfType<Player>().deathSound.Play(0);
            FindObjectOfType<GameManager>().playerDead();
            Destroy(this.gameObject, 0);
        }  
        if (collision.gameObject.tag == "Boundary" && this.shotFromAlien == 1) {
            Destroy(this.gameObject, 0);
        }
    }
}