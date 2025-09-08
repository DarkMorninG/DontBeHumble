using System;
using System.Collections.Generic;

namespace DBH.Config {
    [Serializable]
    public class Config {
        public List<string> AssemblysToScan { get; set; }
        
    }
}