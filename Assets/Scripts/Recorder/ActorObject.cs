using UnityEngine;

public enum ActorStates
{
    Playing,
    Playback,
    Restart,
    Stopped
}

public class ActorObject : MonoBehaviour
{
    private InputRecord record;
    private PlayerController playerController;
    private float recordTime;
    private ActorStates state;
    private int currentStep;

    private float buffer = 0.01f;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        recordTime = 0.0f;
        state = ActorStates.Stopped;
    }

    private void FixedUpdate()
    {
        InputRecordStruct recordStep;
        switch (state)
        {
            case ActorStates.Playing:
                if (currentStep > record.data.Count)
                    break;

                recordStep = record.data[currentStep];

                if (Mathf.Abs(recordTime - recordStep.time) >= buffer || recordTime > recordStep.time)
                {
                    playerController.simulateMove(record.data[currentStep].moveInput);
                    currentStep++;
                }

                if (recordTime <= record.data[record.data.Count - 1].time)
                {
                    recordTime += Time.deltaTime;
                }

                break;

            case ActorStates.Playback:
                if (currentStep == 0)
                    break;

                recordStep = record.data[currentStep];

                if (Mathf.Abs(recordTime - recordStep.time) >= buffer || recordTime < recordStep.time)
                {
                    playerController.simulateMove(record.data[currentStep].moveInput * -1);
                    currentStep--;
                }

                if (recordTime >= 0)
                {
                    recordTime -= Time.deltaTime;
                }

                break;
        }
    }

    public void SetRecord(InputRecord r)
    {
        record = r;
        recordTime = 0.0f;
        state = ActorStates.Playing;
        currentStep = 0;
    }

    public void SetState(ActorStates s)
    {
        state = s;
    }
}
