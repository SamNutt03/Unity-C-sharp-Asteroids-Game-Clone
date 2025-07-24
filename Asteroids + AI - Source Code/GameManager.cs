//Sam Nuttall Asteroids
//the game manager is what is in charge of the overall state of the game, including score/lives etc.
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    //initialising particle explosions.
    public ParticleSystem explosion;
    public ParticleSystem explosion2;

    //initialising prefabs and spawners scripts
    public MothershipSpawn MothershipSpawn;
    public AlienSpawner alienSpawner;
    public Player player;
    public AlienAI AlienAI;
    public MothershipSpawn Mothership;
    public Rockets missiles;
    
    

    //the following lines are used for boolean conditionals to run functions. they are changed to 1 and 0 throughout code.
    public int swarmMode = 0;
    public int aliensComing = 0;
    public int alienAlerted = 0;
    public int aliensKilled = 0;
    public int mothershipGame = 0;
    private int increased2;
    private int increased3;
    public float respawnTime = 3.0f;
    public float respawnInvincibilityTime = 3.0f;

    //the following are all UI overlays, mainly text objects which are changed throughout code / displayed and hidden.
    public GameObject gameOverUI;
    public GameObject diffIncreaseUI;
    public GameObject alienAlertUI;
    public GameObject mothershipAlertUI;
    public GameObject mothershipHealth;
    public GameObject aliensFoundYouUI;
    public GameObject mothershipWonUI;

    //these are getter and setter methods for values and text objects which can be changed and displayed on the game.
    public int mothershipHealthValue { get; private set; }
    public Text mothershipHealthText;
    public int score { get; private set; }
    public Text scoreText;
    public int lives { get; private set; }
    public Text livesText;

    //initialising highscore which is a persistantly stored/fetched on every run variable.
    public int highScore;
    public Text highScoreText;

    //another conditional variable which will change depending on the difficulty level. depending on the value of this different methods will run.
    public int difficultyLevel;
    public Text difficultyText;

    //initialising audio sources used in the game, each linked on the unity project panel.
    public AudioSource difficultySound;
    public AudioSource gameOverSound;
    public AudioSource alienHit;
    public AudioSource alienDeath;
    public AudioSource mothershipSpawning;
    public AudioSource aliensFound;
    public AudioSource mothershipExploding;
    public AudioSource deathSound;
    public AudioSource beeping;
    public AudioSource missileShot;
    public AudioSource beamShot;
    



    //start function, when the game first starts the highscore is fetched and displayed and the difficulty is set to 1 initially. then the newGame function is ran in order to start the game
    private void Start() {
        this.highScore = PlayerPrefs.GetInt("hs");
        highScoreText.text = this.highScore.ToString();
        this.difficultyLevel = 1;
        difficultyText.text = this.difficultyLevel.ToString();
        NewGame();
    }





    private void Update(){
//IF THE USER IS DEAD AND THEY PRESS RETURN THEN A NEW GAME WILL BEGIN.
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
//IF THE USER PRESSES 'H' KEY THEN THE PERSISTANT STORAGE OF THE HIGH SCORE WILL BE RESET BACK TO 0 AND THE SCREEN UI WILL UPDATE TO SHOW THIS.
        if (Input.GetKey(KeyCode.H)){
            PlayerPrefs.SetInt("hs", 0);
            this.highScore = PlayerPrefs.GetInt("hs");
            highScoreText.text = highScore.ToString();
        }
//IF SCORE IS OVER 49(50+) THEN THE INITIAL ALIENS WILL START TO SPAWN IN, AUDIO EFFECTS AND UI ELEMENTS ARE PLAYED AND SHOWN TO THE USER.
        if (this.score >= 49) {
            if (aliensComing == 0){
                aliensComing = 1;
                aliensFound.Play();
                Invoke(nameof(aliensFoundOn), 0);
                Invoke(nameof(aliensFoundOff), 3);
                Invoke(nameof(spawnAlien), 5);
            }
        }
//IF SCORE IS OVER 149(150+) THEN THE SWARM MODE WILL ACTIVATE, SPAWNING 4 ALIENS AT A TIME INSTEAD OF 1. FIRSTLY ALL ALIENS ARE REMOVED FROM THE GAME IN ORDER TO MAKE SPACE FOR NEW ONES.
        if (this.score > 149 && this.alienAlerted != 1) {
            CancelInvoke();
            AlienAI[] aliens = FindObjectsOfType<AlienAI>();
            for (int i = 0; i < aliens.Length; i++) {
                Destroy(aliens[i].gameObject);
            }
            swarmMode = 1;
            this.alienAlerted = 1;
            Invoke(nameof(alienAlertOn), 0);
            Invoke(nameof(alienAlertOff), 3);
            Invoke(nameof(spawnAlien), 13);
            Invoke(nameof(spawnAlien), 13);
            Invoke(nameof(spawnAlien), 13);
            Invoke(nameof(spawnAlien), 13);
        }

//IF SCORE IS OVER 199 (200+) THEN THE ASTEROIDS WILL SPAWN AT DIFFICULTY LEVEL 2 (more spawn at once, and they spawn more frequently.) A sound effect plays and a UI pop up lets the user know their level up status.
        if (this.score > 199 && this.increased2 != 1){
            this.increased2 = 1;
            this.difficultyLevel = 2;
            difficultyText.text = this.difficultyLevel.ToString();
            difficultySound.Play(0);
            Invoke(nameof(diffIncOn), 0);
            Invoke(nameof(diffIncOff), 2);
        }

//IF SCORE IS OVER 299(300+) THEN THE MOTHERSHIP MINIGAME WILL START.
        if (this.score > 299 && this.mothershipGame == 0){
            CancelInvoke();
            mothershipGame = 1;
            aliensKilled = 0;
            Invoke(nameof(CollisionsOffGame), 0);
            Invoke(nameof(mothershipGameOn), 0);
        }
        //When mothership game is active all aliens and asteroids are killed and they stop spawning until the minigame is done.
        if (mothershipGame == 1){
            AlienAI[] aliens = FindObjectsOfType<AlienAI>();
            for (int i = 0; i < aliens.Length; i++) {
                Destroy(aliens[i].gameObject);
            }
            Asteroid[] asteroids = FindObjectsOfType<Asteroid>();
            for (int i = 0; i < asteroids.Length; i++) {
                Destroy(asteroids[i].gameObject);
            }
            
        }

//IF THE SCORE IS OVER 749(750+) THEN THE ASTEROID SPAWN IN AT DIFFICULTY LEVEL 3, WHICH IS THE HARDEST IN MY IMPLEMENTATION. again a sound effect and the UI pop ups let the user know their level.
        if (this.score > 749 && this.increased3 != 1){
            this.increased3 = 1;
            this.difficultyLevel = 3;
            difficultyText.text = this.difficultyLevel.ToString();
            difficultySound.Play(0);
            Invoke(nameof(diffIncOn), 0);
            Invoke(nameof(diffIncOff), 2);
        }      
    }

    


    //method for setting the score of the user, this is constantly updated and displayed to the user, it also changes the high score (and persistant storage of it) if this is reached.
    public void SetScore(int score){
        this.score = score;
        scoreText.text = score.ToString();
        
        if (this.score > this.highScore){
            PlayerPrefs.SetInt("hs", this.score);
        }

        this.highScore = PlayerPrefs.GetInt("hs");
        highScoreText.text = highScore.ToString();
    }

    //method to set the lives of the user on the UI which displays it. this is constantly updated whenever there is a change.
    private void SetLives(int lives){
        this.lives = lives;
        livesText.text = lives.ToString();
    }





