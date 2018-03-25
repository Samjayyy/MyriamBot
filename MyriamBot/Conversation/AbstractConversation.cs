using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public abstract class AbstractConversation
    {
        protected IMainWindow _window;

        public AbstractConversation(IMainWindow window)
        {
            _window = window;
        }
        public virtual async Task<AbstractConversation> Start()
        {
            return this;
        }
        public abstract Task<AbstractConversation> HandleUserInput(string msg);

        public virtual double IntentScore(string msg, HashSet<string> keywords)
        {
            throw new NotImplementedException();
        }
    }
}
