using System;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public abstract class NoInputState : AbstractConversationState
    {
        public NoInputState(IMainWindow window) : base(window)
        {
        }
        public override async Task<AbstractConversationState> HandleUserInput(string msg)
        {
            throw new InvalidOperationException($"This state is not able to handle user input and is expected to pass through another state on initializing.");
        }
    }
}
