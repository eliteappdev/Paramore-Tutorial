using System;

namespace Tasks.Ports.Commands
{
    public abstract class Command
    {
        protected Command(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }

        public abstract void Execute();
    }
}