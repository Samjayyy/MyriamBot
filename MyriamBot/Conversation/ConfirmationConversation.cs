using MyriamBot.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public abstract class ConfirmationConversation : AbstractConversation
    {
        public ConfirmationConversation(IMainWindow window) : base(window)
        {
        }

        public override async Task<AbstractConversation> Start()
        {
            _window.ReplyAsBot(ConfirmationQuestion);
            return this;
        }

        public override async Task<AbstractConversation> HandleUserInput(string msg)
        {
            var set = msg.ParseKeywords();
            var yes = new[] { "yes", "ya", "yeah", "y", "yep", "ja", "jep", "jop", "please", "ok" };
            if (yes.Any(kw => set.Contains(kw)))
            {
                return await HandleConfirmYes();
            }
            var no = new[] { "no", "nope", "nop", "nein", "nee", "n", "nevermind", "not" };
            if (no.Any(kw => set.Contains(kw)))
            {
                return await HandleConfirmNo();
            }
            _window.ReplyAsBot(ConfirmationQuestionDoubleCheck);
            return this;
        }

        protected abstract string ConfirmationQuestion { get; }
        protected abstract string ConfirmationQuestionDoubleCheck { get; }
        protected abstract Task<AbstractConversation> HandleConfirmYes();
        protected abstract Task<AbstractConversation> HandleConfirmNo();
    }
}
