using UnityEngine;
using Vault.BetterCoroutine;

namespace Exception {
    public class InjectionContextMissing : System.Exception {
        public InjectionContextMissing(string message) : base(message) {
            UnityThread.executeInUpdate(Debug.Break);
        }
    }
}