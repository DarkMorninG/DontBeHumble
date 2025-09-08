using UnityEditor;
using UnityEngine;
using Vault.BetterCoroutine;

namespace Dont_Be_Humble.Exception {
    public class InjectionContextMissing : System.Exception {
        public InjectionContextMissing(string message) : base(message) {
            UnityThread.executeInUpdate(Debug.Break);
        }
    }
}