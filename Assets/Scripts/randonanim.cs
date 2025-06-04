using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randonanim : StateMachineBehaviour
{
    [SerializeField]
    private float _TimeUNtilBored;

    [SerializeField]

    private int numberOfBoredAnim;

    private bool _isBored;
    private float _idletime;
    private int _BoredAnim;



   override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        ResetIdle();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_isBored == false)
        {
            _idletime += Time.deltaTime;
             
            if (_idletime > _TimeUNtilBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                _isBored = true;
                _BoredAnim = Random.Range(1, numberOfBoredAnim + 1);
                
            }

        }
        else if (stateInfo.normalizedTime % 1 > 0.98)
        {
            ResetIdle();
        }

        animator.SetFloat("randomanim", _BoredAnim, 0.2f, Time.deltaTime);
    }

    private void ResetIdle()
    {
        _isBored = false;
        _idletime = 0;

        _BoredAnim = 0;

    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
