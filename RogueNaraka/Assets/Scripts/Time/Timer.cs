using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.TimeScripts {
    public class Timer {
        private List<IReceiverFromTimer> receiverList = new List<IReceiverFromTimer>();

        private bool isEnded;
        public bool IsEnded { get { return isEnded; } }

        private DateTime endTime;
        public DateTime EndTime { get { return this.endTime; } }

        internal Timer(DateTime _endTime) {
            this.isEnded = false;
            this.endTime = _endTime;
        }

        public void AddReceiver(IReceiverFromTimer _receiver) {
            this.receiverList.Add(_receiver);
        }

        public TimeSpan GetRemainTime() {
            return this.endTime - DateTime.Now;
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