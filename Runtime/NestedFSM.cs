using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peg.SimpleFSM
{
    /// <summary>
    /// A state that references another FSM that runs in its place.
    /// </summary>
    public abstract class NestedFSM : FSMState
    {
        [Tooltip("An FSM attached to a gameobject that is a direct child of this state's FSM.")]
        public FSM SubFsm;

        protected virtual void Awake()
        {
            if (SubFsm == null)
                throw new UnityException("No FSM associated with the nest fsm state '" + StateName + "' in the FSM '" + Fsm.name + "'.");
            if (SubFsm.transform.parent != this.transform)
                throw new UnityException("The nested FSM state '" + StateName + "' is pointing to an FSM that is not a direct child of the FSM '" + Fsm.name + "'.");
        }

        protected virtual void OnEnable()
        {
            SubFsm.enabled = true;
        }

        protected virtual void OnDisable()
        {
            SubFsm.enabled = true;
        }
    }
}
