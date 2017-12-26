using UnityEngine;
using System.Collections;
using Assets.Scripts.Steering;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

    public GameObject player;
    [HideInInspector]
    public MovingAgent playerMovingAgentCpnt;
    [HideInInspector]
    public List<MovingAgent> enemyList;

    public float boundaryRadius;

    void Awake () {

        gm = this;

        if (player == null) {
            player = GameObject.FindGameObjectWithTag ("Player");
            if (player == null)
                Debug.LogError ("Missing player gameObj");
        }

        playerMovingAgentCpnt = player.GetComponent<MovingAgent> ();
        if (playerMovingAgentCpnt == null) {
            Debug.LogError ("Missing player moving Agent cpnt");
        }

        enemyList = new List<MovingAgent> ();
    }

	// Use this for initialization
	void Start () {
	    
    }
	
	// Update is called once per frame
	void Update () {
        UpdateEnemiesIndicator ();
    }

    void UpdateEnemiesIndicator () {

    }

    public void AddEnemy (MovingAgent enemyAgent) {
        enemyList.Add (enemyAgent);
    }

    public void RemoveEnemy (MovingAgent enemyAgent) {
        //TODO: Add this function when implement destroy planes, etc
        enemyList.Remove (enemyAgent);
    }
}
