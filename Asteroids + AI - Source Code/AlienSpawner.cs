using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienSpawner : MonoBehaviour {
    
    //initialise the alienPrefab from the alienAI script
    public AlienAI alienPrefab; 

    //initialise the gameManager link.
    public GameManager GameManage;

    //just one audiosource for when the aliens spawn
    public AudioSource alienSpawn;

    
    
    private void Start(){}

    private void Update(){}

    //the spawn method for the aliens, picks a random Y coordinate and a random X coordinate out of the full size of the screen width and height and then set this as the spawn position, then an alien prefab is instantiated at this position and the spawn SFX is played.
    public void Spawn(){
        float spawnY = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float spawnX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
 
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        AlienAI alien = Instantiate(this.alienPrefab, spawnPosition, Quaternion.identity);

        alienSpawn.Play(0);
    }
}
