using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Common.FSM {
    public class StateMachine<EntityType> {
        private EntityType _owner;

        private State<EntityType> _currentState;
        private State<EntityType> _previousState;
        private State<EntityType> _globalState;

        public StateMachine (EntityType owner) {
            _owner = owner;
        }

        public void SetCurrentState (State<EntityType> state) {
            _currentState = state;
        }

        public void SetGlobalState(State<EntityType> state) {
            _globalState = state;
        }

        public void SetPreviousState(State<EntityType> state) {
            _previousState = state;
        }

        public void Update () {
            if (_globalState != null) {
                _globalState.Execute (_owner);
            }

            if (_currentState != null) {
                _currentState.Execute (_owner);
            }
        }

        public void ChangeState (State<EntityType> newState) {
            _previousState = _currentState;
            _currentState.Exit (_owner);

            _currentState = newState;
            _currentState.Enter (_owner);
        }

        public void RevertToPreviousState () {
            ChangeState (_previousState);
        }

        public bool IsInState (State<EntityType> state) {
            return state.GetType () == _currentState.GetType ();
        }

        public State<EntityType> CurrentState () {
            return _currentState;
        }

        public State<EntityType> PreviousState () {
            return _previousState;
        }

        public State<EntityType> GlobalState () {
            return _globalState;
        }

        public void Dispose () {
        }
    }
}
