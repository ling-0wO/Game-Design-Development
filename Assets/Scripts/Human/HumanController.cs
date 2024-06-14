using UnityEngine;

namespace DefaultNamespace
{
    public class HumanController: MonoBehaviour
    {
        public Rigidbody rigidbody;
        public Animator animator;
        private string currentState;

        public void SetState(string newState)
        {
            animator.SetBool(currentState,false);
            animator.SetBool(newState, true);
            currentState = newState;
        }

        public string GetState()
        {
            return currentState;
        }
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            currentState = "Idle";            
            animator.SetBool(currentState,false);
        }

        public void SetVelocity(float speed)
        {
            if (speed == 0)
            {
                SetState("Idle");
            }
            else
            {
                SetState("Walk");
            }
            rigidbody.velocity = transform.forward * speed;
        }

        public void SetFacingPosition(Vector3 position)
        {
            Vector3 dir = position - transform.position;
            dir = new Vector3(dir.x, 0, dir.z);
            Quaternion ang = Quaternion.LookRotation(dir);
            rigidbody.MoveRotation(ang);
        }
    }
}