using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyriamBot.Conversation
{
    public class WelcomeConversation : ConfirmationConversation
    {
        private Guid[] _faceIds;
        private static string[] must = new[] { "hey", "hello", "hi", "hallo", "goodmorning", "goodday" };
        private static string[] can = new[] { "myriam", "bot", "you", "miss" };

        public WelcomeConversation(IMainWindow window) : base(window)
        {
        }

        public override double IntentScore(string msg, HashSet<string> keywords)
        {
            int cnt = must.Count(kw => keywords.Contains(kw));
            if (cnt == 0)
            {
                return 0d;
            }
            cnt += can.Count(kw => keywords.Contains(kw));
            return 1D * cnt / keywords.Count;
        }

        public override async Task<AbstractConversationState> Start()
        {
            if (_window.ActivePerson != null)
            {
                _window.ReplyAsBot($"Hey {_window.ActivePerson.Name}");
                return await new StartConversation(_window).Start();
            }
            var group = await _window.FaceApiHelper.GetPersonGroupAsync();
            _window.ReplyAsBot($"Myriam here, I'm responsible for group {group.Name}.");
            _window.ReplyAsBot($"Let's have a look who you are..");
            return await HandleRecognition();
        }

        protected override string ConfirmationQuestion => "Shall I try again to recognize your face?";
        protected override string ConfirmationQuestionDoubleCheck => "Didn't get that, can I recheck your face, yes or no?";
        protected override async Task<AbstractConversationState> HandleConfirmYes()
        {
            _window.ReplyAsBot("Ok I'll have another look..");
            return await HandleRecognition();
        }

        protected override async Task<AbstractConversationState> HandleConfirmNo()
        {
            _window.ReplyAsBot("Ok no problem, bye..");
            return await new StartConversation(_window).Start();
        }

        private async Task<AbstractConversationState> HandleRecognition()
        {
            const int FACE_ATTEMPTS = 2;
            _faceIds = new Guid[FACE_ATTEMPTS];
            for (int i = 0; i < FACE_ATTEMPTS; i++)
            {
                var pathToPic = _window.TakePicture();
                using (var stream = File.OpenRead(pathToPic))
                {
                    var persons = await _window.FaceApiHelper.DetectAsync(stream);
                    if (persons.Length == 0)
                    {
                        _window.ReplyAsBot($"I can't see any faces on the picture");
                        _window.ReplyAsBot($"Can you make sure that you are close and clear before the camera please");
                        return await base.Start();
                    }
                    else if (persons.Length == 1)
                    {
                        _faceIds[i] = persons[0].FaceId;
                    }
                    else
                    {
                        _window.ReplyAsBot($"I see multiple people on the camera, can the other person please step away from the camera");
                        _window.ReplyAsBot($"let me know when you're ready..");
                        return await base.Start();
                    }
                }
                File.Move(pathToPic, Path.Combine(App.PIC_DIR, $"{_faceIds[i]}.jpg"));
            }
            var results = await _window.FaceApiHelper.IdentifyAsync(_faceIds);
            // find best candidate
            Candidate best = null;
            if (results == null)
            {
                _window.ReplyAsBot($"You must be the first person ever talking to me");
            }
            else
            {
                foreach (var res in results)
                {
                    foreach (var cand in res.Candidates)
                    {
                        if (best == null || best.Confidence < cand.Confidence)
                        {
                            best = cand;
                        }
                    }
                }
            }
            if (best == null)
            {
                _window.ReplyAsBot("Looks like I don't know you yet or I just wasn't able to recognize you.");
                return await new WelcomeConfirmNew(_window, _faceIds).Start();
            }
            var person = await _window.FaceApiHelper.GetPersonAsync(best.PersonId);
            return await new WelcomeConfirm(_window, _faceIds, person).Start();
        }
    }
}
