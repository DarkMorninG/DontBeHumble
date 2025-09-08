using System;
using JetBrains.Annotations;

namespace Dont_Be_Humble.Attributes {
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class PostConstruct : Attribute {
        private int _executionOrder;

        public PostConstruct() {
            _executionOrder = 1000;
        }

        public PostConstruct(int executionOrder) {
            _executionOrder = executionOrder;
        }

        public int ExecutionOrder => _executionOrder;
    }
}