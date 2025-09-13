using System;

namespace DBH.Injection.dto {
    public class AfterInjectionDto {
        private Type _type;
        private Action<object> callback;

        public AfterInjectionDto(Type type, Action<object> callback) {
            _type = type;
            this.callback = callback;
        }

        public Type Type => _type;

        public Action<object> Callback => callback;
    }
}