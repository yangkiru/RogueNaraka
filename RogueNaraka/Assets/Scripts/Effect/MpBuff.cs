using UnityEngine;
using System.Collections;
using RogueNaraka.EffectScripts;

public class MpBuff : Effect
{
    public override void Combine(EffectData dt)
    {
        data.time += dt.time;
    }

    public override bool Equal(EffectData dt)
    {
        return dt.value == data.value;
    }

    protected override void OnDestroyEffect()
    {
        target.stat.AddTemp(STAT.MP, -data.value);
    }
    
    protected override void OnInit()
    {
        target.stat.AddTemp(STAT.MP, data.value);
    }
}
