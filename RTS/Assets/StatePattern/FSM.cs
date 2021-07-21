using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KK.StatePattern
{
    /// <summary>
    /// FiniteStateMachine implementing States from the State Design Pattern. Enables manual State udpating
    /// </summary>
    public class FSM
    {
        public State currentState;
        public State defaultState;
        private Queue<State> stateQueue;

        public FSM()
        {            
            stateQueue = new Queue<State>();
        }
        public void AddState(State state)
        {
            stateQueue.Enqueue(state);
        }
        public void ClearStates()
        {
            stateQueue.Clear();
        }
        public void FinishCurrentState()
        {
            if (currentState != null)
            {
                if (stateQueue.Count != 0)
                {
                    SetState(stateQueue.Dequeue());
                }
                else
                {
                    SetState(defaultState);
                }
            }
        }
        /// <summary>
        /// Set the currentState to state
        /// </summary>
        /// <param name="state"></param>
        public void SetState(State state)
        {
            if (currentState != null)
                currentState.OnStateExit();

            currentState = state;

            if (currentState != null)
                currentState.OnStateEnter();
        }
        /// <summary>
        /// Manual update of the state machine. Invoke every frame
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateFSM(float deltaTime)
        {
            if (currentState != null)
            {
                currentState.OnStateUpdate(deltaTime);
            }
            else
            {
                if (stateQueue.Count != 0)
                {
                    SetState(stateQueue.Dequeue());
                }
                else
                {
                    SetState(defaultState);
                }
            }
        }
    }
}