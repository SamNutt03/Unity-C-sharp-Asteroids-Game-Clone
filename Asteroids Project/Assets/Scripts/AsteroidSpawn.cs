//This is the code that works the spawning mechanics for the asteroids, created by Sam Nuttall for my asteroids game for COMP222
//this code manages how the asteroids are spawned in within the game.

using UnityEngine;

public class AsteroidSpawn : MonoBehaviour {

    //initialising link to the asteroid prefab and the game manager script.
    public Asteroid asteroidPrefab; 
    public GameManager GameManage;
    
    //initialise the attribute variables with set values.
    public float trajectoryVariance = 15.0f;
    public float spawnRate = 2.0f;
    public float spawnDistance = 10.0f;
    public int spawnAmount = 1;

    
    //when the scene is first started the function spawn is started and runs at regular intervals, this interval being the spawnRate given in the top.
    private void Start(){
        Invoke(nameof(Spawn), this.spawnRate);
    }

    //changes the attribute variable values based on the difficulty level of the game, this makes it harder as the difficulty increases.
    private void Update(){
        if (GameManage.difficultyLevel == 1){
            this.spawnAmount = 1;
            this.spawnRate = 2.0f;
        }
        if (GameManage.difficultyLevel == 2){
            this.spawnAmount = 1;
            this.spawnRate = 1.25f;
        }
        if (GameManage.difficultyLevel == 3){
            this.spawnAmount = 2;
            this.spawnRate = 2.0f;
        }
    }

    //for the given spawn amount, that many asteroids will be instantiated and float into the field at the given angle and from the given spawn point. the direction they fly in at is random but always TOWARDS the field. and the asteroid size is randomised within a set range. speed is also changed based on the size so some arent too fast or slow.
    private void Spawn(){
        for (int i = 0; i < this.spawnAmount; i++){
            Vector3 spawnDirection = Random.insideUnitCircle.normalized * this.spawnDistance;
            Vector3 spawnPoint = this.transform.position + spawnDirection;

            float variance = Random.Range(-this.trajectoryVariance, this.trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Asteroid asteroid = Instantiate(this.asteroidPrefab, spawnPoint, rotation);
            asteroid.size = Random.Range(asteroid.minSize, asteroid.maxSize);
            
            if (asteroid.size > 0.5f) {
                asteroid.speed = 6.0f;
            }

            asteroid.SetTrajectory(rotation * -spawnDirection);
        }
        Invoke(nameof(Spawn), this.spawnRate);
    }

    
}
