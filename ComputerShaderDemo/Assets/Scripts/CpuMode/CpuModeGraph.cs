using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuModeGraph : MonoBehaviour
{
    [SerializeField]
    private Transform m_PointPrefab = default;

    [SerializeField, Range(10, 200)]
    private int m_Resolution = 10;

    private Transform[] m_PointArray;

    private float m_PointSize;

    private void Awake()
    {
        m_PointSize = 2f / m_Resolution;
        var scale = Vector3.one * m_PointSize;

        m_PointArray = new Transform[m_Resolution * m_Resolution];

        for (int i = 0; i < m_PointArray.Length; i++)
        {
            Transform point = Instantiate(m_PointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            m_PointArray[i] = point;
        }
    }

    private void Update()
    {
        UpdatePointsPosition();
    }

    private void UpdatePointsPosition()
    {
        float time = Time.time;

        float v = 0.5f * m_PointSize - 1f;
        for (int i = 0, x = 0, z = 0; i < m_PointArray.Length; i++, x++)
        {
            if (x == m_Resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * m_PointSize - 1f;
            }
            float u = (x + 0.5f) * m_PointSize - 1f;

            m_PointArray[i].localPosition = PositionFunc.Wave(u, v, time);
        }
    }
}
