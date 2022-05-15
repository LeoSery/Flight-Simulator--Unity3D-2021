using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    private static readonly string[] m_directions =
    {
        "N",
        "NE",
        "E",
        "SE",
        "S",
        "SW",
        "W",
        "NW"
    };

    struct Tick
    {
        public RectTransform Transform;
        public Image Image;
        public uint Angle;

        public Tick(RectTransform transform, Image image, uint angle)
        {
            Transform = transform;
            Image = image;
            Angle = angle;
        }
    }

    public Transform PlaneTransform;
    public Camera CurrentCamera;

    public GameObject LargeTickPrefab, SmallTickPrefab, TextPrefab;
    public uint LargeTickInterval, SmallTickInterval;

    private List<Tick> m_ticks;
    private List<Text> m_ticksText;

    private RectTransform m_transform;

    private void Awake()
    {
        Assert.IsTrue(LargeTickPrefab);
        Assert.IsTrue(SmallTickPrefab);
        Assert.IsTrue(TextPrefab);
    }

    private void Start()
    {
        m_transform = GetComponent<RectTransform>();
        m_ticks = new List<Tick>();
        m_ticksText = new List<Text>();

        for (uint i = 0; i < 360; i++)
        {
            if (i % LargeTickInterval == 0)
            {
                GameObject tick = Instantiate(LargeTickPrefab, transform);
                RectTransform tickTransform = tick.GetComponent<RectTransform>();
                Image tickImage = tick.GetComponent<Image>();

                GameObject text = Instantiate(TextPrefab, tickTransform);
                Text textText = text.GetComponent<Text>();

                if (i % 45 == 0)
                    textText.text = m_directions[i / 45];
                else
                    textText.text = i.ToString();

                m_ticks.Add(new Tick(tickTransform, tickImage, i));
                m_ticksText.Add(textText);
            }
            else if (i % SmallTickInterval == 0)
            {
                GameObject tick = Instantiate(SmallTickPrefab, transform);
                RectTransform tickTransform = tick.GetComponent<RectTransform>();
                Image tickImage = tick.GetComponent<Image>();

                m_ticks.Add(new Tick(tickTransform, tickImage, i));
            }
        }
    }

    private void LateUpdate()
    {
        if (PlaneTransform == null)
            return;

        float yaw = PlaneTransform.eulerAngles.y;

        foreach (var tick in m_ticks)
        {
            float angle = Mathf.DeltaAngle(yaw, tick.Angle);
            float position = Utilities.GetPosition(Utilities.ConvertAngle(angle), CurrentCamera);

            if (Mathf.Abs(angle) < 90f && position >= m_transform.rect.xMin && position <= m_transform.rect.xMax)
            {
                var pos = tick.Transform.localPosition;
                tick.Transform.localPosition = new Vector3(position, pos.y, pos.z);
                tick.Transform.gameObject.SetActive(true);
            }
            else
            {
                tick.Transform.gameObject.SetActive(false);
            }
        }
    }
}
