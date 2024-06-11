namespace DefaultNamespace
{
    public class HumanStateMachine
    {
        public HumanState currentState { get; private set; }
        // Start is called before the first frame update

        public void Initialize(HumanState _startState)
        {
            currentState = _startState;
            currentState.Enter();
        }

        // Update is called once per frame
        public void ChangeState(HumanState _newState)
        {
            currentState.Exit();
            currentState = _newState;
            currentState.Enter();
        }
    }
}