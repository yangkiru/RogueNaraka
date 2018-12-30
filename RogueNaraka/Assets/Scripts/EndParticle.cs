using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndParticle : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        StatOrbManager.instance.endPool.EnqueueObjectPool(gameObject);
    }
}
