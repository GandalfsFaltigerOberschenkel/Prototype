using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public void ProcessInput(Transform groundChecker, Transform ceilingChecker, ref bool isGrounded, ref bool hitCeiling)
    {
       
       isGrounded = IsGrounded(groundChecker);
       hitCeiling = IsHitCeiling(ceilingChecker);

     

     
    }

    public bool IsGrounded(Transform groundChecker)
    {
        if(GetComponent<FallThroughPlattforms>().isFallingThrough)
        {
            return false;
        }
        return Physics2D.OverlapCircleAll(groundChecker.position, 0.1f)
            .Any(collider => collider.gameObject != gameObject && !collider.isTrigger && collider.tag != "Enemy");
    }

    public bool IsHitCeiling(Transform ceilingChecker)
    {
        return Physics2D.OverlapCircleAll(ceilingChecker.position, 0.1f)
            .Any(collider => collider.gameObject != gameObject && !collider.isTrigger && collider.tag != "Enemy");
    }
}


