using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GpuMode : MonoBehaviour
{
    [SerializeField]
    private ComputeShader m_ComputeShader = default;

    [SerializeField]
    private Material m_Material = default;

    [SerializeField]
    private Mesh m_Mesh = default;

    [SerializeField, Range(10, 1000)]
    private int m_Resolution = 10;

    private ComputeBuffer m_PositionsBuffer;

    private float m_PointSize;

    private int m_KernelIndex = -1;

    private readonly int positionsId = Shader.PropertyToID("_Positions");
    private readonly int resolutionId = Shader.PropertyToID("_Resolution");
    private readonly int stepId = Shader.PropertyToID("_Step");
    private readonly int timeId = Shader.PropertyToID("_Time");
    //private readonly int transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    private void OnEnable()
    {
        m_PointSize = 2f / m_Resolution;

        m_PositionsBuffer = new ComputeBuffer(m_Resolution * m_Resolution, 3 * 4);

        m_KernelIndex = m_ComputeShader.FindKernel("WaveKernel");
    }

    private void OnDisable()
    {
        m_PositionsBuffer.Release();
        m_PositionsBuffer = null;
    }

    private void Update()
    {
        UpdatePointsPosition();
    }

    private void UpdatePointsPosition()
    {
        m_ComputeShader.SetInt(resolutionId, m_Resolution);
        m_ComputeShader.SetFloat(stepId, m_PointSize);
        m_ComputeShader.SetFloat(timeId, Time.time);

        m_ComputeShader.SetBuffer(m_KernelIndex, positionsId, m_PositionsBuffer);

        int groups = Mathf.CeilToInt(m_Resolution / 8f);

        //Dispatch命令可以激活我们的Compute Shader，这不是一条绘图指令，但是是一条渲染指令，所以仍然可以条件执行。
        //这个函数的输入也是与传统管线的内置输入不同（顶点、片段），因为它没有任何用户定义的输入。
        //它是取决于我们对计算着色器定义的线程数量，类似工作组的概念，可以执行不同的工作组，
        //这个组的定义是三维的，X,Y,Z，并且它的执行是不按任何规定的顺序执行的，比如可以先执行工作组（1，2，0），
        //然后跳至组（1，0，1）。因此它的执行是跟顺序无关。
        m_ComputeShader.Dispatch(m_KernelIndex, groups, groups, 1);

        m_Material.SetBuffer(positionsId, m_PositionsBuffer);
        m_Material.SetFloat(stepId, m_PointSize);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / m_Resolution));
        Graphics.DrawMeshInstancedProcedural(
            m_Mesh, 0, m_Material, bounds, m_Resolution * m_Resolution
        );
    }
}
