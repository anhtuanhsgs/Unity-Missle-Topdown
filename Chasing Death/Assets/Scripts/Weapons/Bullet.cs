using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    GameObject BulletExplodeParticle;

    //References to gameObjects
    Transform _transform;
    Rigidbody2D _rigidbody;
    public int damage;

    // Use this for initialization
    void Awake () {
        _transform = gameObject.GetComponent<Transform> ();

        _rigidbody = gameObject.GetComponent<Rigidbody2D> ();

        if (_rigidbody == null)
            Debug.LogError ("Rigidbody is missing");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D (Collider2D collision) {
        if (collision.tag == "Player") {
            collision.gameObject.GetComponent<Health> ().GetHit (damage);
        }
    }

    public void SetPosition(Vector3 newPosition) {
        _transform.position = newPosition;
    }

    public void SetVelocity (Vector2 velocity) {
        _rigidbody.velocity = velocity;
    }
}
