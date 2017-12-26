using System;
using Assets.Scripts.Steering;

namespace Assets.Scripts.Entity.Plane {
    class Plane : MovingAgent{

        protected override void Update () {

            SteeringOut steeringOut = steeringBehavior.CalculateDithered ();

            if (!steeringOut.IsOriginal ()) {
                this.limitSpeed = steeringOut.LimitSpeed;
                this.targetAngle = steeringOut.TargetAngle;
            }
            base.Update ();
        }

    }    
}
