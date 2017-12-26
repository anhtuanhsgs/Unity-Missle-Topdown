using UnityEngine;
using System.Collections;

public class RotateToParent : MonoBehaviour {

    Transform _parentTransform;

    ParticleSystem _particleSystem;
    Vector4 _particleRotation;


	// Use this for initialization
	void Start () {

        _parentTransform = gameObject.transform.parent.GetComponent<Transform> ();
        if (_parentTransform == null) {
            Debug.Log ("Missing parent Transform");
        }

        _particleSystem = gameObject.GetComponent<ParticleSystem> ();
        if (_particleSystem == null) {
            Debug.Log ("Missing particle system");
        }
    }
	
	// Update is called once per frame
	void Update () {
        _particleSystem.startRotation = Mathf.Deg2Rad * _parentTransform.rotation.eulerAngles.z;
    }
}
