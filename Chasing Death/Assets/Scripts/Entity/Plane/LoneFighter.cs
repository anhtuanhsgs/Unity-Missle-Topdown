using System;
using System.Collections.Generic;
using Assets.Scripts.Steering;
using UnityEngine;

namespace Assets.Scripts.Entity.Plane {
    class LoneFighter : Plane {

        void Awake () {
            //TODO: Update alternate behavior, gm adding way accoording to TAG

        }

        protected override void Start () {
            base.Start ();
            
            steeringBehavior.TargetAgent = GameManager.gm.playerMovingAgentCpnt;
            
            steeringBehavior.PursuitOn ();

            GetParameter ();

            gameObject.tag = "Enemy";
            if (gameObject.tag == "Enemy") {
                GameManager.gm.AddEnemy (this);
            }
        }

        private void GetParameter () {
            //TODO: GetParameter update
        }
    }
}
