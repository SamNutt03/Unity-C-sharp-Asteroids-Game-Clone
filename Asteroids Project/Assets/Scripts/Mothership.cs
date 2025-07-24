using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mothership : MonoBehaviour {

    //initialising the sprite renderer and rigidBody for the gameObject.
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    
    //initialise alienBullet Rockets and player prefab object.
    public GameManager gameManage;
    public alienBullet alienBulletPrefab;
    public Rockets missilePrefab;
    public GameObject player;

    //initialise variables that will be changed in code.
    public int mothershipHits = 0;
    public int mothershipBulletLifetime = 2;
    public int shooting = 0;
    public float missileSpeed = 2.0f;
    
    //just a vector that will be used to change the size of the mothership bullet as it uses the same prefab as the alienbullet.
    private Vector3 scaleChange;

    

    private void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.isKinematic = true; //means the object is not effected by forces and is still, only effected by transform functions.
    }

    //when the mothership is first instantiated it will begin shooting after 6 seconds.
    void Start() {
        Invoke(nameof(mothershipShootMissile), 2.25f);
        Invoke(nameof(mothershipShootMissile), 3.75f);
        Invoke(nameof(mothershipShooting), 6.0f);
    }

    //if the mothership is not in the shooting mode then it will follow the users X coordinate, this keeps the mothership at the bottom of the screen but always in line with the user.
    void Update() {
        if (this.shooting != 1){
            Vector2 currentPosition = (Vector2)transform.position;
            Vector2 targetPosition = new Vector2(GameObject.Find("Player").transform.position.x, currentPosition.y);
 
            transform.position = Vector2.MoveTowards(currentPosition, targetPosition, Time.deltaTime * 2.5f);
        }
        
        Rockets[] missiles = FindObjectsOfType<Rockets>();
        for (int i = 0; i < missiles.Length; i++) {
            player = GameObject.Find("Player"); 
            missiles[i].transform.position = Vector2.MoveTowards(missiles[i].transform.position, player.transform.position, missileSpeed * Time.deltaTime);
            Vector2 direction = player.transform.position - missiles[i].transform.position;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;       
            missiles[i].transform.rotation = Quaternion.Euler(Vector3.forward * (angle + 270.0f));
        }
        
    }


    //collision reaction method for the mothership. if a bullet collides then the hits counter is incremented, and passed through to the mothershipHits method in the gameManager. if its the player then they will die, and if the hitCounter is 100 then the mothership dies and the mothershipDead method will run.
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "Bullet"){
            if (this.mothershipHits < 100){
                mothershipHits += 1;
                FindObjectOfType<GameManager>().mothershipHits(mothershipHits);
            }
        }
        if (collision.gameObject.tag == "InvincibleGame"){
            FindObjectOfType<Player>().deathSound.Play(0);
            FindObjectOfType<GameManager>().playerDead();
        }
            
        if (this.mothershipHits == 100) {
            FindObjectOfType<GameManager>().mothershipDied(this);
            Destroy(this.gameObject);
        }
    }
    
    //method to set the motherships active mode to 'shooting' which means it wont move but will instead shoot. invokes the actual shoot method.
    private void mothershipShooting(){
        this.shooting = 1;
        Invoke(nameof(mothershipShoot), 1.25f);
    }
   
    //method to control the motherships shooting mechanic. spawns a alienBullet prefab and scales it to be large enough to fill the screen vertically (as i wanted it to imitate a big laser beam), the bullet will be destroyed after a couple seconds and then the shooting function will be ran again in a few seconds so there is a constant loop of shooting at set intervals.
    private void mothershipShoot(){
        alienBullet mothershipBullet = Instantiate(this.alienBulletPrefab, this.transform.position, this.transform.rotation);
        FindObjectOfType<GameManager>().beamShot.Play();
        scaleChange = new Vector3(0.0f, 300.0f, 0.0f);
        mothershipBullet.rigidBody.isKinematic = true;
        mothershipBullet.transform.localScale += scaleChange;
        Destroy(mothershipBullet.gameObject, this.mothershipBulletLifetime);
        Invoke(nameof(enableMothership), this.mothershipBulletLifetime);
    }

    private void mothershipShootMissile(){
        Rockets missile = Instantiate(this.missilePrefab, this.transform.position + new Vector3(0.0f, 1.0f, 0.0f), this.transform.rotation);
        FindObjectOfType<GameManager>().missileShot.Play();
        StartCoroutine(explodeMissile(missile));
    }

    IEnumerator explodeMissile(Rockets missile)
    {
        yield return new WaitForSeconds(10);
        FindObjectOfType<GameManager>().missilesExplode(missile);
    }

    //method to re-enable the motherships movement when the shooting is done. this means that during the time it is not shooting it is tracking the users position again along the X axis.
    void enableMothership(){
        this.shooting = 0;
        Invoke(nameof(mothershipShootMissile), 2.25f);
        Invoke(nameof(mothershipShootMissile), 3.75f);
        Invoke(nameof(mothershipShooting), 6.0f);
    }
    
}
