using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class SkillBook : Skill
    {
        SkillData nextData;
        Skill nextSkill = null;
        public override void OnInit()
        {
            GetRandomSkill();
        }
        public override void Use(ref Vector3 mp)
        {
            nextSkill.Use(ref mp);
            GetRandomSkill();
        }

        private void GetRandomSkill(){
            if (nextSkill != null) Destroy(nextSkill);
            int rnd = Random.Range(0, GameDatabase.instance.skills.Length);
            if(rnd == data.id) {
                GetRandomSkill();
                return;
            }
            nextData = Skill.LevelUpData((SkillData)GameDatabase.instance.skills[rnd].Clone(), data.level);
            data.size = nextData.size;
            data.isCircleToPlayer = nextData.isCircleToPlayer;
            string str = string.Format("RogueNaraka.SkillScripts.{0}", nextData.name);
            System.Type type = System.Type.GetType(str);

            nextSkill = gameObject.AddComponent(type) as Skill;
            nextSkill.Init((SkillData)nextData.Clone(), null);
        }

        private void OnDestroy(){
            Destroy(nextSkill);
        }
    }
}