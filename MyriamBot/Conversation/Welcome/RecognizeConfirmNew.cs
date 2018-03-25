using System;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public class RecognizeConfirmNew : ConfirmationConversation
    {
        private readonly Guid[] _faceIds;
        public RecognizeConfirmNew(IMainWindow window, Guid[] faceIds) : base(window)
        {
            _faceIds = faceIds;
        }
        protected override string ConfirmationQuestion => $"Is it ok for you that I try to remember your face as a new person?";

        protected override string ConfirmationQuestionDoubleCheck => $"Sorry, I didn't get that. You want me to remember your face, yes or no?";

        protected override async Task<AbstractConversation> HandleConfirmNo()
        {
            _window.ReplyAsBot($"Ok, no problem. Maybe another time.");
            return await new StartConversation(_window).Start();
        }
        protected override async Task<AbstractConversation> HandleConfirmYes()
        {
            return await new RecognizeNew(_window, _faceIds).Start();
        }
    }
}
