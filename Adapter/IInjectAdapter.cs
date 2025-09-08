using System;
using System.Collections.Generic;
using Dont_Be_Humble.Injection;

namespace Dont_Be_Humble.Adapter {
    public interface IInjectAdapter {
        void Inject(InjectionContext injectionContext);
    }
}