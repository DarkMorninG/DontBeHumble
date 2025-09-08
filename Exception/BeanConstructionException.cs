using System;

namespace DBH.Exception {
    public class BeanConstructionException : System.Exception {
        public BeanConstructionException(string message) : base(message) {
        }public BeanConstructionException(string message, Type missingTypes) : base(message) {
        }
    }
}