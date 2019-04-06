using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.TheBackendScripts {
    public class PushSoulReward : PushEvent {
        public override bool CheckAcceptable() {
            return false;
        }

        public override void AcceptReward() {
            Debug.LogError(string.Format("Reward Soul! : {0} souls"));
        }
    }
}
