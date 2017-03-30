using UnityEngine;
using System.Collections;

public class NoCollidersBehavior : StateMachineBehaviour
{
    [Header("Deactivate Colliders on Enter and activate them on Exit")]

    CapsuleCollider[] cap;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cap = animator.GetComponentsInChildren<CapsuleCollider>();
        foreach (CapsuleCollider item in cap) item.enabled = false;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (CapsuleCollider item in cap)  item.enabled = true;
    }

}