using UnityEngine;

namespace AkaitoAi
{
    [RequireComponent(typeof(Animator))]

    public class AnimatorIK : MonoBehaviour
    {
        protected Animator animator;

        public bool ikActive = true;

        public IKPointsClass IKPoints;

        private float speed = 0.0f;

        private Vector3 myPosition;
        private Quaternion myRotation;

        [System.Serializable]
        public class IKPointsClass
        {
            public Transform rightHand, leftHand;
            public Transform rightFoot, leftFoot;
        }

        private void Awake()
        {
            animator = transform.GetComponent<Animator>();

            myPosition = transform.localPosition;
            myRotation = transform.localRotation;
        }

        //a callback for calculating IK
        void OnAnimatorIK()
        {
            if (transform.GetComponent<Animator>().enabled != true) return;

            if (animator)
            {

                //if the IK is active, set the position and rotation directly to the goal. 
                if (ikActive)
                {

                    if (IKPoints.leftHand != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
                    }

                    if (IKPoints.rightHand != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                    }

                    if (IKPoints.rightFoot != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);
                    }

                    if (IKPoints.leftFoot != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
                    }


                    if (speed > -1)
                    {

                        //set the position and the rotation of the right hand where the external object is
                        if (IKPoints.rightHand != null)
                        {
                            animator.SetIKPosition(AvatarIKGoal.RightHand, IKPoints.rightHand.position);
                            animator.SetIKRotation(AvatarIKGoal.RightHand, IKPoints.rightHand.rotation);
                        }

                        if (IKPoints.rightFoot != null)
                        {
                            animator.SetIKPosition(AvatarIKGoal.RightFoot, IKPoints.rightFoot.position);
                            animator.SetIKRotation(AvatarIKGoal.RightFoot, IKPoints.rightFoot.rotation);
                        }

                        if (IKPoints.leftFoot != null)
                        {

                            animator.SetIKPosition(AvatarIKGoal.LeftFoot, IKPoints.leftFoot.position);
                            animator.SetIKRotation(AvatarIKGoal.LeftFoot, IKPoints.leftFoot.rotation);
                        }

                        if (IKPoints.leftHand != null)
                        {
                            animator.SetIKPosition(AvatarIKGoal.LeftHand, IKPoints.leftHand.position);
                            animator.SetIKRotation(AvatarIKGoal.LeftHand, IKPoints.leftHand.rotation);
                        }
                    }
                }

                //if the IK is not active, set the position and rotation of the hand back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                }
            }
        }
    }
}