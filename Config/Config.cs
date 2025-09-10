using System;
using System.Collections.Generic;

namespace DBH.Config {
    [Serializable]
    public class Config {
        private List<string> assemblysToScan = new List<string> {
            "DontBeHumble"
        };

        public List<string> AssemblysToScan {
            get => assemblysToScan;
            set => assemblysToScan = value;
        }
    }
}