using Metroidvania.Player.Animation;
using System.Collections;
using UnityEngine;

public class PlayerAnimationView : MonoBehaviour, ICharacterAnimationView
{
    private ICharacterViewDriver CharacterDriver;
    private Animator _animator;
    private Transform _root;
    private Transform _projector;
    private GameObject _model;
    private Transform _headBone;
    private Rigidbody[] _boneRig;
    private readonly int HashSpeed = Animator.StringToHash("Speed");
    private readonly int HashJump = Animator.StringToHash("Jump");
    private readonly int HashFall = Animator.StringToHash("JumpDown");
    private float mass = 0.1f;	// Mass of each bone

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        if (CharacterDriver == null)
        {
            CharacterDriver = GetComponentInParent<ICharacterViewDriver>();
        }
        CharacterDriver.RegisterCharacterAnimationView(this);

        if (_root == null)
            _root = transform.Find("Root");
        if (_projector == null)
            _projector = transform.Find("Blob Shadow Projector");
        if (_model == null)
            _model = transform.Find("MicroMale").gameObject;
        if (_headBone == null)
            _headBone = transform.Find("Head");
        _boneRig = gameObject.GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
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


    public void DisableRagdoll()
    {
        foreach (Component ragdoll in _boneRig)
        {
            if ((ragdoll.GetComponent<Collider>() != null) && ragdoll.GetComponent<Collider>() != this.GetComponent<Collider>())
            {
                ragdoll.GetComponent<Collider>().enabled = false;
                ragdoll.GetComponent<Rigidbody>().isKinematic = true;
                ragdoll.GetComponent<Rigidbody>().mass = 0.01f;
            }
        }
        GetComponent<Collider>().enabled = true;
    }

    public IEnumerator EnableRagdoll(float delay, Vector3 force)
    {
        yield return new WaitForSeconds(delay);
        foreach (Component ragdoll in _boneRig)
        {
            if (ragdoll.GetComponent<Collider>() != null)
                ragdoll.GetComponent<Collider>().enabled = true;
            ragdoll.GetComponent<Rigidbody>().isKinematic = false;
            ragdoll.GetComponent<Rigidbody>().mass = mass;
            if (force.magnitude > 0)
                ragdoll.GetComponent<Rigidbody>().AddForce(force * UnityEngine.Random.value);
        }
        GetComponent<Animator>().enabled = false;
        GetComponent<Collider>().enabled = false;

        //Destroy(GetComponent<BotControlScript>());
        
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }



}

