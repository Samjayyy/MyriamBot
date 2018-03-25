using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyriamBot
{
    public interface IFaceApiHelper
    {
        Task<Face[]> DetectAsync(Stream stream);
        Task<IdentifyResult[]> IdentifyAsync(Guid[] faceIds);
        Task<Person> GetPersonAsync(Guid id);
        Task<CreatePersonResult> CreatePersonAsync(string name);
        Task<AddPersistedFaceResult> AddFaceImageAsync(Guid id, Stream stream);
        Task<PersonGroup> GetPersonGroupAsync();
        Task<bool> StartTrainingAsync();
        Task RemovePersonAsync(Guid id);

    }
}
