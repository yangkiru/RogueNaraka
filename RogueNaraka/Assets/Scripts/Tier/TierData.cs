using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.TierScripts {
    public enum TIER_TYPE {
        IRON,
        BRONZE,
        SILVER,
        GOLD,
        PLATINUM,
        DIAMOND,
        CHALLENGER,
        END
    }

    [Serializable]
    public struct TierData {
        public TIER_TYPE type;
        public int tier_num;
        public float requiredRankingPercent;
    }
}