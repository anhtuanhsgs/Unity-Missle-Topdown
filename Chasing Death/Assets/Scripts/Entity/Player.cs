using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    public GameObject missileObj;
    Missile missile;

	// Use this for initialization
	void Start () {
        if (missileObj == null) Debug.LogError ("Missing missileObj");

        missile = missileObj.GetComponent<Missile> ();
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput ();
	}

    void HandleInput () {
        //Keyboard input
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            float horizontal = CrossPlatformInputManager.GetAxis ("Horizontal");
            missile.SetTargetAngle (missile.DegreeHeading () - horizontal * 90);
        }
        //Screen touch input
        else { 
                    
            byte horizontal = 0;

            if (CrossPlatformInputManager.GetButtonDown ("Left")) {
                horizontal += 1;
            }

            if (CrossPlatformInputManager.GetButtonDown ("Right")) {
                horizontal += 2;
            }

            switch (horizontal) {
                case 1: //left
                    missile.SetTargetAngle (missile.DegreeHeading () + 90);
                    break;
                case 2: //right
                    missile.SetTargetAngle (missile.DegreeHeading () - 90);
                    break;
                case 3: //both
                    //TODO: Implement some fancy spectial funcs here
                    break;
            }
        }
        
    }
}
