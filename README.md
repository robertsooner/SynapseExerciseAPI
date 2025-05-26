# SynapseExerciseAPI

This project is the backend API for communication with the OpenAI platform.  For notes regarding the client project see [Synapse Exercise Client ReadMe](https://github.com/robertsooner/SynapseExerciseClient).

## Running the project

Download the project to your local source location.
From the directory that you downloaded the project to, click on the "SynapseExerciseAPI.sln" file to open the solution in Visual Studio.
You will need to insert your OpenAI API key into the appsettings.json file in the "OpenAIKey" property.
Start the API in Debug mode. The API should be run locally using http (http://localhost:5155). The client is configured to talk to the API at that URL.
Once the server is running, you can spin up the client.

## Exercise Approach

While the exercise could have been completed using strictly client-side UI and direct OpenAI calls, I felt that breaking the OpenAI communication into a separate layer was more "real-world", as this functionality would likely become a piece of a larger API with broader capabilities for a centralized, shared service.

The exercise required only a single POST method to perform the following:
1) Accept and validate the client parameters for the OpenAI request
2) Initialize the Chat Completions object using the supplied parameters
3) Submit the Chat Completions request to OpenAI
4) Validate the response and generate the response object to send back to the client

## Improvements for Future Consideration
1) Modify the input AI request object to allow a variable number of system/user prompts (instead of fixed properties) for more flexibility.
2) Implement OpenTelemetry instrumentation to be able to access more accurate diagnostic data.
3) Implement functionality to persist the results to a repository for further analysis and review.
4) Implement functionality to allow selection of the model to submit the request to.