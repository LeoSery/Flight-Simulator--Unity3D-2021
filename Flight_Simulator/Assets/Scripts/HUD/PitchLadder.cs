using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PitchLadder : MonoBehaviour
{
    struct Bar
    {
        public RectTransform Transform;
        public float Angle;
        public PitchBar CurrentBar;

        public Bar(RectTransform transform, float angle, PitchBar bar)
        {
            Transform = transform;
            Angle = angle;
            CurrentBar = bar;
        }
    }

    public Camera CurrentCamera;
    public Transform PlaneTransform;
    public GameObject PitchHorizonPrefab;
    public GameObject PitchPositivePrefab;
    public GameObject PitchNegativePrefab;
    public int BarInterval = -1;
    public int Range = -1;

    private RectTransform m_transform;
    private List<Bar> m_bars;

    private void Awake()
    {
        Assert.IsTrue(PitchNegativePrefab);
        Assert.IsTrue(PitchPositivePrefab);
        Assert.IsTrue(BarInterval > 0);
        Assert.IsTrue(Range > 0);
    }

    private void Start()
    {
        m_transform = GetComponent<RectTransform>();
        m_bars = new List<Bar>();

        for (int i = -Range; i <= Range; i++)
        {
            if (i % BarInterval != 0) continue;

            if (i == 0 || i == 90 || i == -90)
                CreateBar(i, PitchHorizonPrefab);
            else if (i > 0)
                CreateBar(i, PitchPositivePrefab);
            else
                CreateBar(i, PitchNegativePrefab);
        }
    }

    void CreateBar(int angle, GameObject prefab)
    {
        var bar = Instantiate(prefab, m_transform);
        var barTransform = bar.GetComponent<RectTransform>();
        var barBar = bar.GetComponent<PitchBar>();

        barBar.SetNumber(angle);
        m_bars.Add(new Bar(barTransform, angle, barBar));
    }

    private void LateUpdate()
    {
        if (PlaneTransform == null)
            return;

        float currentPitch = -PlaneTransform.eulerAngles.x;
        float currentRoll = PlaneTransform.eulerAngles.z;
        transform.localEulerAngles = new Vector3(0, 0, -currentRoll);

        foreach (var bar in m_bars)
        {
            float angle = Mathf.DeltaAngle(currentPitch, bar.Angle);
            float position = Utilities.GetPosition(Utilities.ConvertAngle(angle), CurrentCamera);

            if (Mathf.Abs(angle) < 90f && position >= m_transform.rect.yMin && position <= m_transform.rect.yMax)
            {
                var pos = bar.Transform.localPosition;
                bar.Transform.localPosition = new Vector3(pos.x, position, pos.z);
                bar.Transform.gameObject.SetActive(true);

                if (bar.CurrentBar != null)
                    bar.CurrentBar.UpdateRoll(currentRoll);
            }
            else
            {
                bar.Transform.gameObject.SetActive(false);
            }

        }
    }
}
