using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Common.Misc;

public class TargetIndicator : MonoBehaviour {

    public Canvas mainGUI;
    public float indicatorOffset;
    public Camera mainCamera;
    GameManager gameManager;
    RectTransform mainGUIRectTransf;

    public Image indicatorPrefab;
    public int indicatorNum;
    LinkedList<Image> indicatorLList;

    // Use this for initialization
    void Awake () {
        if (mainGUI == null) {
            Debug.Log ("Missing GUI canvas");
        }

        if (mainCamera == null) {
            Debug.Log ("Missing main camera");
        }

        if (indicatorPrefab == null) {
            Debug.Log ("Missing main indicator prefab");
        }

        indicatorLList = new LinkedList<Image> ();
        for (int i = 0; i < indicatorNum; i++) {
            InitiateIndicator ();
        }

        mainGUIRectTransf = mainGUI.GetComponent<RectTransform> ();

        gameManager = GameManager.gm;
    }

	// Update is called once per frame
	void Update () {
        UpdateEnemyIndicators ();
	}


    //Dequeue for indicator pooling
    void UpdateEnemyIndicators () {
        //TODO: update when an enemy destroyed
        LinkedListNode<Image> it = indicatorLList.First;
        for (int i = 0; i < gameManager.enemyList.Count; i++) {
            MovingAgent agent = gameManager.enemyList[i];

            //Create new when all available indicators are used
            if (it == null) {
                InitiateIndicator ();
                it = indicatorLList.Last;
            }


            if (!agent.IsVisible()) {
                Image indicatorImg = it.Value;
                indicatorImg.enabled = true;
                UpdateIndicator (agent, indicatorImg);
                it = it.Next;
            }

            if (i == gameManager.enemyList.Count - 1) {
                while(it != null && it.Value.enabled == true) {
                    it.Value.enabled = false;
                    it = it.Next;
                }
            } 
        }
        //TODO: update indicators: Update on died enemy, OnVisible, OnInvisible of enemy
    }

    void InitiateIndicator () {
        Image img = Instantiate (indicatorPrefab);
        img.transform.SetParent (gameObject.transform, false);
        img.enabled = false;
        indicatorLList.AddLast (img);
    }

    void UpdateIndicator (MovingAgent agent, Image indicatorImg) {
        Vector2 agent2Camera = agent.Position - (Vector2) mainCamera.transform.position;

        /*Position update*/
        //y = mx + 0
        float m = Mathf.Abs (agent2Camera.y / agent2Camera.x);
        float halfW = mainGUIRectTransf.rect.width / 2 - indicatorOffset;
        float halfH = mainGUIRectTransf.rect.height / 2 - indicatorOffset;

        //after replace x = halfW, y = halfH
        float X = halfH / m; //y = halfH
        float Y = halfW * m; //x = halfW

        Vector2 indicatorPos = new Vector2(halfW, halfH);

        if (Mathf.Abs (X) > halfW) {
            indicatorPos.y = Y;
        }
        else if (Mathf.Abs (Y) > halfH) { 
            indicatorPos.x = X;
        }

        if (agent2Camera.x < 0) indicatorPos.x *= -1;
        if (agent2Camera.y < 0) indicatorPos.y *= -1;
        indicatorImg.rectTransform.anchoredPosition = indicatorPos;
        /*End of position update*/

        /*Angle update*/
        float angle = Utils.Vector2Angle (agent2Camera);
        indicatorImg.rectTransform.eulerAngles = new Vector3 (0, 0, angle);
        /*End of angle update*/
    }
}
