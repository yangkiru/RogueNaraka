using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.SkillScripts
{
    public abstract class Skill : MonoBehaviour
    {
        public SkillData data { get { return _data; } }
        SkillData _data;

        public void Init(SkillData dt)
        {
            _data = (SkillData)dt.Clone();
        }

        public ValueData GetValue(SKILL_VALUE name)
        {
            for(int i = 0; i < _data.values.Length; i++)
            {
                if (name == _data.values[i].name)
                    return _data.values[i];
            }
            return null;
        }

        public EffectData GetEffect(EFFECT type)
        {
            for (int i = 0; i < _data.effects.Length; i++)
            {
                if (type == _data.effects[i].type)
                    return _data.effects[i];
            }
            return null;
        }

        public void LevelUp(int amount)
        {
            data.level += amount;
            for (int i = 0; i < amount; i++)
            {
                data.manaCost += data.levelUp.manaCost;
                data.size += data.levelUp.size;
                for (int j = 0; j < data.levelUp.values.Length; j++)
                {
                    ValueData value = GetValue(data.levelUp.values[j].name);
                    if(value != null)
                        value.value += data.levelUp.values[j].value;
                }

                for (int j = 0; j < data.levelUp.effects.Length; j++)
                {
                    EffectData effect = GetEffect(data.levelUp.effects[j].type);
                    if (effect != null)
                    {
                        effect.time += data.levelUp.effects[j].time;
                        effect.value += data.levelUp.effects[j].value;
                    }
                }
            }
        }

        public abstract void Use(Vector3 mp);
    }
}