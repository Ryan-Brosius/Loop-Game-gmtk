using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdentifier : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] SkinnedMeshRenderer body;
    [SerializeField] SkinnedMeshRenderer head;
    [SerializeField] Material redMat;
    [SerializeField] Material blueMat;

    [Header("Head Text")]
    [SerializeField] TextMeshProUGUI identifierText;


    private void Awake()
    {
        if (body == null) body = this.transform.Find("body_mesh").GetComponent<SkinnedMeshRenderer>();
        if (head == null) head = this.transform.Find("head_mesh").GetComponent<SkinnedMeshRenderer>();
        if (identifierText == null) identifierText = this.transform.Find("Identifier Text").GetComponent<TextMeshProUGUI>();
    }

    public void MakeBlue()
    {
        if (body != null) body.material = blueMat;
        if (head != null) head.material = blueMat;
    }

    public void MakeRed()
    {
        if (body != null) body.material = redMat;
        if (head != null) head.material = redMat;
    }

    public void IdentifierNumber(int actorNumber)
    {
        if (identifierText != null)
        {
            identifierText.text = IntToRoman(actorNumber);
        }
    }

    public static string IntToRoman(int number)
    {
        if (number < 1 || number > 100)
            return "Invalid"; // Roman numerals range from 1 to 3999

        var map = new (int value, string symbol)[]
        {
        (100, "C"),
        (90, "XC"),
        (50, "L"),
        (40, "XL"),
        (10, "X"),
        (9, "IX"),
        (5, "V"),
        (4, "IV"),
        (1, "I")
        };

        string result = "";
        foreach (var (value, symbol) in map)
        {
            while (number >= value)
            {
                result += symbol;
                number -= value;
            }
        }
        return result;
    }
}
