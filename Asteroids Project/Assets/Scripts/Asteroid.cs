//Code for asteroids, created by Sam Nuttall for my Asteroids game.
//This C# Scripting code effects the asteroids sprites which is the boulders that go for you in game.

using UnityEngine;

public class Asteroid : MonoBehaviour {  

    //initialise the list of sprite images that the asteroid can pick from when randomly generating. there is 4 different images that the asteroid could possibly be.
    public Sprite[] sprites;
    public Asteroid asteroidPrefab;
    
    //initialises these variable attributes for the asteroid prefab to take.
    public float size = 1.0f;
    public float minSize = 0.3f;
    public float maxSize = 0.8f;
    public float speed = 3.0f;
    public float maxTime = 15.0f;
    public float respawnInvincibilityTime = 5.0f;
    public bool spawnedIn = false;

    //initialise the spriterendering and rigidbody for the prefabs.
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    

    //fetch the components for sprite renderer and rigidbody.
    private void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    //when the asteroid spawns it picks a random sprite from the list of sprites to display as. it picks a rotation angle to fly into the screen at, and its mass is determined based on the size, this effects its speed. for a set time the asteroids are invincible so they can pass through the boundary layer, they are then enabled collisions so they can bounce around inside. smaller asteroids are invincible for less time as they get into the game area fastest.
    private void Start(){
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, Random.value * 360.0f); //visual rotation of the random asteroid sprite picked from the array of sprites.
        this.transform.localScale = Vector3.one * this.size;

        rigidBody.mass = this.size; 

        if (this.spawnedIn == false) {
            this.gameObject.layer = LayerMask.NameToLayer("InvincibleAst");
            
            if (this.size < 0.4f) { //gives smaller asteroids less invincibilty time as they travel onto the game field faster, so we dont want them to fly straight through.
                Invoke(nameof(CollisionsOn), (this.respawnInvincibilityTime / 1.5f));
            }else{
                Invoke(nameof(CollisionsOn), this.respawnInvincibilityTime);
            }
            this.spawnedIn = true;
        }
    }


    //method that changes the asteroid layer so that collisions are enabled
    private void CollisionsOn(){
        rigidBody.gameObject.layer = LayerMask.NameToLayer("Asteroid");
    }


    //adds force in the randomly picked direction from the other method and then this makes the asteroid move in that direction, the asteroid is the deleted after a maximum lifetime in the scene.
    public void SetTrajectory(Vector2 direction){
        rigidBody.AddForce(direction * this.speed);
        Destroy(this.gameObject, this.maxTime);
    }
   

    //collision reaction methods for the asteroid, if it collides with a bullet then the asteroid is either destroyed or it creates a split , which means that there is a random number of smaller asteroids generated out of the one destroyed one, 2-6 asteroids can be created in the split, this is randomly generated.
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "Bullet"){
            if ((this.size * 0.525f) >= this.minSize){
                for (int i = 0; i < Random.Range(2,4); i++) 
                    createSplit();
            }

            FindObjectOfType<GameManager>().asteroidDead(this); //the asteroidDead function plays the explosion sound and removes the original asteroid.
            Destroy(this.gameObject);
        }
    }


    //method for creating the smaller asteroid splits, this picks a position in a circle around the destroyed asteroids location and then instantiates a new asteroid in this area with a new random direction and speed given and a new lifetime, there is also a grace period of no collisions so they dont instantly collide with each other.
    private void createSplit(){
        Vector2 position = this.transform.position;
        position = position + (Random.insideUnitCircle * 1.25f);
        Asteroid half = Instantiate(this.asteroidPrefab, position, this.transform.rotation);
        half.gameObject.layer = LayerMask.NameToLayer("Asteroid");
        half.spawnedIn = true;
        half.size = this.size * 0.66f;
        half.SetTrajectory(Random.insideUnitCircle.normalized * this.speed);
        
    }

}
