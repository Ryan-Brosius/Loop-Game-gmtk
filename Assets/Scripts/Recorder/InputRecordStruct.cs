using System.Collections.Generic;
using UnityEngine;

public struct InputRecordStruct
{
    public float time;
    public Vector2 moveInput;
    public Vector2 lookInput;
}

public class InputRecord
{
    public List<InputRecordStruct> data = new List<InputRecordStruct>();

    public void addToRecord(InputRecordStruct record)
    {
        data.Add(record);
    }
}
