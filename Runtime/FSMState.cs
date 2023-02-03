using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox.SimpleFSM
{
    /// <summary>
    /// Base class for all FSM States.
    /// </summary>
    [RequireComponent(typeof(FSM))]
    public abstract class FSMState : MonoBehaviour
    {
        [Tooltip("Identifying name of this state. Must be unique.")]
        [SerializeField]
#pragma warning disable CS0649 // Field 'FSMState._StateId' is never assigned to, and will always have its default value null
        HashedString _StateId;
#pragma warning restore CS0649 // Field 'FSMState._StateId' is never assigned to, and will always have its default value null
        public string StateName { get { return _StateId.Value; } }
        public int StateHashId { get { return _StateId.Hash; } }
        public FSM Fsm { get; set; }

        /// <summary>
        /// Helper for generating a hashid from a string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int StateNameToHash(string name)
        {
            return HashedString.StringToHash(name);
        }

        /// <summary>
        /// Switches to the named state if the transition is considered valid.
        /// </summary>
        /// <param name="stateId">The hash id of the state to switch to.</param>
        /// <param name="force">If <c>true</c> the state switch will occur even if a valid transition does not exist.</param>
        /// <returns><c>true</c> if the switch was successful, <c>false</c> otherwise.</returns>
        public void TransitionTo(int stateId, bool force = false)
        {
            Fsm.SwitchToState(stateId, force);
        }

        public abstract void OnRestarted();
    }
}
