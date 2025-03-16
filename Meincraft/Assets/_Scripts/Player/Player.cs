using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] Vector3Int playerSize = new Vector3Int(1,2,1);
    [SerializeField] Transform pCam;

    
    [Space(10)]
    [SerializeField] private Material blockBreakingMaterial;

    private Vector3Int currentBlockPos;
    
    private Vector3Int _aimedBlock;
    private bool _isDigging = false;
    private float _breakProgress = 0f;
    private void OnEnable()
    {
        InputReader.Instance.OnAttackEvent += OnAttack;
        InputReader.Instance.OnInteractEvent += OnInteract;
    }
    private void OnDisable()
    {
        InputReader.Instance.OnAttackEvent -= OnAttack;
        InputReader.Instance.OnInteractEvent -= OnInteract;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _breakProgress = 0;
        blockBreakingMaterial.SetFloat("_BreakProgress", _breakProgress);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDigging)
        {
            if (_aimedBlock != -Vector3Int.one)
            {
                _breakProgress+= Time.deltaTime;
                if (_breakProgress >= 1)
                {
                    _breakProgress = 0f;
                    World.Instance.RemoveBlock(_aimedBlock);
                }
                blockBreakingMaterial.SetFloat("_BreakProgress", _breakProgress);
            }
        }
        else
        {
            if (_breakProgress > 0)
            {
                _breakProgress = 0f;
                blockBreakingMaterial.SetFloat("_BreakProgress", _breakProgress);
            }
        }
        
        var newBlock = GetAimedBlock();
        if (_aimedBlock != newBlock)
        {
            //Aimed block changed
            _aimedBlock = newBlock;
            _breakProgress = 0;
            blockBreakingMaterial.SetFloat("_BreakProgress", _breakProgress);
            blockBreakingMaterial.SetVector("_TargetBlockPosition",_aimedBlock.ToVector4());
        }

        currentBlockPos = new Vector3Int(Mathf.FloorToInt(transform.position.x),
            Mathf.FloorToInt((transform.position.y - playerSize.y / 2f) +0.1f),
            Mathf.FloorToInt(transform.position.z));
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Disabled:
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Started:
                _isDigging = true;
                break;
            case InputActionPhase.Performed:
                break;
            case InputActionPhase.Canceled:
                _isDigging = false;
                break;
        }
    }
    void OnInteract(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Started:
                if (Physics.Raycast(pCam.position, pCam.forward, out RaycastHit hit, 5, LayerMask.GetMask("World")))
                {
                    Vector3Int blockPos = Vector3Int.FloorToInt(hit.point + hit.normal * 0.5f);
                    World.Instance.AddBlock(blockPos);
                }
                break;
            case InputActionPhase.Performed:
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    Vector3Int GetAimedBlock()
    {
        if (Physics.Raycast(pCam.position, pCam.forward, out RaycastHit hit, 5, LayerMask.GetMask("World")))
        {
            Vector3Int blockPos = Vector3Int.FloorToInt(hit.point - hit.normal * 0.5f);
            return blockPos;
        }

        return -Vector3Int.one;
    }
    public void Spawn(Vector3 position)
    {
        transform.position = position;
    }
    public bool CheckIntersects(Vector3Int blockPos)
    {
        Vector3 playerPosMin = currentBlockPos;
        Vector3 playerPosMax = currentBlockPos + playerSize;

        Vector3 blockPosMin = blockPos;
        Vector3 blockPosMax = blockPos + Vector3.one;

        bool overlaps = (playerPosMin.x < blockPosMax.x && playerPosMax.x > blockPosMin.x) &&
                        (playerPosMin.y < blockPosMax.y && playerPosMax.y > blockPosMin.y) &&
                        (playerPosMin.z < blockPosMax.z && playerPosMax.z > blockPosMin.z);

        return overlaps;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(currentBlockPos + new Vector3(0.5f,playerSize.y/2f,0.5f), playerSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(currentBlockPos + Vector3.one/2f, Vector3.one);
    }
}
