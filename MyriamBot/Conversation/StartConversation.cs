using MyriamBot.Extensions;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public class StartConversation : AbstractConversationState
    {
        private AbstractConversationState[] _conversations;
        public StartConversation(IMainWindow window) : base(window)
        {
            _conversations = new AbstractConversationState[]
            {
              new WelcomeConversation(_window)
            , new ForgetMe(_window)
            , new TrainFaces(_window)
            , new Goodbye(_window)
            };
        }

        public override async Task<AbstractConversationState> Start()
        {
            return this;
        }

        public override async Task<AbstractConversationState> HandleUserInput(string msg)
        {
            var keywords = msg.ParseKeywords();
            int best = -1;
            double bestScore = 0d;
            for (int i = 0; i < _conversations.Length; i++)
            {
                double score = _conversations[i].IntentScore(msg, keywords);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = i;
                }
            }
            if (best >= 0)
            {
                return await _conversations[best].Start();
            }
            _window.ReplyAsBot($"If you want something try to talk with keywords, I might understand that even better.");
            return this;
        }


    }
}
