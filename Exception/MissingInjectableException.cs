using System;
using System.Collections.Generic;

namespace Dont_Be_Humble.Exception {
    public class MissingInjectableException : System.Exception {
        public MissingInjectableException(string message, Type missingTypes) : base(message) {
        }
    }
}