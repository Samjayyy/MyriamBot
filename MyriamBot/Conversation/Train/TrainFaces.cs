using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public class TrainFaces : NoInputState
    {
        public TrainFaces(IMainWindow window) : base(window)
        {
        }

        public override double IntentScore(string msg, HashSet<string> keywords)
        {
            var must = new[] { "train" };
            int cnt = must.Count(kw => keywords.Contains(kw));
            if (cnt == 0)
            {
                return 0d;
            }
            var can = new[] { "faces", "remembering", "everyone", "all" };
            cnt += can.Count(kw => keywords.Contains(kw));
            return 1D * cnt / keywords.Count;
        }

        public override async Task<AbstractConversationState> Start()
        {
            _window.ReplyAsBot("I'm learning all the names of those who have recently passed by.");
            await _window.FaceApiHelper.StartTrainingAsync();
            return await new StartConversation(_window).Start();
        }
    }
}
