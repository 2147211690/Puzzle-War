using System;

namespace Tools
{
    public class StateMachine<T> where T : IState
    {
        public StateMachine(T initialState)
        {
            PreviousState = initialState;
            _currentState = initialState;
        }

        public T CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                CurrentStateChanged?.Invoke(this, value);
            }
        }

        public EventHandler<T>? CurrentStateChanged;
        public T PreviousState { get; private set; }
        private T _currentState;
        public void Init()
        {
            CurrentState.OnEnter();
        }
        public void ChangeState(T newState)
        {
            CurrentState.OnExit();
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState.OnEnter();
        }

        public void ResetState()
        {
            ChangeState(CurrentState);
        }

        public void BackPrevState()
        {
            ChangeState(PreviousState);
        }
    }
    
    public interface IState
    {
        void OnEnter();
        void OnExit();
    }
}