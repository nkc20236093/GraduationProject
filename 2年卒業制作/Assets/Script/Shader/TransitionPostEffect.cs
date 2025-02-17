using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPostEffect : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite[] colorImage;
    [SerializeField] private Material postEffectMaterial; // ��ʑJ�ڃ|�X�g�G�t�F�N�g�̃}�e���A��
    [SerializeField] private float transitionTime = 2f; // ��ʑJ�ڂ̎���
    private readonly int _progressId = Shader.PropertyToID("_Progress"); // �V�F�[�_�[�v���p�e�B��Reference��

    [SerializeField] private Text resultText;

    /// <summary>
    /// ��ʑJ��
    /// </summary>
    public IEnumerator Transition(int num)
    {
        image.sprite = colorImage[num];
        image.enabled = true;
        float t = 0f;
        while (t < transitionTime)
        {
            float progress = t / transitionTime;

            // �V�F�[�_�[��_Progress�ɒl��ݒ�
            postEffectMaterial.SetFloat(_progressId, progress);
            yield return null;

            t += Time.deltaTime;
        }
        float a_color = 0;
        while (a_color < 1)
        {
            resultText.color = new Color(resultText.color.r, resultText.color.b, resultText.color.g, a_color);
            a_color += Time.deltaTime / 5;
        }

        postEffectMaterial.SetFloat(_progressId, 1f);
    }
}
