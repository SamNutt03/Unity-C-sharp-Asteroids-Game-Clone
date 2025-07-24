using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAI : MonoBehaviour {

    //initialises the gameManager link.
    public GameManager GameManage;

    public Sprite[] sprites;
    
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;

    //initialise the prefab links for the alienBullet, the player and the alien model.
    public alienBullet alienBulletPrefab;
    public GameObject player;
    public AlienAI alienPrefab; 

    //a range of variable initialisations for variables that will be changed throughout the code / manage the behaviour of the alien.
    private Vector2 randomMovement;
    public float speed;
    private float distance;
    public float followDistance;
    public float freeSpace;
    public float accelerationTime;
    public float maxSpeed;
    private float timeLeft;
    private int alienHits = 0;
    

    //initiailes a variable which refers to the sprite renderer and rigidbody of the prefab initialised.
    private void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }


    //when the prefab is first initialised it will located the 'Player' named prefab and then invoke a repeating function of the alienShoot method every 5 seconds.
    void Start(){
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        player = GameObject.Find("Player");
        InvokeRepeating(nameof(alienShoot), 5.0f, 5.0f);//shoots every 5 seconds.
    }


    //collison reaction methods for the aliens, if the alien collides with a 'Bullet' tagged object then the hits counter is incremented and the alien hit sound effect is played. if the alien has been hit 2 times then it dies and the alienDead method is ran.
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "Bullet"){

            rigidBody.velocity = new Vector3(0,0,0);

            this.alienHits = this.alienHits + 1;

            if (this.alienHits <= 2){
                Destroy(collision.gameObject);
                FindObjectOfType<GameManager>().alienHitSound();
            }
            if (this.alienHits == 3) {
            Destroy(collision.gameObject);
            FindObjectOfType<GameManager>().alienDead(this);
            Destroy(this.gameObject);
            }
        }
    }


    //the alien will always be seeking the player Object, if they die and then respawn then the alien will instantly find the new player prefab. find the distance between the alien and the user at any given time and then has the alien move towards the users location, if the alien gets too close then it will go on a flying loopw here it just flies around in a set area. then continue to follow when the user moves away.
    void Update(){
        player = GameObject.Find("Player"); // added this code here too because there was a bug where if the alien spawned as a player is dead then the player is not passed onto the alien script.
        
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;

        if (distance > followDistance) {
            rigidBody.velocity = new Vector3(0,0,0);
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime); 
        }

        if (freeSpace < distance && distance < followDistance) {
            flyAround();
        }

    }


    //method for the alien shooting, find the angle that the player is away from the alien and instantiates a bullet prefab that moves in this direction at a given speed. similar to the player shoot method.
    private void alienShoot(){
        Vector2 playerPosition = player.transform.position;
        Vector2 enemyPosition = this.transform.position;
        Vector2 playerShotAngle = playerPosition - enemyPosition;
        playerShotAngle.Normalize();
        
        alienBullet alienBullet = Instantiate(this.alienBulletPrefab, this.transform.position, this.transform.rotation);
        alienBullet.shotFromAlien = 1;
        alienBullet.alienShoot(playerShotAngle);
    }


    //method that is called when the alien is to just fly around when too close to the user. picks a random movement direction and then flies in this direction at a set speed for a set amount of time, and then it changes the direction and moves again for a set time. as soon as the user is moved far enough away this method wont run and the alien will follow the player again.
    private void flyAround(){
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0){
            randomMovement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            timeLeft += accelerationTime;
            rigidBody.AddForce(randomMovement * maxSpeed);
        }
    }

}
