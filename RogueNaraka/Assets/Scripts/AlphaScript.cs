using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaScript : MonoBehaviour
{
    public Image mainImage;
    public TMPro.TextMeshProUGUI mainTxt;
    public Image[] images;
    public TMPro.TextMeshProUGUI[] txts;
    public ParticleSystem[] particles;

    float originAlpha;
    float currentAlpha;
    float lastAlpha = -1;

    List<float> alphas = new List<float>();

    private void Reset()
    {
        mainImage = GetComponent<Image>();
        mainTxt = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Start()
    {
        originAlpha = mainImage ? mainImage.color.a : mainTxt ? mainTxt.color.a : 1;
        for (int i = 0; i < images.Length; i++)
        {
            alphas.Add(images[i].color.a);
        }
        for (int i = 0; i < txts.Length; i++)
            alphas.Add(txts[i].color.a);
        for (int i = 0; i < particles.Length; i++)
            alphas.Add(particles[i].main.startColor.color.a);
    }

    private void Update()
    {
        currentAlpha = mainImage ? mainImage.color.a : mainTxt ? mainTxt.color.a : 1;

        if (lastAlpha != currentAlpha)
        {
            float value = Mathf.InverseLerp(0, originAlpha, currentAlpha);
            for(int i = 0; i < images.Length; i++)
            {
                Color color = images[i].color;
                color.a = alphas[i] * value;
                images[i].color = color;
            }
            for (int i = 0; i < txts.Length; i++)
            {
                Color color = txts[i].color;
                color.a = alphas[images.Length + i] * value;
                txts[i].color = color;
            }
            for (int i = 0; i < particles.Length; i++)
            {
                Color color = particles[i].main.startColor.color;
                color.a = alphas[images.Length + txts.Length + i] * value;
                var main = particles[i].main;
                var startColor = main.startColor;
                startColor.color = color;
                main.startColor = startColor;
            }
            lastAlpha = currentAlpha;
        }
    }
}
