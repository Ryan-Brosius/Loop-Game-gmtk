using System.Collections.Generic;
using UnityEngine;

public struct InputRecordStruct
{
    public float time;
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool isAttacking;
}

public class InputRecord
{
    public List<InputRecordStruct> data = new List<InputRecordStruct>();
    public Vector3 startPosition { get; private set; }

    public InputRecord() : this(Vector3.zero) { }

    public InputRecord(Vector3 startPosition)
    {
        this.startPosition = startPosition;
    }

    public void AddToRecord(InputRecordStruct record)
    {
        data.Add(record);
    }
}
