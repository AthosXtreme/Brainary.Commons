namespace Brainary.Commons
{
    using System;

    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T eventData)
        {
            EventData = eventData;
        }

        public T EventData { get; private set; }
    }
}
