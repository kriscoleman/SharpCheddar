# Contributor Documentation

## Dependencies
If you are developing for this project you will need to have dotnetcore sdk, and Visual Studio. You will also need Docker to get the containers running for the various db types. You will also want to have GitVision installed to tag version updates. 

## Building and Running the App
Being that this is a .Net project, it will be easiest to develop this project in the latest version of Visual Studio or Visual Studio Code.

### With Visual Studio:

1) Pull down the code from https://github.com/kriscoleman/SharpCheddar.git
2) do a `dotnet restore`
3) build and run! The Tests project should be the startup project, and you will need to have certain dependencies installed on your machine. 

### With Visual Studio Code:

I'm not using VSCode to dev this project right now and will have to add those details later. I suspect it will be much the same as above, but you'll need all of the csharp plugins and the dependencies installed on the machine, then create a build def and you can build and run. 

## How to Contribute
Always follow GitFlow when working on git projects.
From github, clone the project. Pull from the develop branch, and create a new branch for your task.

create a feature/ branch if working on a feature
create a bug/ branch if working on a bug
create a fix/ branch if working on a fix/feedback that doesn't fit into feature or bug
Make your changes and commit often to your new branch.

Please utilize TDD when contributing to this project. Please note that your pull requests will be expected to have tests passing, and if you added a feature, new tests to validate it. If you fix a bug that wasn't captured in a test, then please write a test for it.

When you are done with your changes, run any tests that may be included in the solution and make sure they all pass.

Finally, create a new pull request in github to merge your branch into develop.

Wait for Peer Review. Upon Peer Review completion, the merge can be completed.

Later, before publishing new changes, a pull request from develop to master should be created for a new release.

## Trello Board
The project is managed on a public trello board. Feel free to join up and contribute, or just watch the progress! https://trello.com/b/D3uSgguS/sharpcheddar%F0%9F%A7%80

## Deployment 
I'm working on CI/CD to publish the project as a nuget package automatically on pull requests to `master`. I will update the documentation upon completion.

