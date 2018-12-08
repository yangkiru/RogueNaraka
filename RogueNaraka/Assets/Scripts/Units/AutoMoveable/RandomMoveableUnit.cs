using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.AutoMoveableUnit
{
    public class RandomMoveableUnit : AutoMoveableUnit
    {
        float distance;

        public override void Init(UnitData data)
        {
            base.Init(data);
            distance = data.moveDistance;
        }

        protected override void AutoMove()
        {
            Vector2 rnd = new Vector2(Random.Range(-distance, distance), Random.Range(-distance, distance));
            moveable.Move(rnd);
        }
    }
}