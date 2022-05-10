using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitchBar : MonoBehaviour
{
    public List<Text> Texts;
    private List<Transform> m_transforms;

    void Start()
    {
        m_transforms = new List<Transform>();

        foreach (var text in Texts)
            m_transforms.Add(text.GetComponent<Transform>());
    }

    public void SetNumber(int number)
    {
        foreach (var text in Texts)
            text.text = number.ToString();
    }

    public void UpdateRoll(float angle)
    {
        foreach (var transform in m_transforms)
            transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
