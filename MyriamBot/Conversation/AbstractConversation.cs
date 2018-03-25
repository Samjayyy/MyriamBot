using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public abstract class AbstractConversationState
    {
        protected IMainWindow _window;

        public AbstractConversationState(IMainWindow window)
        {
            _window = window;
        }
        public virtual async Task<AbstractConversationState> Start()
        {
            return this;
        }
        public abstract Task<AbstractConversationState> HandleUserInput(string msg);

        public virtual double IntentScore(string msg, HashSet<string> keywords)
        {
            throw new NotImplementedException();
        }
    }
}
