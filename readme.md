## Stupid Todo - azure-storage-table branch
A branch of the Stupid Todo concept application with an Azure Storage Table data repository implementation.

This implementation utilizes [TableEntityAdapter\<T>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.windowsazure.storage.table.tableentityadapter-1?view=azure-dotnet) instead of the more traditional [TableEntity](https://docs.microsoft.com/en-us/dotnet/api/microsoft.windowsazure.storage.table.tableentity?view=azure-dotnet) to persist `Todo` objects.