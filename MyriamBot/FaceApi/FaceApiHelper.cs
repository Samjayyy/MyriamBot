using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace MyriamBot
{
    public class FaceApiHelper : IFaceApiHelper
    {
        private static readonly string PersonGroupKey = ConfigurationManager.AppSettings["PersonGroupKey"]; // just for a demo, one person group
        private IFaceServiceClient _faceClient { get; set; }
        private PersonGroup _personGroup;

        public FaceApiHelper(IFaceServiceClient faceServiceClient)
        {
            _faceClient = faceServiceClient;
        }

        public async Task<Face[]> DetectAsync(Stream stream)
        {
            return await _faceClient.DetectAsync(stream);
        }

        public async Task<IdentifyResult[]> IdentifyAsync(Guid[] faceIds)
        {
            try
            {
                return await _faceClient.IdentifyAsync(faceIds, personGroupId: PersonGroupKey, largePersonGroupId: null);
            }
            catch (FaceAPIException ex) when ("PersonGroupNotTrained".Equals(ex.ErrorCode))
            {
                var success = await StartTrainingAsync();
                if (!success)
                {
                    return null;
                }
                return await IdentifyAsync(faceIds); // try again after training
            }
        }

        public async Task RemovePersonAsync(Guid id)
        {
            await _faceClient.DeletePersonFromPersonGroupAsync(PersonGroupKey, id);
        }

        public async Task<Person> GetPersonAsync(Guid id)
        {
            return await _faceClient.GetPersonInPersonGroupAsync(PersonGroupKey, id);
        }

        public async Task<CreatePersonResult> CreatePersonAsync(string name)
        {
            return await _faceClient.CreatePersonInPersonGroupAsync(PersonGroupKey, name);
        }

        public async Task<AddPersistedFaceResult> AddFaceImageAsync(Guid id, Stream stream)
        {
            return await _faceClient.AddPersonFaceInPersonGroupAsync(PersonGroupKey, id, stream);
        }

        public async Task<bool> StartTrainingAsync()
        {
            TrainingStatus trainingStatus = null;
            await _faceClient.TrainPersonGroupAsync(PersonGroupKey);
            while (true)
            {
                trainingStatus = await _faceClient.GetPersonGroupTrainingStatusAsync(PersonGroupKey);
                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }
                await Task.Delay(3000);
            }
            return trainingStatus.Status == Status.Succeeded;
        }

        public async Task<PersonGroup> GetPersonGroupAsync()
        {
            if (_personGroup == null)
            {
                try
                {
                    _personGroup = await _faceClient.GetPersonGroupAsync(PersonGroupKey);
                }
                catch (FaceAPIException)
                {
                    // spijtig, niet gevonden
                }
                if (_personGroup == null)
                {
                    await _faceClient.CreatePersonGroupAsync(PersonGroupKey, "CapEmployees");
                    await _faceClient.GetPersonGroupAsync(PersonGroupKey); // make sure the set is trained, even at the start
                    return await GetPersonGroupAsync();
                }
            }
            return _personGroup;
        }
    }
}
