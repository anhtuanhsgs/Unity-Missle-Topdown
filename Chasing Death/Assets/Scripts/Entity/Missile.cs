using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Missile : MovingAgent {

    public int hitDamage;
    public GameObject explosionObj;

    protected override void Start () {
        base.Start ();
        this.limitSpeed = 10f;
    }

    protected override void Update () {
        base.Update ();
    }

    protected override void UpdateActualSpeed () {

        _rigidbody.AddForce (VectorHeading () * moveForce);

        velocity = _rigidbody.velocity.magnitude;

        //Clamp velocity
        if (velocity > limitSpeed) {
            velocity = limitSpeed;
            _rigidbody.velocity = _rigidbody.velocity.normalized * limitSpeed;
        }
        //TODO: Limit heading angle when having different direction
    }

    void OnTriggerEnter2D (Collider2D collision) {
        
        Health health = gameObject.GetComponent<Health>();

        if (gameObject.tag == "Player" && collision.tag == "Enemy") { //TODO: Expand for enemies' missiles
            //Distroy the missile
            health.GetHit (health.maxHp);

            //Reduce target's hp
            Health targetHealth = collision.GetComponent<Health> ();
            if (targetHealth != null) {
                targetHealth.GetHit (hitDamage);
            }

            //Trigger explosionObj
            if (explosionObj != null) {
                explosionObj.SetActive (true);
            }

            //Only hit one object
            gameObject.GetComponent<BoxCollider2D> ().enabled = false;
        }
    }
}