using System.Reflection;

namespace Injection.dto {
    public class PostConstructValue {
        private readonly int _priority;
        private readonly MethodInfo _methodToInvoke;
        private readonly object _objectThatHasMethodToInvoke;

        public PostConstructValue(int priority, MethodInfo methodToInvoke, object objectThatHasMethodToInvoke) {
            _priority = priority;
            _methodToInvoke = methodToInvoke;
            _objectThatHasMethodToInvoke = objectThatHasMethodToInvoke;
        }

        public int Priority => _priority;

        public MethodInfo MethodToInvoke => _methodToInvoke;

        public object ObjectThatHasMethodToInvoke => _objectThatHasMethodToInvoke;
    }
}