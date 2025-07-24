//Code for bullet movement, created by Sam Nuttall for my Asteroids game.
//This C# Scripting code effects the bullet sprite which is the laser that is shot in game.

using UnityEngine;

public class Bullet : MonoBehaviour {

    //initialise variables which will be attributes of the players bullet. the speed and the max lifetime of the bullet on the game.
    public float speed = 500.0f;
    public float maxTime = 5.0f;

    private Rigidbody2D rigidBody;

    //when the bullet is instantiated the rigidBody is fetched as the component of the prefab.
    private void Awake(){
        rigidBody = GetComponent<Rigidbody2D>();
    }

    //shoot method for the bullet. Adds force to the bullet in the direction that is used as the input and then destroys the bullet after given time travelling in that direction.
    public void Shoot(Vector2 direction){
        rigidBody.AddForce(direction * this.speed);
        Destroy(this.gameObject, this.maxTime);
    }

    //if the bullet collides with anything then this method destroys the bullet, no matter what it collides with.
    private void OnCollisionEnter2D(Collision2D collision){
        Destroy(this.gameObject);
    }

}
