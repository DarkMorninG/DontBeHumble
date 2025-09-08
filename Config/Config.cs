using System;
using System.Collections.Generic;

namespace Config {
    [Serializable]
    public class Config {
        public List<string> AssemblysToScan { get; set; }
        
    }
}