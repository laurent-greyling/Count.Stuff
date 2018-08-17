# Counting instances of agents
This project is intended to call and navigate an api and count the instances of agents with specific search criteria(s).

The project has the following components, each performing a specific function to get to the end result.

## Android App
Android (Count.Stuff): This will initialise the entire process by sending a message to a storage queue. This message is structured as follows:

```
{
    "ProcessId":"<ProcessId>",
    "MessageType":"<MessageType>",
    "ObjectNumber":"<Object/Page number>"
}
```

- __ProcessId__: This is a unique ID that will be sent along inorder to identify the specific process. This is mostly a GUID, but if you use your own tool you can send any random id along.
- __MessageType__: This is the message that will determine what handler should be called by the `Azure Function`. There are currently only two types `BeginProcess`, `SaveDetails`. The Android app will only use `BeginProcess`
- __ObjectNumber__: This will almost always be 1, but if you want the api to start from a different page/object change this and it will start from that point in the api - but please note, if you do this counts will not match up and the client will give error stating that the counts may be off.

Once the background process has finished the results can be viewed under that specific process.

### Navigate the app
- start a process by clicking on the `+` symbol in the top right corner
- a message will be sent to the function app, the function app might take a bit to start if it is asleep. This will mean that the `progress` of this process is not logged yet.
- once the message is beign processed it will take a few minutes to complete. Press the refresh icon in the top right to refresh. Once the process is completed, press the button for the results you want to view.

![progress](https://user-images.githubusercontent.com/17876815/44250899-a4942880-a1f6-11e8-8039-8fd5f360a3bc.gif)

## Azure Function
Azure Function (Count.Functions) - This is the background task which will iterate over your api and get all instances of an agent with specified criteria.

The message received from the client app will be processed and the function will start calling the api as many times as it is needed till end.

For the app to know what resource or api is needed you need the following settings (if you test locally you need to create a local.settings.json file)

```
{
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "<Storage account connection string>",
        "AzureWebJobsDashboard": "<Storage account connection string>",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "ApiBaseUrl": "<Base URL for the api untill 'json/'>", //JSON is the expected data type for this app, xml will fail the app
        "ApiKey": "<api key>"
      }
}

```

As the api calls are processed a progress counter will be kept and indicate once the process is done in a table called `progress`. This table contains the following information:

- PartitionKey - a constant called `CountProgress`
- Rowkey - the `ProcessId` sent by the client
- GardenProgress - progress of the search criteria for agents with a garden
- NumberOfGardenObjects - number of objects with garden search criteria as given by the api
- NormalProgress - progress with no search criteria added. Overall agent counts
- NumberOfNormalObjects - number of objects as given by the api
- IsGardenSearchDone - a bool value calculated by comparing the progress with the object number in api
- IsNormalSearchDonea -  bool value calculated by comparing the progress with the object number in api
- InErrorState - if any errors were thrown, this bool value will be set to true and client will warn that counts might be incorrect.

As the Function iterate over the api it will keep record of each agent in a table called agents.

This table contain the following info:
- PartitionKey - `ProcessId`
- RowKey - `AgentId`
- AgentName - Name of agent
- GardenCount - How many times this agent appeared when garden search criteria was called
- NormalCount - Overall times this agent appeared

## Setup needed
This app expect a few things to be in place before you can use it. As this is an Azure app, it expects some azure services to be in place. These were manually created for this excersise, but usually these resources will be setup by your ARM template as part of your deployment process.

Resources that need to be in place:

- Resource Group:
    - Storage account with queue and tables
    - queue named `management`
    - tables
        - `agents`
        - `progress`
- The resource group is also the same resource into which you will deploy the function

