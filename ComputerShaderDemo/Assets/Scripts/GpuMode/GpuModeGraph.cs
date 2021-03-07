using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GpuModeGraph : MonoBehaviour
{
    //单个点的mesh
    [SerializeField]
    private Mesh m_Mesh = default;

    //用于渲染Mesh的材质
    [SerializeField]
    private Material m_Material = default;

    //图形分辨率
    [SerializeField, Range(10, 1000)]
    private int m_Resolution = 10;

    //用于计算点位置的Computershader
    [SerializeField]
    private ComputeShader m_ComputeShader = default;

    //整个图形的大小
    private const float m_GraphSize = 2;

    //图形中点的间隔大小
    private float m_StepSize;

    //使用ComputerBuffer将物体位置存储在GPU上
    private ComputeBuffer m_PositionsBuffer;

    //ComputerShader中Kernel方法入口
    private int m_KernelIndex = -1;

    //Shader中的变量id
    private readonly int positionsId = Shader.PropertyToID("_Positions");
    private readonly int resolutionId = Shader.PropertyToID("_Resolution");
    private readonly int stepId = Shader.PropertyToID("_Step");
    private readonly int timeId = Shader.PropertyToID("_Time");

    private void OnEnable()
    {
        m_StepSize = m_GraphSize / m_Resolution;

        //为ComputerBuffer申请空间(元素个数，单个元素占用的字节数) 目前存储的是物体位置vector3, 每个float占用4个字节
        m_PositionsBuffer = new ComputeBuffer(m_Resolution * m_Resolution, 3 * 4);

        //找到ComputerShader中名为WaveKernel的方法
        m_KernelIndex = m_ComputeShader.FindKernel("WaveKernel");
    }

    private void OnDisable()
    {
        //释放PositionBuffer
        m_PositionsBuffer.Release();
        m_PositionsBuffer = null;
    }

    private void Update()
    {
        //刷新点位置
        UpdatePointsPosition();
    }

    private void UpdatePointsPosition()
    {
        //设置ComputerShader参数值
        m_ComputeShader.SetInt(resolutionId, m_Resolution);
        m_ComputeShader.SetFloat(stepId, m_StepSize);
        m_ComputeShader.SetFloat(timeId, Time.time);
        //设置ComputerShader中的Buffer
        m_ComputeShader.SetBuffer(m_KernelIndex, positionsId, m_PositionsBuffer);

        //计算所需的线程组数量，内核程序调用一次只处理一个元素，CS中[numthreads(8,8,1)]表示一个线程组有8*8=64个线程
        //则处理所有的元素需要(m_Resolution*m_Resolution)/(8*8)个线程组, 则单个维度的大小为 m_Resolution / 8
        int groups = Mathf.CeilToInt(m_Resolution / 8f);

        //激活ComputerShader中kernel方法
        m_ComputeShader.Dispatch(m_KernelIndex, groups, groups, 1);

        //设置渲染shader中的参数
        m_Material.SetBuffer(positionsId, m_PositionsBuffer);
        m_Material.SetFloat(stepId, m_StepSize);

        //进行GPU-Instancing绘制
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + m_StepSize));
        Graphics.DrawMeshInstancedProcedural(
            m_Mesh, 0, m_Material, bounds, m_Resolution * m_Resolution
        );
    }
}
