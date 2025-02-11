using System;

namespace Nahoum.UnityJSInterop.Tests
{
    /// <summary>
    /// Test exposing events
    /// </summary>
    public class TestEvents
    {
        [ExposeWeb]
        public static TestEvents GetNewInstance() => new TestEvents();

        // Try the "property style"
        [ExposeWeb]
        public event Action<string> TestEventStringAuto = delegate { };

        // Try the "manual style" with add and remove
        public event Action<string> _eventManual = delegate { };
        public event Action<string> TestEventStringManual { [ExposeWeb] add { _eventManual += value; } [ExposeWeb] remove { _eventManual -= value; } }

        // Triggers the event for testing purposes
        [ExposeWeb]
        public void InvokeEvent(string value)
        {
            TestEventStringAuto.Invoke(value);
            _eventManual.Invoke(value);
        }
    }
}