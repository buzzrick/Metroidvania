using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationView : MonoBehaviour, ICharacterAnimationView
{
    public ThirdPersonMovement PlayerController;
    private Animator _animator;

    private readonly int HashSpeed = Animator.StringToHash("Speed");
    private readonly int HashJump = Animator.StringToHash("Jump");
    private readonly int HashFall = Animator.StringToHash("JumpDown");

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        PlayerController.RegisterCharacterAnimationView(this);
    }

    public void SetSpeed(float speed)
    {
        _animator.SetFloat(HashSpeed, speed);
    }

    public void SetJumping(bool isJumping)
    {
        _animator.SetBool(HashJump, isJumping);
    }

    public void SetGrounded(bool isGrounded)
    {
        _animator.SetBool(HashFall, isGrounded);
    }
}

