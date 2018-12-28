using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.SkillScripts
{
    public abstract class Skill : MonoBehaviour
    {
        public SkillData data { get { return _data; } }
        [SerializeField]
        SkillData _data;

        SkillGUI gui;

        public void Init(SkillData dt, SkillGUI gui)
        {
            _data = (SkillData)dt.Clone();
            this.gui = gui;
        }

        public ValueData GetValue(Value name)
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

        private void Update()
        {
            if (data.coolTimeLeft > 0)
            {
                data.coolTimeLeft -= Time.deltaTime;
                gui.SyncCoolImg();
                gui.SyncCoolText();
                return;
            }
            else if (data.coolTimeLeft < 0)
            {
                data.coolTimeLeft = 0;
                gui.SyncCoolImg();
                gui.SyncCoolText();
            }
        }

        public abstract void Use(Vector3 mp);
    }
}