using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public class RandomMoveableUnit : AutoMoveableUnit
    {
        protected override void AutoMove()
        {
            Vector2 rnd = new Vector2(Random.Range(-distance, distance), Random.Range(-distance, distance));
            moveable.Move(rnd);
        }
    }
}