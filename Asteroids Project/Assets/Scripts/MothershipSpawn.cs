using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothershipSpawn : MonoBehaviour {

    //initialise the mothership prefab link.
    public Mothership mothershipPrefab; 

    //initialise the game Manager link.
    public GameManager GameManage;

    //initialise the variables which store the sprite renderer and the rigidbody components of the prefab.
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    
    //when the mothership spawn script is first accessed the components are fetched and stored in the variables that are declared above.
    private void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }
    
    void Start(){
    }

    private void Update(){
    }

    //the spawn method for the mothership. picks a random point on the X axis of the screen and instantiates a new mothership prefab, it will be at a set Y axis height so that its at the bottom of the screen.
    public void Spawn(){
        float spawnY = Camera.main.ScreenToWorldPoint(new Vector2(0, 90)).y;
        float spawnX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        Mothership mothership = Instantiate(this.mothershipPrefab, spawnPosition, Quaternion.identity);
    }
}
