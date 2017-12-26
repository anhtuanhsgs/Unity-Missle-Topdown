using UnityEngine;
using System.Collections;
using Assets.Scripts.Common.Misc;

public class BurstGun : Gun {    

    public int shotsPerBurst = 3;
    public const float delayPerBurst = 1f;
    public const float delayPerShot = 0.2f;
    float curBurstDelay = 0f;
    float curShotDelay = 0f;
    int shotsFired = 0;
    public float bulletVelocity;

    public GameObject target;

    public float shotableDeltaAngle;
    public float shotableDistantSqr;

    Transform _transform;

	// Use this for initialization
	protected override void Start () {
        base.Start ();
        _transform = gameObject.GetComponent<Transform> ();
        bulletPooler = BasicBulletPooler.instance;
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update ();
        if (!firing) Aim ();
	}

    //Aim then fire
    private void Aim () {
        Vector2 toTarget = (Vector2)target.transform.position - _owner.Position;

        if (toTarget.sqrMagnitude > shotableDistantSqr) {
            return;
        }

        float targetAngle = Utils.Vector2Angle (toTarget);
        float absDeltaAngle = Utils.AbsDeltaAngle (_owner.DegreeHeading (), targetAngle);

        if (absDeltaAngle <= shotableDeltaAngle) {
            Fire ();
        }
    }

    //Burst fire
    public override void Fire () {

        if (curBurstDelay <= 0) {
            firing = true;
            _owner.Firing = true;
            curBurstDelay = delayPerBurst;
        }
    }

    protected override void UpdateFire () {
        
        // n-round burst
        if (firing == false) {
            if (curBurstDelay > 0)
                curBurstDelay -= Time.deltaTime;
            return;
        }
        curShotDelay -= Time.deltaTime;

        //Update between shot in a burst
        if (curShotDelay <= 0) {
            curShotDelay = delayPerShot;
            GetBulletFromPooler ();
            shotsFired++;
        }

        if (shotsFired == shotsPerBurst) {
            shotsFired = 0;
            StopFire ();
        }           
    }
    

    void GetBulletFromPooler () {
        GameObject bulletObj = bulletPooler.GetPooledObj ();
        Bullet bulletInstance = bulletObj.GetComponent<Bullet> ();
        bulletInstance.SetPosition (_transform.position);
        bulletInstance.SetVelocity (_owner.VectorHeading () * bulletVelocity);
    }
}
