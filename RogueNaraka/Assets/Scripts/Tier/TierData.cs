using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.TierScripts {
    public enum TIER_TYPE {
        Iron,
        Bronze,
        Silver,
        Gold,
        Platinum,
        Diamond,
        Challenger,
        END
    }

    [Serializable]
    public struct TierData {
        public TIER_TYPE type;
        public int tier_num;
        public float requiredRankingPercent;
        public Sprite emblem;
    }
}