using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.TimeScripts {
    public class Timer {
        private bool isEnded;
        private DateTime endTime;
        private List<IReceiverFromTimer> receiverList = new List<IReceiverFromTimer>();

        public bool IsEnded { get { return isEnded; } }

        internal Timer(DateTime _endTime) {
            this.isEnded = false;
            this.endTime = _endTime;
        }

        public void AddReceiver(IReceiverFromTimer _receiver) {
            this.receiverList.Add(_receiver);
        }

        internal void UpdateTimer(DateTime _now) {
            if(!this.isEnded && DateTime.Compare(_now, this.endTime) >= 0) {
                this.isEnded = true;
                for(int i = 0;  i < this.receiverList.Count; ++i) {
                    this.receiverList[i].RecieveFromTimer();
                }
            }
        }
    }

    public interface IReceiverFromTimer {
        void RecieveFromTimer();
    }
}