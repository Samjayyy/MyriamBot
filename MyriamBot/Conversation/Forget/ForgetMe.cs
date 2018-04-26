using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public class ForgetMe : ConfirmationConversation
    {
        public ForgetMe(IMainWindow window) : base(window)
        {
        }

        public override double IntentScore(string msg, HashSet<string> keywords)
        {
            var must = new[] { "forget", "me", "my" };
            int cnt = must.Count(kw => keywords.Contains(kw));
            if (cnt < 2 || !keywords.Contains("forget")) // at least 2 of the 3 and forget must be one of them
            {
                return 0d;
            }
            var can = new[] { "name", "about", "please" };
            cnt += can.Count(kw => keywords.Contains(kw));
            return 1D * cnt / keywords.Count;
        }

        protected override string ConfirmationQuestion => $"Are you sure that I should not remember you anymore, {_window.ActivePerson.Name}?";
        protected override string ConfirmationQuestionDoubleCheck => $"Sorry, I didn't get that. Should I forget about you {_window.ActivePerson.Name}, yes or no?";

        protected override async Task<AbstractConversation> HandleConfirmNo()
        {
            _window.ReplyAsBot($"Ok fine, happy that you want me to remember you.");
            return await new StartConversation(_window).Start();
        }

        protected override async Task<AbstractConversation> HandleConfirmYes()
        {
            _window.ReplyAsBot($"Ok, it was a good time knowing you. Bye bye.");
            await _window.FaceApiHelper.RemovePersonAsync(_window.ActivePerson.PersonId);
            _window.ActivePerson = null;
            await _window.FaceApiHelper.StartTrainingAsync(); // so the person is not found anymore in the cached learning model
            return await new StartConversation(_window).Start();
        }
    }
}
