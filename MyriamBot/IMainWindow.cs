using Microsoft.ProjectOxford.Face.Contract;
using System.IO;

namespace MyriamBot.Conversation
{
    public interface IMainWindow
    {
        IFaceApiHelper FaceApiHelper { get; }
        Person ActivePerson { get; set; }

        void ReplyAsBot(string msg);
        string TakePicture();
    }
}
