using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace KK.StatePattern
{
    /// <summary>
    /// Base State for implementing the State Design Pattern. Modified to support the Job System if needed
    /// </summary>
    public abstract class State
    {
        protected FSM fsm;
        public State(FSM fsm)
        {
            this.fsm = fsm;
        }
        /// <summary>
        /// By default Jobs support is disabled. Override in deriving class. To support Jobs, schedule a Job and return true
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="jobHandle"></param>
        /// <returns>True if the State supports Jobs</returns>
        public virtual bool ScheduleJob(float deltaTime, out JobHandle jobHandle)
        {
            jobHandle = new JobHandle();
            return false;
        }
        /// <summary>
        /// Invoked once when the State begins to update. Can be overriden for custom behaviour
        /// </summary>
        public virtual void OnStateEnter()
        {

        }
        /// <summary>
        /// Invoked each time the State updated. Can be overriden for custom behaviour
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void OnStateUpdate(float deltaTime)
        {

        }
        /// <summary>
        /// Invoked once when the State ceases to update. Can be overriden for custom behaviour
        /// </summary>
        public virtual void OnStateExit()
        {

        }
    }
}