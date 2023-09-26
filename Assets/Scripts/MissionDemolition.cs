using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode { 
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition S; // a private Singleton 

    [Header("Inscribed")]
    public Text uitLevel; // The UIText_Level Text
    public Text uitShots; // The UIText_Shots Text
    public Text uitHighScore; // The UIText_HighScore Text
    public Vector3 castlePos; // The place to put castles
    public GameObject[] castles; // An array of the castles
    public GameObject[] GOUI;  //Array of GOUI objects
    public int score; //score holder
    private int defScore = 35; //Default score
    public bool resetHighScoreNow = false; //for easy reset of high score

    [Header("Dynamic")]
    public int level; // The current level
    public int levelMax; // The number of levels
    public int shotsTaken;
    public GameObject castle; // The current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot"; // FollowCam mode

    void Start() {
        S = this; // Define the Singleton 

        GOUI = GameObject.FindGameObjectsWithTag("GOUI");
        foreach (GameObject G in GOUI){
            G.SetActive(false);
        }

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        setHighScore();
        StartLevel();
    }

    void StartLevel() {
        // Get rid of the old castle if one exists
        if (castle != null) {
            Destroy(castle);
        }

        // Destroy old projectiles if they exist (the method is not yet written)
        Projectile.DESTROY_PROJECTILES(); // This will be underlined in red 

        // Instantiate the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        // Reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        // Zoom out to show both         
        FollowCam.SWITCH_VIEW( FollowCam.eView.both );
    }

    void UpdateGUI() {
        // Show the data in the GUITexts
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }
        
    

    void Update() {
        UpdateGUI();

        // Check for level end
        if ((mode == GameMode.playing) && Goal.goalMet) {
            // Change mode to stop checking for level end
            mode = GameMode.levelEnd;
            // Zoom out to show both             
            FollowCam.SWITCH_VIEW( FollowCam.eView.both );
            // Start the next level in 2 seconds
            Invoke("NextLevel", 2f); 
        }
    }

    void NextLevel() {
        level++;
        if (level == levelMax) {
            if (shotsTaken < PlayerPrefs.GetInt("HighScore")) {
                PlayerPrefs.SetInt("HighScore", shotsTaken);
                Debug.Log("Setting new high score: " + PlayerPrefs.GetInt("HighScore"));
            }
            foreach (GameObject G in GOUI){
                G.SetActive(true);
            }   
        }else{
        StartLevel();
        }
    }

    public void RestartGame(){
        level = 0;
        shotsTaken = 0;
        foreach (GameObject G in GOUI){
            G.SetActive(false);
        }
        setHighScore();
        StartLevel();
    }

    public void EndGame(){
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void setHighScore(){
        if (PlayerPrefs.HasKey("HighScore") && PlayerPrefs.GetInt("HighScore") < defScore && resetHighScoreNow == false) { //if HighScore exists and is smaller than the default score
            score = PlayerPrefs.GetInt("HighScore"); //set score to high score
        }else{
            score = defScore; //set score to default score
            PlayerPrefs.SetInt("HighScore", defScore); //make sure high score is set to the default (the highest it should ever be)
        }
        uitHighScore.text = "High Score: " + score; //write high score to the UI
    }

    // Static method that allows code anywhere to increment shotsTaken
    static public void SHOT_FIRED() { 
        S.shotsTaken++;
    }

    // Static method that allows code anywhere to get a reference to S.castle
    static public GameObject GET_CASTLE() { 
        return S.castle;
    }

    void OnDrawGizmos() { //button to reset the high score
        if (resetHighScoreNow) {
           setHighScore();
           resetHighScoreNow = false;
        }
    }
    
}
