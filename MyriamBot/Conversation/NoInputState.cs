using System;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public abstract class NoInputState : AbstractConversation
    {
        public NoInputState(IMainWindow window) : base(window)
        {
        }
        public override async Task<AbstractConversation> HandleUserInput(string msg)
        {
            throw new InvalidOperationException($"This state is not able to handle user input and is expected to pass through another state on initializing.");
        }
    }
}
