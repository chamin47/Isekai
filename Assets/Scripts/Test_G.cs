using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Test_G : MonoBehaviour
{
    private static Material grayscaleMaterial;

    void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;

        // ���̴��� ����� ��Ƽ���� �ʱ�ȭ
        if (grayscaleMaterial == null)
        {
            Shader grayscaleShader = Shader.Find("Custom/Grayscale");
            grayscaleMaterial = new Material(grayscaleShader);
        }
    }

    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (grayscaleMaterial != null)
        {
            // 'EffectAmount' �� ������ �������Ѽ� ������� ��ȯ
            float effectAmount = Mathf.PingPong(Time.time / 2f, 1f);  // ���÷� �ð��� ������ ���� ���ϴ� ��

            grayscaleMaterial.SetFloat("_EffectAmount", effectAmount);

            // CommandBuffer�� �������� ȭ���� ó��
            CommandBuffer cmd = CommandBufferPool.Get("ApplyGrayscaleEffect");
            cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, grayscaleMaterial);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
