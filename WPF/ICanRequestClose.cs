using System;

namespace gPadX.WPF {
    interface ICanRequestClose {
        event Action<bool?> CloseRequest;
    }
}
