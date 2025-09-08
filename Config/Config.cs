using System;
using System.Collections.Generic;

namespace Dont_Be_Humble.Config {
    [Serializable]
    public class Config {
        public List<string> AssemblysToScan { get; set; }
        
    }
}