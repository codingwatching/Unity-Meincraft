using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float sprintSpeedMultiplier;
    [SerializeField] float gravity;
    
    [Space(10), Header("Jumping")]
    [SerializeField] float jumpPower;
    [SerializeField] float jumpThresholdTime;

    [Space(10), Header("Flying")] 
    [SerializeField] private float flyingThresholdTime;
    [SerializeField] private float flyingSpeedMultiplier;
    [SerializeField] private float riseSpeed;
    [SerializeField] private float descendSpeed;
    
    
    private Vector3 _movement;
    private Vector3 _velocity;
    private float _lastJumpPressInputTime;

    private bool _sprinting;
    private bool _flying;
    private bool _rise;
    private bool _descend;
    
    [Space(10)]
    [SerializeField] CharacterController cc;
    private void Reset()
    {
        TryGetComponent(out cc);
    }
    private void OnEnable()
    {
        InputReader.Instance.OnJumpEvent += OnJump;
        InputReader.Instance.OnSprintEvent += OnSprint;
        InputReader.Instance.OnCrouchEvent += OnCrouch;
    }
    private void OnDisable()
    {
        InputReader.Instance.OnJumpEvent -= OnJump;
        InputReader.Instance.OnSprintEvent -= OnSprint;
        InputReader.Instance.OnCrouchEvent -= OnCrouch;
    }
    
    void Start()
    {
        _lastJumpPressInputTime = -1f;
    }

    void Update()
    {
        float totalMovementMultiplier = 1f;
        if (_flying)
        {
            totalMovementMultiplier *= flyingSpeedMultiplier;
        }
        if (_sprinting)
        {
            totalMovementMultiplier *= sprintSpeedMultiplier;
        }
        _movement = transform.forward * InputReader.Instance.Movement.y + transform.right * InputReader.Instance.Movement.x;
        _velocity.x = _movement.normalized.x * movementSpeed * totalMovementMultiplier;
        _velocity.z = _movement.normalized.z * movementSpeed * totalMovementMultiplier;
        if (!cc.isGrounded)
        {
            if(!_flying) _velocity.y -= gravity * Time.deltaTime;
            else
            {
                if(_rise) _velocity.y = riseSpeed;
                else if (_descend) _velocity.y = -descendSpeed;
                else _velocity.y = 0;
            }
        }
        else
        {
            if (_flying) _flying = false;
            if (_velocity.y < jumpPower && Time.time <= _lastJumpPressInputTime + jumpThresholdTime)
            {
                //Jump
                _velocity.y = jumpPower;
            }
            else
            {
                _velocity.y = 0;
            }
        }
        
        cc.Move(_velocity * Time.deltaTime);
    }
    void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (Time.time - _lastJumpPressInputTime <= flyingThresholdTime)
                {
                    _flying = !_flying;
                }

                _rise = true;
                _lastJumpPressInputTime = Time.time;
                break;
            case InputActionPhase.Canceled:
                _rise = false;
                break;
        }
    }
    void OnCrouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _descend = true;
                break;
            case InputActionPhase.Canceled:
                _descend = false;
                break;
        }
    }
    void OnSprint(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _sprinting = true;
                break;
            case InputActionPhase.Canceled:
                _sprinting = false;
                break;
        }
    }
}
