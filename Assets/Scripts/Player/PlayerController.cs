using Unity.VisualScripting;
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
    private Vector3 aimInput;
    private bool isAttacking;

    private Vector3 velocity;

    public InputRecord recorder;
    private bool isRecording = true;

    [Header("Shootings stuff")]
    [SerializeField] private GameObject ShootPoint;
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] LayerMask groundLayer;
    public bool canAttack = true;
    [SerializeField] GameObject heldSpear;

    [Header("Animation")]
    [SerializeField] Animator animController;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animController = this.GetComponent<Animator>();
    }

    private void Awake()
    {
        recorder = new InputRecord(this.transform.position);
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnAttack()
    {
        if (canAttack)
        {
            Aim();
            isAttacking = true;
        }
    }

    private void FixedUpdate()
    {
        HandleMove();
        HandleAttack();

        HandleRecording();

        isAttacking = false;

        if (heldSpear) heldSpear.SetActive(canAttack);
    }

    private void HandleMove()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (move.magnitude > 0)
        {
            if (animController) animController.SetBool("Moving", true);

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            controller.Move(move * moveSpeed * Time.deltaTime);
        }
        else
        {
            if (animController) animController.SetBool("Moving", false);
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
            aimInput = aimInput,
            isAttacking = isAttacking,
        });
    }

    void HandleAttack()
    {
        if (isAttacking && canAttack)
        {
            if (animController) animController.SetTrigger("Attack");

            canAttack = false;
            GameObject spear = Instantiate(ProjectilePrefab, ShootPoint.transform.position, Quaternion.LookRotation(aimInput));

            if (spear.TryGetComponent<SpearController>(out SpearController spearScript))
            {
                spearScript.SpawnSpear(this.gameObject);
            }
        }
    }

    void Aim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 target = hit.point;
            target = new Vector3(target.x, ShootPoint.transform.position.y, target.z);
            aimInput = (target - ShootPoint.transform.position).normalized;
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

    public void simulateAttack(bool value, Vector3 aimDirection)
    {
        aimInput = aimDirection;
        isAttacking = value;
    }
}