
using System.Collections;
using UnityEngine;
using Cinemachine;

public partial class GameManager : MonoBehaviour {

    public CinemachineVirtualCamera cinemachineVirtualCamera;
    private float shakeTimer;
    private bool isActivate;
    
    public void ShakeCamera(float intensity, float time, float gap){
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (cinemachineBasicMultiChannelPerlin.m_AmplitudeGain <= intensity){
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
                cinemachineBasicMultiChannelPerlin.m_FrequencyGain = gap;
                shakeTimer = time;
                isActivate = true;
                Debug.Log("쉐이크 시작"+intensity + " " + time);
            }
    }

    private void Update(){
        if (shakeTimer > 0)
            shakeTimer -= Time.deltaTime;
        else if(isActivate){
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            isActivate = false;
            Debug.Log("쉐이크 종료");
        }
    }
}