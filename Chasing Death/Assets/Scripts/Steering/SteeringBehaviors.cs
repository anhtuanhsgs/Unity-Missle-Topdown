using System;
using UnityEngine;
using Assets.Scripts.Common.Misc;
using System.Collections.Generic;


namespace Assets.Scripts.Steering {
    public class SteeringOut {
        public float _limitSpeed;
        public float _targetAngle;
        private bool _original;

        public SteeringOut () {
            _limitSpeed = 0;
            _targetAngle = 0;
            _original = true;
        }

        public SteeringOut(float limitSpeed, float targetAngle) {
            _limitSpeed = limitSpeed;
            _targetAngle = targetAngle;
            _original = false;
        }

        public float LimitSpeed {
            get { return _limitSpeed; }
            set {
                _original = false;
                _limitSpeed = value;
            }
        }

        public float TargetAngle {
            get { return _targetAngle; }
            set {
                _original = false;
                _targetAngle = value;
            }
        }

        public bool IsOriginal () {
            return _original;
        }
    }

    public class SteeringBehaviors {
        enum BehaviorType {
            none = 0x00000,
            seek = 0x00002,
            flee = 0x00004,
            arrive = 0x00008,
            wander = 0x00010,
            cohesion = 0x00020,
            separation = 0x00040,
            alignment = 0x00080,
            obstacle_avoidance = 0x00100,
            wall_avoidance = 0x00200,
            follow_path = 0x00400,
            pursuit = 0x00800,
            evade = 0x01000,
            interpose = 0x02000,
            hide = 0x04000,
            flock = 0x08000,
            offset_pursuit = 0x10000
        }

        MovingAgent _owner;

        MovingAgent _targetAgent;
        Vector2 _vTarget;

        //A point in the wander circle that the agent head to
        Vector2 _vWanderTarget;
        //TODO: Change jitter value
        float _fWanderJitter = 36f;

        //multipliers. These can be adjusted to effect strength of the  
        //appropriate behavior. Useful to get flocking the way you require
        //for example.
        private float _dWeightSeparation;
        private float _dWeightCohesion;
        private float _dWeightAlignment;
        private float _dWeightWander;
        private float _dWeightObstacleAvoidance;
        private float _dWeightWallAvoidance;
        private float _dWeightSeek;
        private float _dWeightFlee;
        private float _dWeightArrive;
        private float _dWeightPursuit;
        private float _dWeightOffsetPursuit;
        private float _dWeightInterpose;
        private float _dWeightHide;
        private float _dWeightEvade;
        private float _dWeightFollowPath;

        //A binary flag to mark which behavior is on or off
        int _flag = 0;

        public LayerMask whatMustAvoid;

        enum Deceleration {
            slow = 3,
            normal = 2,
            fast = 1
        }

        Deceleration _deceleration = Deceleration.fast;
        const float decelerationTweaker = 0.1f;

        public SteeringBehaviors (MovingAgent owner) {
            _owner = owner;
        }

        private bool IsOn (BehaviorType bt) {
            return (_flag & ((int)bt)) == (int)bt;
        }



        private float DeltaAngle2LimitSpeed (float deltaAngle) {
            if (deltaAngle > 90) return _owner.MinSpeed;
            else return Mathf.Max (_owner.MinSpeed, _owner.MaxSpeed * (90 - deltaAngle) / 90);
        }

        private float Distance2LimitSpeed (float distance, Deceleration deceleration) {
            return Mathf.Clamp (distance / (float)deceleration * decelerationTweaker,
                _owner.MinSpeed, _owner.MaxSpeed);
        }

        private SteeringOut Seek (Vector2 targetPos) {
            Vector2 toTarget = targetPos - _owner.Position;

            float targetAngle = Utils.Vector2Angle (toTarget);

            float limitSpeed = DeltaAngle2LimitSpeed (Utils.AbsDeltaAngle
                (_owner.DegreeHeading (), targetAngle));

            return new SteeringOut (limitSpeed, targetAngle);
        }

        private SteeringOut Arive (Vector2 targetPos) {
            Vector2 toTarget = targetPos - _owner.Position;

            float targetAngle = Utils.Vector2Angle (toTarget);
            float limitSpeed = DeltaAngle2LimitSpeed (Utils.AbsDeltaAngle
                (_owner.DegreeHeading (), targetAngle));
            limitSpeed = Mathf.Min (limitSpeed, Distance2LimitSpeed (toTarget.magnitude, _deceleration));

            return new SteeringOut (limitSpeed, targetAngle);
        }

