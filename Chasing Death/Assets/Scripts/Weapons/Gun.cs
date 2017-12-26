using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    protected MovingAgent _owner;

    public GameObject gunExplosionParticle;
    protected Pooler bulletPooler;
    protected bool firing;

	// Use this for initialization
	protected virtual void Start () {
        _owner = gameObject.transform.parent.GetComponent<MovingAgent> ();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	    if (_owner.Firing) {
            Fire ();
        }
        UpdateFire ();
    }

    public virtual void Fire () {
        
    }

    public virtual void StopFire () {
        firing = false;
        _owner.Firing = false;
    }

    protected virtual void UpdateFire () {
    }
}
