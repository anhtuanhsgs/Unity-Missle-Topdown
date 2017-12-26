using System;
using System.Collections.Generic;
using Assets.Scripts.Steering;
using System.Text;

namespace Assets.Scripts.Entity.Plane {
    class FlockingFighter : Plane{

        void Awake () {

        }

        protected override void Start () {
            base.Start ();

            steeringBehavior.TargetAgent = GameManager.gm.playerMovingAgentCpnt;

            steeringBehavior.AlignmentOn ();
            steeringBehavior.CohesionOn ();
            steeringBehavior.SeperationOn ();
            steeringBehavior.PursuitOn ();

            //TODO: Finsh flocking fighter

            gameObject.tag = "Enemy";
            if (gameObject.tag == "Enemy") {
                GameManager.gm.AddEnemy (this);
            }
        }

    }
}
