using UnityEngine;
using Vault.BetterCoroutine;

namespace DBH.Exception {
    public class InjectionContextMissing : System.Exception {
        public InjectionContextMissing(string message) : base(message) {
            UnityThread.executeInUpdate(Debug.Break);
        }
    }
}