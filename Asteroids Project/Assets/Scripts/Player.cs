//Code for player movement, created by Sam Nuttall for my Asteroids game.
//This C# Scripting code effects the player sprite which is the ship in game.

using UnityEngine;

public class Player : MonoBehaviour {

    //initialise the bullet prefab that will be used when instantiating bullets.
    public Bullet bulletPrefab;

    //variables for the movement
    private bool Forward;
    private bool Backward;
    private float Turn;

    //variables the attributes of the player movement mechanics.
    public float ForwardSpeed = 1.0f;
    public float BackwardSpeed = 1.0f;
    public float TurnSpeed = 1.0f;
    public float fireRate = 0.175f;
    private float lastShot = 0.0f;

    //audio sources storing the death sound and the shooting sound effects.
    public AudioSource shootingSound;
    public AudioSource deathSound;

    //declaring the variable which will store the rigidbody component
    private Rigidbody2D rigidBody;

    //fetches the rigidbody component when the player is first instantiated.
    private void Awake(){
        rigidBody = GetComponent<Rigidbody2D>();
    }

    //constantly checking the user input. if there is user input then it will run functions based off which key it is. WASD control movement and turning and spacebar or left-click will run the shoot method.
    private void Update(){
        Forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        Backward = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            Turn = 1.0f;
        }else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            Turn = -1.0f;
        }else {
            Turn = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){
            Shooting();
        }
    }

    //if the movement is forward then there is a forward force applied to the user in the direction they are facing, at a given speed. the user has drag also so they can decelerate etc. backword movement adds more drag to show deceleration. and the turn variable is used to add torque to the user, which changes their rotation.
    private void FixedUpdate(){ //This runs after fixed times
        if (Forward) {
            rigidBody.drag = 1;
            rigidBody.AddForce(this.transform.up * this.ForwardSpeed);
        }else if (Backward) {
            rigidBody.drag = 3;
        }
        if (Turn != 0.0f) {
            rigidBody.AddTorque(Turn * this.TurnSpeed);
        }
    }

    //method for instantiating the bullet when the user shoots, this will spawn the bullet and then pass the 'forward' direction to the shoot method to add movement in this direction to the bullet, the bullet shooting sfx is also played.
    private void Shooting(){
        if (Time.time > fireRate + lastShot){
            Bullet bullet = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);
            bullet.Shoot(this.transform.up);
            shootingSound.Play(0);
            lastShot = Time.time;
        }  
    }
    
    //if the player collides with an asteroid, then the explosion sound plays and the game object is deactivated. the playerdead function is ran.
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "Asteroid"){
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0.0f;
            deathSound.Play(0);
            this.gameObject.SetActive(false);

            FindObjectOfType<GameManager>().playerDead();
        }
        // if the player collides with a mothership then the same happens.
        if (collision.gameObject.tag == "Mothership"){
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0.0f;
            deathSound.Play(0);
            this.gameObject.SetActive(false);
            
            FindObjectOfType<GameManager>().playerDead();
        }
        //if the player collides with an alien bullet they also die and the sfx is played again, all the alien bullets are removed from the scene, and then the playerDead function is ran again.
        if (collision.gameObject.tag == "AlienBullet"){
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0.0f;
            deathSound.Play(0);
            this.gameObject.SetActive(false);

            alienBullet[] alien = FindObjectsOfType<alienBullet>();

            for (int i = 0; i < alien.Length; i++) {
                Destroy(alien[i].gameObject);
            }

            FindObjectOfType<GameManager>().playerDead();
        }

        if (collision.gameObject.tag == "Missile"){
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0.0f;
            deathSound.Play(0);
            this.gameObject.SetActive(false);

            Rockets[] missiles = FindObjectsOfType<Rockets>();

            for (int i = 0; i < missiles.Length; i++) {
                FindObjectOfType<GameManager>().missilesExplode(missiles[i]);
            }

            FindObjectOfType<GameManager>().playerDead();
        }
    }
}