        private SteeringOut Flee (Vector2 targetPos) {
            Vector2 toTarget = _owner.Position - targetPos;

            float targetAngle = Utils.Vector2Angle (toTarget);

            float limitSpeed = DeltaAngle2LimitSpeed (Utils.AbsDeltaAngle
                (_owner.DegreeHeading (), targetAngle));

            return new SteeringOut (limitSpeed, targetAngle);
        }

        private SteeringOut Pursuit (MovingAgent targetAgent) {
            Vector2 toAgent = targetAgent.Position - _owner.Position;

            float deltaHeadingAngle = Utils.AbsDeltaAngle (_owner.DegreeHeading (),
                targetAgent.DegreeHeading ());

            float deltaAngle2Agent = Utils.AbsDeltaAngle (_owner.DegreeHeading (),
                Utils.Vector2Angle (toAgent));

            // If owner and the target agent head to each other
            // -> Seek straight to target agent
            if (deltaAngle2Agent < 10 && deltaHeadingAngle < 10) {
                return Seek (targetAgent.Position);
            }

            //Else Seek toward the predicted future position
            float lookAheadTime = toAgent.magnitude /
                (_owner.Velocity + targetAgent.Velocity);

            return Seek (targetAgent.FuturePosition (lookAheadTime));
        }
        
        private SteeringOut Evade (MovingAgent targetAgent) {
            Vector2 toAgent = targetAgent.Position - _owner.Position;

            float lookAheadTime = toAgent.magnitude /
                (_owner.Velocity + targetAgent.Velocity);

            return Flee (targetAgent.FuturePosition (lookAheadTime));
        }    

        private SteeringOut Wander () {
            SteeringOut steeringOut = new SteeringOut ();

            float newTargetAngle = _owner.TargetAngle ();

            float jitterValue = UnityEngine.Random.Range (-1f, 1f) * _fWanderJitter;
            newTargetAngle += jitterValue;

            if (newTargetAngle > 360)   newTargetAngle -= 360;
            if (newTargetAngle < 0)     newTargetAngle += 360;

            float deltaAngle = Utils.AbsDeltaAngle (newTargetAngle, _owner.DegreeHeading());

            steeringOut.LimitSpeed = DeltaAngle2LimitSpeed (deltaAngle);
            steeringOut.TargetAngle = newTargetAngle;

            return steeringOut;
        }

        private SteeringOut ObstacleAvoidance (LayerMask obstDef) {
            SteeringOut steeringOut = new SteeringOut ();

            RaycastHit2D rcHit =  Physics2D.Raycast (_owner.Position, _owner.VectorHeading(), 
                _owner.obsFeelerLen, obstDef);

            /* Draw feeler
            Debug.DrawRay (_owner.Position, new Vector3 (_owner.VectorHeading().x,
                _owner.VectorHeading ().y, 0) * _owner.obsFeelerLen, Color.green);
            */

            if (rcHit.collider == null) return steeringOut;

            Vector2 toObst = (Vector2) rcHit.transform.position - _owner.Position;
            float toObstDeltaAngle = Utils.DeltaAngle (_owner.DegreeHeading (), Utils.Vector2Angle (toObst));

            float newTargetAngle = _owner.DegreeHeading ();

            if (toObstDeltaAngle < 0)
                newTargetAngle = newTargetAngle + 90 + toObstDeltaAngle;
            else newTargetAngle = newTargetAngle - 90 + toObstDeltaAngle;

            float deltaAngle = Utils.AbsDeltaAngle (newTargetAngle, _owner.DegreeHeading ());

            steeringOut.LimitSpeed = DeltaAngle2LimitSpeed (deltaAngle);
            steeringOut.TargetAngle = newTargetAngle;

            return steeringOut;
        }

        private SteeringOut Alignment (List <MovingAgent> neighbors) {
            SteeringOut steeringOut = new SteeringOut ();

            float newTargetAngle = 0;

            //Getting common direction
            foreach (MovingAgent agent in neighbors) {
                if (agent == null) continue;

                newTargetAngle += agent.DegreeHeading ();
            }
            newTargetAngle /= neighbors.Count;

            float deltaAngle = Utils.AbsDeltaAngle (newTargetAngle, _owner.DegreeHeading ());
            steeringOut.LimitSpeed = DeltaAngle2LimitSpeed (deltaAngle);
            steeringOut.TargetAngle = newTargetAngle;

            return steeringOut;
        }

