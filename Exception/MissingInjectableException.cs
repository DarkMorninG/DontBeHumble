using System;

namespace DBH.Exception {
    public class MissingInjectableException : System.Exception {
        public MissingInjectableException(string message, Type missingTypes) : base(message) {
        }
    }
}