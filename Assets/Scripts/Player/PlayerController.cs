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

    private Vector3 velocity;

    InputRecord recorder = new InputRecord();
    [SerializeField] private ActorObject test;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    /*void OnLook(InputValue value)
    {
    }*/

    private void FixedUpdate()
    {
        HandleMove();
        HandleLook();

        HandleRecording();

        if ((Input.GetKeyDown(KeyCode.K)) && test != null)
        {
            Debug.Log("Started??");
            InputRecorderManager.Instance.AddRecording(recorder, test);
            InputRecorderManager.Instance.PlayAllActors();

        }

        if ((Input.GetKeyDown(KeyCode.L)) && test != null)
        {
            InputRecorderManager.Instance.ReverseAllRecordings();
        }
    }

    private void HandleMove()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        transform.position += move * Time.deltaTime;
    }

    private void HandleLook()
    {
        if (Mouse.current == null)
            return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = (mouseWorldPos - transform.position);
        if (direction.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle - 90f, 0);
    }

    private void HandleRecording()
    {
        recorder.addToRecord(new InputRecordStruct
        {
            time = Time.time,
            moveInput = moveInput,
            lookInput = lookInput,
        });
    }

    public void simulateMove(Vector2 value)
    {
        moveInput = value;
    }
}