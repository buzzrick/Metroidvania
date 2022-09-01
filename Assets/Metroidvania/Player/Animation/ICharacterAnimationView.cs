using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterAnimationView 
{
    void SetGrounded(bool isGrounded);
    void SetJumping(bool isJumping);
    void SetSpeed(float speed);
}
