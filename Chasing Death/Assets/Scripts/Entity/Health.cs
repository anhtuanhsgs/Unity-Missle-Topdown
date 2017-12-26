using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public int maxHp;
    public int curHp;
    public int[] hpThreshold;
    private int _thresholdIndex = 0;
    public ParticleSystem[] hpStates;
    private bool isDead;

    public ParticleSystem deathParticle;
    public ParticleSystem[] particlesToStop;

    void Awake () {
        deathParticle.Pause (true);

        foreach (ParticleSystem p in hpStates) {
            p.Pause (true);
        }
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (hpThreshold != null && _thresholdIndex < hpThreshold.Length
            && curHp < hpThreshold [_thresholdIndex]) {
            //TODO: animations, state machines, ...
            if (_thresholdIndex < hpStates.Length) {
                hpStates[_thresholdIndex].Play (true);
            }
            _thresholdIndex++;
        }

        if (curHp <= 0 && gameObject.activeInHierarchy) {
            DiePhrase1 ();
        }
	}

    public void GetHit (int damage) {
        curHp -= damage;
    }

    bool IsAllDoneAfterDeath () {
        //Check all particles needed to stop before disable gameobject
        foreach (ParticleSystem p in particlesToStop) {
            if (p.IsAlive(true)) return false;
        }

        if (deathParticle.IsAlive(true)) return false;

        return true;
    }

    void DiePhrase1 () {
        if (!isDead) {
            //Stop motion, render
            gameObject.GetComponent<SpriteRenderer> ().enabled = false;
            gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;

            //Stop particles
            foreach (ParticleSystem p in particlesToStop) {
                p.Stop (true);
            }

            foreach (ParticleSystem p in hpStates) {
                p.Stop (true);
            }

            //Start DeathParticle
            if (!deathParticle.isPlaying && isDead == false) {
                deathParticle.Clear ();
                deathParticle.Play (true);
            }
        }

        isDead = true;  //Not making pharase 1 start again
        
        DiePhrase2 ();
    }

    void DiePhrase2 () {
        if (IsAllDoneAfterDeath ()) {
            gameObject.SetActive (false);
        }
    }
}
