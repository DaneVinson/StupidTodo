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
    public class TodosBlobRepository : ITodoRepository
    {
        public TodosBlobRepository(AzureStorageOptions options)
        {
            Options = options ?? throw new ArgumentNullException();

            BlobContainer = CloudStorageAccount.Parse(Options.ConnectionString)
                                                .CreateCloudBlobClient()
                                                .GetContainerReference(Options.TodoContainer);
        }


        public Task<IEnumerable<Todo>> AddTodosAsync(IEnumerable<Todo> todo)
        {
            throw new NotImplementedException($"{nameof(ITodoRepository)}.{nameof(AddTodosAsync)} implementation excluded from {nameof(TodosBlobRepository)}");
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

        public Task<bool> DeleteTodoAsync(string userId, string id)
        {
            throw new NotImplementedException($"{nameof(ITodoRepository)}.{nameof(DeleteTodoAsync)} implementation excluded from {nameof(TodosBlobRepository)}");
        }

        public async Task<IEnumerable<Todo>> UpdateTodosAsync(IEnumerable<Todo> todos)
        {
            if (todos == null || !todos.Any()) { return null; }
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(todos.ToArray()))))
            {
                CloudBlockBlob blockBlob = BlobContainer.GetBlockBlobReference(todos.First().UserId);
                await blockBlob.UploadFromStreamAsync(stream);
                return todos;
            }
        }


        private readonly CloudBlobContainer BlobContainer;
        private readonly AzureStorageOptions Options;
    }
}
