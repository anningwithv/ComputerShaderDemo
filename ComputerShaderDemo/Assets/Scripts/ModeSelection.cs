using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelection : MonoBehaviour
{
    public enum Mode
    {
        None,
        CPU_Mode,
        GPU_Mode,
    }

    [SerializeField]
    private GameObject m_CpuModeObj = null;

    [SerializeField]
    private GameObject m_GpuModeObj = null;

    private Mode m_CurMode = Mode.None;

    private void Start()
    {
        m_CurMode = Mode.CPU_Mode;

        m_CpuModeObj.SetActive(true);
        m_GpuModeObj.SetActive(false);
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(200, 0, 100, 50), m_CurMode.ToString());

        if (GUI.Button(new Rect(0, 0, 100, 50), "CpuMode"))
        {
            m_CurMode = Mode.CPU_Mode;

            m_CpuModeObj.SetActive(false);
            m_GpuModeObj.SetActive(false);

            m_CpuModeObj.SetActive(true);
        }

        if (GUI.Button(new Rect(100, 0, 100, 50), "GpuMode"))
        {
            m_CurMode = Mode.GPU_Mode;

            m_CpuModeObj.SetActive(false);
            m_GpuModeObj.SetActive(false);

            m_GpuModeObj.SetActive(true);
        }
    }
}
