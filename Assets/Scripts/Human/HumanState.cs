using UnityEngine;

namespace DefaultNamespace
{
    public class HumanState
    {
        protected HumanStateMachine stateMachine;
        protected HumanController human;

        protected Rigidbody rb;
        private string animBoolName;

        protected float stateTimer;
        protected bool triggerCalled;
        public HumanState(HumanController _human, HumanStateMachine _stateMachine, string _animBoolName)
        {
            this.human = _human;

            this.stateMachine = _stateMachine;
            this.animBoolName = _animBoolName;
        }

        public virtual void Enter()
        {
            human.animator.SetBool(animBoolName, true);
            rb = human.rigidbody;
            triggerCalled = false;
        }

        public virtual void Update()
        {
            stateTimer -= Time.deltaTime;
        }

        public virtual void Exit()
        {

            human.animator.SetBool(animBoolName, false);
        }

        public virtual void AnimationFinishTrigger()
        {
            triggerCalled = true;
        }
    }
}