        //Directly affect body2d
        private void Seperation (List <MovingAgent> neighbors) {
            SteeringOut steeringOut = new SteeringOut ();
            const float seperationForceWeight = 30;

            foreach (MovingAgent agent in neighbors) {
                if (agent == null) continue;

                Vector2 seperationForce = agent.Position - _owner.Position;
                if (seperationForce != Vector2.zero) {
                    seperationForce = seperationForce.normalized / seperationForce.magnitude * seperationForceWeight;
                }
                agent.AddForce (seperationForce);
            }
                
        }

        //Directly affect body2d
        private void Cohesion (List<MovingAgent> neighbors) {
            SteeringOut steeringOut = new SteeringOut ();

            const float cohesionForceWeight = 2;

            Vector2 centerMass = new Vector2(0, 0);

            foreach (MovingAgent agent in neighbors) {
                centerMass += agent.Position;  
            }
            centerMass /= neighbors.Count;

            foreach (MovingAgent agent in neighbors) {
                Vector2 cohesionForce = (centerMass - agent.Position) * cohesionForceWeight;
                agent.AddForce (cohesionForce);
            }

        }

        public SteeringOut CalculateDithered () {
            SteeringOut steeringOut = new SteeringOut ();
            
            double prAlignment = 0.3f;
            double prSeek = 0.8f;
            double prPursuit = 0.8f;
            //TODO: change prValue to get data from file

            //Directly affecting function

            if (IsOn (BehaviorType.cohesion)) {
                Cohesion (_owner.neighbors);
            }

            if (IsOn (BehaviorType.separation)) {
                Seperation (_owner.neighbors);
            }

            //Indirectly affecting function
            System.Random rand = new System.Random ();

            if (IsOn (BehaviorType.alignment) && rand.NextDouble () < prAlignment) {
                steeringOut = Alignment (_owner.neighbors);
            }  

            if (IsOn (BehaviorType.seek) && rand.NextDouble () < prSeek) {
                steeringOut = Seek (_targetAgent.Position);
            }

            if (IsOn (BehaviorType.pursuit) && rand.NextDouble () < prPursuit) {
                steeringOut = Pursuit (_targetAgent);
            }

            return steeringOut;
        }

        public SteeringOut Calculate () {
            SteeringOut steeringOut = new SteeringOut ();
            //TODO: Update on calculate method

            UnityEngine.Random.Range (0f, 1f);

            if (IsOn (BehaviorType.obstacle_avoidance) && Utils.Rand01f () > 0.4f) {
                steeringOut = ObstacleAvoidance (_owner.obstDef);
            }

            if (_targetAgent != null && steeringOut.IsOriginal()) {
                if (IsOn (BehaviorType.pursuit)) {
                    steeringOut = Pursuit (_targetAgent);
                    return steeringOut;
                }

                if (IsOn (BehaviorType.seek)) {
                    steeringOut = Seek (_targetAgent.Position);
                }

                if (IsOn (BehaviorType.arrive)) {
                    steeringOut = Seek (_targetAgent.Position);
                }

                if (IsOn (BehaviorType.flee)) {
                    steeringOut = Flee (_targetAgent.Position);
                }

                if (IsOn (BehaviorType.evade)) {
                    steeringOut = Evade (_targetAgent);
                }

                if (IsOn (BehaviorType.wander)) {
                    steeringOut = Wander ();
                }   
            }            

            return steeringOut;
        }

        public MovingAgent TargetAgent {
            set { _targetAgent = value; }
            get { return _targetAgent; }
        }

        public Vector2 VTarget {
            set { _vTarget = value; }
            get { return _vTarget; }
        }

        public void SeekOn () {
            _flag |= (int) BehaviorType.seek;
        }

        public void ArriveOn () {
            _flag |= (int) BehaviorType.arrive;
        }

        public void PursuitOn () {
            _flag |= (int) BehaviorType.pursuit;
        }

        public void FleeOn () {
            _flag |= (int)BehaviorType.flee;
        }

        public void EvadeOn () {
            _flag |= (int)BehaviorType.evade;
        }

        public void WanderOn () {
            _flag |= (int)BehaviorType.wander;
        }

        public void ObstacleAvoidanceOn () {
            _flag |= (int)BehaviorType.obstacle_avoidance;
        }

        public void CohesionOn () {
            _flag |= (int)BehaviorType.cohesion;
        }

        public void AlignmentOn () {
            _flag |= (int)BehaviorType.alignment;
        }

        public void SeperationOn () {
            _flag |= (int)BehaviorType.separation;
        }
    }
}
