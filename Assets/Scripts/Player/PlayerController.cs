using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isAttacking;

    private Vector3 velocity;

    public InputRecord recorder = new InputRecord();
    private bool isRecording = true;

    [Header("Shootings stuff")]
    [SerializeField] private GameObject ShootPoint;
    [SerializeField] private GameObject ProjectilePrefab;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnAttack()
    {
        isAttacking = true;
    }

    private void FixedUpdate()
    {
        HandleMove();
        HandleAttack();

        HandleRecording();

        isAttacking = false;  
    }

    private void HandleMove()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (move.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            controller.Move(move * moveSpeed * Time.deltaTime);
        }
    }

    private void HandleRecording()
    {
        if (!isRecording)
            return;

        recorder.AddToRecord(new InputRecordStruct
        {
            time = Time.time,
            moveInput = moveInput,
            lookInput = lookInput,
            isAttacking = isAttacking,
        });
    }

    void HandleAttack()
    {
        if (isAttacking)
        {
            Instantiate(ProjectilePrefab, gameObject.transform.position, Quaternion.identity);
        }
    }

    public InputRecord KillMyselfStopRecording()
    {
        isRecording = false;
        return recorder;
    }

    public void simulateMove(Vector2 value)
    {
        moveInput = value;
    }

    public void simulateAttack(bool value)
    {
        isAttacking = value;
    }
}