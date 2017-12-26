using UnityEngine;
using System.Collections;

public class TimedPooledObject : PooledObject {

    public float timeToLive;

    protected override void OnEnable () {
        base.OnEnable ();
        Invoke ("Destroy", timeToLive);
    }

    protected override void Destroy () {
        base.Destroy ();
	}

    protected override void OnDisable () {
        base.OnDisable ();
        CancelInvoke ();
    }
}
