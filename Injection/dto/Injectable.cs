namespace Dont_Be_Humble.Injection {
    public class Injectable {
        private readonly object _inject;
        private readonly bool _proto;

        public Injectable(object inject, bool proto = false) {
            _inject = inject;
            _proto = proto;
        }

        public object Inject => _inject;

        public bool Proto => _proto;

        public override bool Equals(object obj) {
            if (GetType() != obj.GetType()) return false;
            if (obj is Injectable injectable) {
                return _inject.GetType() == injectable.Inject.GetType() && _proto == injectable._proto;
            }
            return false;
        }
        public override int GetHashCode() {
            return _inject.GetType().GetHashCode();
        }
    }
}