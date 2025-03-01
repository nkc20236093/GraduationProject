using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionPostEffect : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite[] colorImage;
    [SerializeField] private Material postEffectMaterial; // 画面遷移ポストエフェクトのマテリアル
    [SerializeField] private float transitionTime = 2f; // 画面遷移の時間
    private readonly int _progressId = Shader.PropertyToID("_Progress"); // シェーダープロパティのReference名

    [SerializeField] private Text resultText;
    /// <summary>
    /// 死亡、クリア時の画面遷移
    /// </summary>
    public IEnumerator Transition(int num, string sceneName, string text)
    {
        image.sprite = colorImage[num];
        image.enabled = true;
        resultText.text = text;
        float t = 0f;
        while (t < transitionTime)
        {
            float progress = t / transitionTime;

            // シェーダーの_Progressに値を設定
            postEffectMaterial.SetFloat(_progressId, progress);
            yield return null;

            t += Time.deltaTime;
        }
        float a_color = 0;
        while (resultText.color.a < 1)
        {
            resultText.color = new Color(resultText.color.r, resultText.color.b, resultText.color.g, a_color);
            a_color += Time.deltaTime / 5;
            yield return null;
        }
        postEffectMaterial.SetFloat(_progressId, 1f);
        SceneManager.LoadScene(sceneName);
    }
}
