using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.AzureStorage
{
    public class TodoRepository : ITodoRepository
    {
        public TodoRepository(AzureStorageOptions options)
        {
            Options = options ?? throw new ArgumentNullException();

            BlobContainer = CloudStorageAccount.Parse(Options.ConnectionString)
                                                .CreateCloudBlobClient()
                                                .GetContainerReference(Options.TodoContainer);
        }


        public async Task<IEnumerable<Todo>> GetTodosAsync(string userId)
        {
            var blob = await BlobContainer.GetBlobReferenceFromServerAsync(userId);

            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);
                return JsonConvert.DeserializeObject<Todo[]>(Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        public async Task<bool> PersistTodosAsync(string userId, IEnumerable<Todo> todos)
        {
            if (todos == null || !todos.Any()) { return false; }
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(todos.ToArray()))))
            {
                CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(userId);
                await blockBlob.UploadFromStreamAsync(stream);
                return true;
            }
        }


        private readonly CloudBlobContainer BlobContainer;
        private readonly AzureStorageOptions Options;
    }
}
