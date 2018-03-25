using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public class WelcomeConfirm : ConfirmationConversation
    {
        private readonly Guid[] _faceIds;
        private readonly Person _person;
        public WelcomeConfirm(IMainWindow window, Guid[] faceIds, Person person) : base(window)
        {
            _faceIds = faceIds;
            _person = person;
        }
        protected override string ConfirmationQuestion => $"You are {_person.Name}, right?";

        protected override string ConfirmationQuestionDoubleCheck => $"Sorry, I didn't get that. Are you {_person.Name}, yes or no?";

        protected override async Task<AbstractConversationState> HandleConfirmNo()
        {
            _window.ReplyAsBot($"Hmm ok, my mistake.");
            return await new WelcomeConfirmNew(_window, _faceIds).Start();
        }
        protected override async Task<AbstractConversationState> HandleConfirmYes()
        {
            _window.ReplyAsBot($"Great! What can I do for you?");
            _window.ActivePerson = _person;
            return await new StartConversation(_window).Start();
        }
    }
}
