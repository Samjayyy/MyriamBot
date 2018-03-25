using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public class Goodbye : NoInputState
    {
        public Goodbye(IMainWindow window) : base(window)
        {
        }

        public override double IntentScore(string msg, HashSet<string> keywords)
        {
            var must = new[] { "bye", "goodbye", "ciao", "adios", "cu", "later" };
            int cnt = must.Count(kw => keywords.Contains(kw));
            if (cnt == 0)
            {
                return 0d;
            }
            var can = new[] { "myriam", "bot", "you", "miss" };
            cnt += can.Count(kw => keywords.Contains(kw));
            return 1D * cnt / keywords.Count;
        }

        public override async Task<AbstractConversation> Start()
        {
            if (_window.ActivePerson == null)
            {
                _window.ReplyAsBot($"Goodbye stranger");
            }
            else
            {
                _window.ReplyAsBot($"Bye {_window.ActivePerson.Name}!");
                _window.ActivePerson = null;
            }
            return await new StartConversation(_window).Start();
        }
    }
}