//START OF MOTHERSHIP RELATED METHODS.
    //mothership UI alerts and sounds are played and the spawning methods are invoked.
    private void mothershipGameOn(){
        alienBullet[] alienBullets = FindObjectsOfType<alienBullet>();
        for (int i = 0; i < alienBullets.Length; i++) {
            Destroy(alienBullets[i].gameObject);
        }
        mothershipSpawning.Play();
        SetMothershipHealth(100);
        Invoke(nameof(mothershipAlertOn), 0);
        Invoke(nameof(mothershipAlertOff), 5);
        Invoke(nameof(spawnMothership), 6);
    }
    //spawning method for the mothership, runs a method that instantiates it with certain variables.
    private void spawnMothership(){ MothershipSpawn.Spawn(); }

    //changes the motherships health as the user damages it. and displays this to the user in the UI.
    private void SetMothershipHealth(int mothershipHealthValue){
        this.mothershipHealthValue = mothershipHealthValue;
        mothershipHealthText.text = mothershipHealthValue.ToString();
    }

    //each time the mothership is hit, the setMothershipHealth method is ran with the input of 100-mothershipHits, which is the remaining life of it.
    public void mothershipHits(int mothershipHits){ SetMothershipHealth(100-mothershipHits); }

    //method that runs when the mothership dies, it plays the custom explosion particle effects on the motherships location and plays the death sound. the game settings are reverted back to normal play, and the score is updated. some alien spawns are invoked so that the game can resume
    public void mothershipDied(Mothership Mothership){
        explosion2.transform.position = Mothership.transform.position;
        this.mothershipExploding.Play();
        explosion2.Play();

        this.mothershipGame = 2;
        this.swarmMode = 1;
        this.aliensKilled = 0;

        Invoke(nameof(mothershipWonOn), 0);
        Invoke(nameof(mothershipWonOff), 3);

        Invoke(nameof(CollisionsOn), 3);

        SetScore((score + 100));

        mothershipHealth.SetActive(false);
        mothershipHealthText.text = " ";

        Invoke(nameof(spawnAlien), 10);
        Invoke(nameof(spawnAlien), 10);
        Invoke(nameof(spawnAlien), 10);
        Invoke(nameof(spawnAlien), 10);
    }

    public void missilesExplode(Rockets missiles){
        explosion.transform.position = missiles.transform.position;
        explosion.Play();
        deathSound.Play();
        Destroy(missiles.gameObject);
    }
