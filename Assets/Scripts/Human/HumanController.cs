using UnityEngine;

namespace DefaultNamespace
{
    public class HumanController: MonoBehaviour
    {
        public Rigidbody rigidbody;
        public Animator animator;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        public void SetVelocity(float speed)
        {
            if (speed == 0)
            {
                animator.SetBool("Walk",false);
                animator.SetBool("Idle", true);
            }
            else
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Walk",true);
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