using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPotParticle : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem particle;

    ParticleSystem.MainModule main;

    void Awake()
    {
        main = particle.main;
    }

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        Turn();
    }

    void Turn()
    {
        int rndColor = Random.Range(0, (int)STAT.MPREGEN + 1);
        main.startColor = StatOrbManager.instance.statColor[rndColor];
        animator.SetFloat("speed", Random.Range(0.5f, 0.75f));
        transform.rotation = Random.rotation;
    }
}