//END OF MOTHERSHIP RELATED CODE




    //method runs when an asteroid is shot by the user. plays an explosion particle effect on the asteroids location, and adds score depending on the size of the asteroid.
    public void asteroidDead(Asteroid asteroid){
        this.explosion.transform.position = asteroid.transform.position;
        this.explosion.Play();
        //Scoring
        //smallest asteroids = 3 points as hard to hit,
        //mid asteroids = 2 points
        //large asteroids = 1 point
        if (asteroid.size < 0.45f) { 
            SetScore((score + 3));
        }else if (asteroid.size < 0.65f && asteroid.size >= 0.45f){ 
            SetScore((score + 2));
        }else{
            SetScore((score + 1));
        }
    }

    //method runs when an alien is killed by the user, it changes the tally of aliens killed and plays an explosion effect on the aliens location, aswell as death SFX. score is changed and more aliens are spawned, depending whether its swarm mode or not depends on the new amount of alienSpawns invoked.
    public void alienDead(AlienAI alien){
        this.aliensKilled += 1;
        this.explosion.transform.position = alien.transform.position;
        this.explosion.Play();
        this.alienDeath.Play();
        this.deathSound.Play(0);
         SetScore((score + 25));

        if (swarmMode == 0 && aliensKilled > 0 && aliensKilled < 4 && mothershipGame != 1) {
            aliensKilled = 0;
            Invoke(nameof(spawnAlien), 5);
        }
        if (swarmMode == 1 && aliensKilled >= 4 && mothershipGame != 1) {
            aliensKilled = 0;
            Invoke(nameof(spawnAlien), 10);
            Invoke(nameof(spawnAlien), 10);
            Invoke(nameof(spawnAlien), 10);
            Invoke(nameof(spawnAlien), 10);
        }       
    }

    //method runs when the player dies, the death sound and explosion are played on the players death location and all the bullets are deleted in the world if the lives count is 0. lives are decremented down when the player dies, they initially have 3.
    public void playerDead(){
        this.explosion.transform.position = this.player.transform.position;
        this.explosion.Play();

        SetLives(lives - 1);

        //if the players lives reach 0 then all the bullets are removed and invokes are cancelled. the game over screen is then displayed. otherwise, if they have lives left then the respawn method is ran.
        if (this.lives <= 0){
            Bullet[] bullets = FindObjectsOfType<Bullet>();
            for (int i = 0; i < bullets.Length; i++) {
                Destroy(bullets[i].gameObject);
            }
            mothershipHealth.SetActive(false);
            mothershipHealthText.text = " ";
            CancelInvoke();
            GameOver();
        }else{
            Invoke(nameof(Respawn), this.respawnTime);
        }

        Rockets[] rockets = FindObjectsOfType<Rockets>();
        for (int i = 0; i < rockets.Length; i++) {
            Destroy(rockets[i].gameObject);
        }
    }




    //respawn method which is ran if the user dies and has lives remaining. moves player back the center of screen and gives a grace period where collisions are off.
    private void Respawn(){
        this.player.transform.position = Vector3.zero;
        Invoke(nameof(CollisionsOff), 0);
        this.player.gameObject.SetActive(true);
        Invoke(nameof(CollisionsOn), this.respawnInvincibilityTime);
    }
    //gameOver method runs if the player dies with no lives remaining. turns all UI alerts off and stops all invoked methods from running. gameOver SFX are played and all settings are reverted back to default. if the user wants they can press Return as instructed to start a new game.
    private void GameOver(){
        mothershipAlertOff();
        alienAlertOff();
        diffIncOff();
        CancelInvoke();
        
        gameOverSound.Play(0);
        gameOverUI.SetActive(true);
        swarmMode = 0;
        aliensKilled = 0;
    }
    //newGame method can be ran after the gameOver if the user wants to play again, destroys all asteroids, aliens and motherships that may still be in the field. resets lives and score and the difficulty level and then reinitialises the game and respawns the user with the Respawn() method.
    public void NewGame(){
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();
        AlienAI[] aliens = FindObjectsOfType<AlienAI>();
        Mothership[] theMothership = FindObjectsOfType<Mothership>();
        for (int i = 0; i < asteroids.Length; i++) { Destroy(asteroids[i].gameObject); }
        for (int i = 0; i < aliens.Length; i++) { Destroy(aliens[i].gameObject); }
        for (int i = 0; i < theMothership.Length; i++) { Destroy(theMothership[i].gameObject); }

        gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);

        this.increased2 = 0;
        this.increased3 = 0;
        this.difficultyLevel = 1;
        difficultyText.text = "1";
        alienAlerted = 0;
        mothershipGame = 0;
        aliensComing = 0;
        
        Respawn();
    }





    public void alienHitSound(){ alienHit.Play(0); } //method just to play the alien hit sound effect
    public void spawnAlien(){ alienSpawner.Spawn(); } //method to spawn aliens from the spawner script.

    private void CollisionsOff(){ this.player.gameObject.layer = LayerMask.NameToLayer("Invincible"); } //changes the players layer to invincible with no collisions
    private void CollisionsOffGame(){ this.player.gameObject.layer = LayerMask.NameToLayer("InvincibleGame"); }//changes the players layer to invincible with no collisions but this is modified so it can collide with mothership and bullets.
    private void CollisionsOn(){ this.player.gameObject.layer = LayerMask.NameToLayer("Player"); }//changes layer mask back to player so that all collisions are activated again.

    private void diffIncOn(){ diffIncreaseUI.SetActive(true); } //turns UI for difficulty on
    private void diffIncOff(){ diffIncreaseUI.SetActive(false); } //turns UI for difficulty off

    private void aliensFoundOn(){ aliensFoundYouUI.SetActive(true); } //turns UI for aliens finding you on
    private void aliensFoundOff(){ aliensFoundYouUI.SetActive(false); } //turns UI for aliens finding you off

    private void alienAlertOn(){ beeping.Play(); alienAlertUI.SetActive(true); } //turns UI for swarm mode activating on
    private void alienAlertOff(){ alienAlertUI.SetActive(false); } //turns the swarm mode activation UI off

    private void mothershipAlertOn(){ mothershipAlertUI.SetActive(true); mothershipHealth.SetActive(true);} //turns the mothership mission starting UI alert on
    private void mothershipAlertOff(){ mothershipAlertUI.SetActive(false); } //turns the mothership mission starting UI alert off

    private void mothershipWonOn(){ mothershipWonUI.SetActive(true); } //turns the UI to show that you beat the mothership on
    private void mothershipWonOff(){ mothershipWonUI.SetActive(false); } //turns the UI to show that you beat the mothership off
   
}
