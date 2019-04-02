using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraShake : MonoBehaviour {

    static public CameraShake instance;
    public new Camera camera;
    public bool isShake = true;
    public Button shakeToggleBtn;
    public float time;
    public float power;
    public float speed;
    private Vector3 origin = Vector3.zero;

    Transform cachedTransform;

    private void Awake()
    {
        instance = this;
        isShake = PlayerPrefs.GetInt("isShake") == 0;
        cachedTransform = camera.transform;
        UpdateShakeBtn();
    }

    public void ToggleShake()
    {
        isShake = PlayerPrefs.GetInt("isShake") != 0;
        PlayerPrefs.SetInt("isShake", isShake ? 0 : 1);
        UpdateShakeBtn();
    }
    
    public void UpdateShakeBtn()
    {
        if (!shakeToggleBtn)
            return;
        
        TextMeshProUGUI txt = shakeToggleBtn.GetComponentInChildren<TextMeshProUGUI>();
        
        txt.color = isShake ? shakeToggleBtn.colors.normalColor : shakeToggleBtn.colors.disabledColor;
        shakeToggleBtn.image.color = txt.color;
    }

    public void Shake(float time, float power, float gap, bool isUnscale = false)
    {
        if(isShake)
            StartCoroutine(RandomMove(time, power, gap, isUnscale));
    }

    public void Shake(ShakeData data, bool isUnscale = false)
    {
        if(isShake)
            Shake(data.time, data.power, data.gap, isUnscale);
    }

    private IEnumerator RandomMove(float time, float power, float gap, bool isUnscale = false)
    {
        float t1 = 0, t2 = 0;
        if (gap <= 0)
            gap = 0.001f;
        if(origin == Vector3.zero)
            origin = camera.transform.position;
        while (t1 <= time)
        {
            Vector3 random = new Vector3(Random.Range(-power, power), Random.Range(-power, power), origin.z);
            cachedTransform.position = random;
            
            while (t2 <= gap)
            {
                yield return null;
                t1 += isUnscale ? Time.unscaledDeltaTime : Time.deltaTime;
                t2 += isUnscale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            t2 = 0;

            cachedTransform.position = origin;
        }
    }
}
