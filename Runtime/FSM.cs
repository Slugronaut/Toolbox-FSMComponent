using System;
using System.Collections.Generic;
using UnityEngine;


namespace Peg.SimpleFSM
{
    /// <summary>
    /// 
    /// TODO:
    ///     -Allow an option to 'pass filter' when transitioning states. This means that if a state switch is
    ///      attempted and can't be done (e.g. the state doesn't exist, another state can be switched to as a backup.)
    ///     -Implement 'transitions'. They act as filters that can determine what states are allow to transition to what other states.
    ///     -Implement 'triggers'. Similar to Unity's Animator triggers. They act as an easy way to tell the system to transition to
    ///      a different state by simply setting a value (bool, float, trigger).
    /// </summary>
    public sealed class FSM : MonoBehaviour
    {
        public enum RestartAction
        {
            Restart,
            Unpause,
        }

        [Tooltip("The initial state that will be enabled when this FSM starts.")]
        public FSMState DefaultState;

        [Tooltip("Does this FSM restart to the default state or pause when disabled?")]
        public RestartAction WhenEnabled;

        FSMState _CurrentState;
        public FSMState CurrentState
        {
            get { return _CurrentState; }
            set { SwitchToState(value.StateHashId); }
        }

        [Tooltip("If true, reports errors when trying to switch to states that don't exist. Set to false if you need this functionality without errors.")]
        public bool ErrorOnInvalidSwitch = true;

#pragma warning disable CS0649 // Field 'FSM.States' is never assigned to, and will always have its default value null
        Dictionary<int, FSMState> States;
#pragma warning restore CS0649 // Field 'FSM.States' is never assigned to, and will always have its default value null
        bool FirstRun = true;

        void Awake()
        {
            if (DefaultState == null)
                throw new UnityException("No default state supplied to the FSM '" + name + "'.");
            RefreshStates();
        }

        private void Start()
        {
            foreach (var state in States.Values)
            {
                if (state != DefaultState) state.enabled = false;
            }
        }

        void OnEnable()
        {
            RefreshStates();

            if (FirstRun)
            {
                FirstRun = false;
                return;
            }

            if (WhenEnabled == RestartAction.Restart)
                Restart();

        }

        void OnDisable()
        {
            if (_CurrentState != null)
                _CurrentState.enabled = false;
        }

        public void RefreshStates()
        {
            States.Clear();

            var states = GetComponents<FSMState>();
            for (int i = 0; i < states.Length; i++)
            {
                try { States.Add(states[i].StateHashId, States[i]); }
#pragma warning disable 0168
                catch (Exception e)
#pragma warning restore 0168
                {
                    Debug.LogError("State name '" + states[i].StateName + "' is already in use in the FSM '" + name + "'. If that name isn't in use then a duplicate hash id has been generated and one of the offending states names must change.");
                }
            }
        }

        /// <summary>
        /// Returns the FSM to the default state and invokes Restarted() on all states.
        /// </summary>
        public void Restart()
        {
            foreach (var state in States.Values)
                state.OnRestarted();
            CurrentState = DefaultState;
        }

        /// <summary>
        /// Switches to the named state if the transition is considered valid.
        /// NOTE: Transitions are not current implemented so all state switches will be valid.
        /// </summary>
        /// <param name="stateId">The hash id of the state to switch to.</param>
        /// <param name="force">If <c>true</c> the state switch will occur even if a valid transition does not exist.</param>
        /// <returns><c>true</c> if the switch was successful, <c>false</c> otherwise.</returns>
        public bool SwitchToState(int stateId, bool force = false)
        {
            //TODO: implement transition filters

            FSMState state;
            if (!States.TryGetValue(stateId, out state))
            {
                if (ErrorOnInvalidSwitch)
                    Debug.LogError("State id '" + stateId + "' does not belong to the FSM '" + name + "'.");
                return false;
            }

            _CurrentState.enabled = false;
            _CurrentState = state;
            _CurrentState.enabled = true;
            return true;
        }
    }
}
