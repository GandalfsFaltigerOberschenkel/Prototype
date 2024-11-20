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
        return Physics2D.OverlapCircleAll(groundChecker.position, 0.2f)
            .Any(collider => collider.gameObject != gameObject);
    }

    public bool IsHitCeiling(Transform ceilingChecker)
    {
        return Physics2D.OverlapCircleAll(ceilingChecker.position, 0.2f)
            .Any(collider => collider.gameObject != gameObject && !collider.isTrigger);
    }
}


