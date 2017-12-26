using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    float radius = -1;
    public float duration;
    public int damage;
    public float forceScale;
    LayerMask affectedLayer;


	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (duration <= 0) {
            gameObject.SetActive (false);
        }

        duration -= Time.deltaTime;
    }

    void OnTriggerEnter2D (Collider2D col) {
        Vector2 targetPos = col.transform.position;
        Vector2 explosionPos = gameObject.transform.position;

        //Get radius of explosion
        if (radius < 0) {
            CircleCollider2D circleCollider2DCpn = GetComponent<CircleCollider2D> ();
            if (circleCollider2DCpn != null) {
                radius = circleCollider2DCpn.radius;
            }
        }

        //Apply push force
        Vector2 toTarget = targetPos - explosionPos;
        float pushForce = radius - toTarget.magnitude;
        col.GetComponent<Rigidbody2D> ().AddForce (toTarget.normalized * pushForce * forceScale);

        //Apply damage
        col.gameObject.GetComponent<Health> ().GetHit (damage);
    }
}
