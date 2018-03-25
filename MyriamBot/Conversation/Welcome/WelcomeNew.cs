using MyriamBot.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public class WelcomeNew : AbstractConversationState
    {
        private readonly Guid[] _faceIds;
        public WelcomeNew(IMainWindow window, Guid[] faceIds) : base(window)
        {
            _faceIds = faceIds;
        }

        public override async Task<AbstractConversationState> Start()
        {
            _window.ReplyAsBot($"Ok, What's your name please?");
            return this;
        }

        public override async Task<AbstractConversationState> HandleUserInput(string msg)
        {
            var name = ParseName(msg);
            if (string.IsNullOrWhiteSpace(name))
            {
                return this;
            }
            _window.ReplyAsBot($"Ok I'll try to memorize you from now on as {name}");
            var create = await _window.FaceApiHelper.CreatePersonAsync(name);
            foreach (var face in _faceIds)
            {
                using (var stream = File.OpenRead(Path.Combine(App.PIC_DIR, $"{face}.jpg")))
                {
                    await _window.FaceApiHelper.AddFaceImageAsync(create.PersonId, stream);
                }
                await _window.FaceApiHelper.StartTrainingAsync();
            }
            _window.ReplyAsBot($"What can I do for you?");
            return await new StartConversation(_window).Start();
        }

        private string ParseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            name = name.Trim();
            if (!name.Contains(" "))
            {
                return name; // just a name
            }
            const string nameIs = "name is ";
            int ix = name.ToLower().IndexOf(nameIs);
            if (ix >= 0)
            {
                return name.Substring(ix + nameIs.Length).CleanSeperated();
            }
            return name;
        }
    }
}
