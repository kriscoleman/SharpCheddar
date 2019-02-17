# SharpCheddar 🧀
A suite of .NetStandard and DotNetCore data repositories

Designed with TDD, bearing the SOLID principles in mind, and implemnting `async/await` patterns

# Why repositories?
These days databases are a dime a dozen. Not only are they plenty, but many of them
have specialized uses. It's becoming more common to use different types of datatypes
for different projects, or sometimes even for different purposes in a single project. 

SharpCheddar lets you abstract out your data layer using a common interface, providing many benefits. Just like cheese, you can put SharpCheddar 🧀 on anything!

- Working with the different database types feels uniform
- It allow for more flexibility, ever wish you could change the underlying data provider of a data type? With SharpCheddar it's a brie-ze!
- Simpler, easier to maintain data access layers!
- Quicker to adopt a multiple databases in a project! 
- Quicker ramp up - just define your models and your database's native config, and start ripping one!

# Consumer Information

## How to use
1) pull down the nuget `SharpCheddar.Core` nuget package. 
2) pull down whatever other nuget packages you might want, like the `SharpCheddar.EntityFrameworkCore` or `SharpCheddar.BrightstarDb` packages
3) make sure you also pull down the nuget packages for the database/orms you are using, too. Individual dependencies should have been downloaded automatically when you pulled down the SharpCheddar packages. 
4) configure your database connections as you normally would. Typically this is done in the `web.config` or `app.config`.
5) define your models
6) typically projects these days implement some kind of Depedency Injection, go-ahead now and register your repository types for your models. 
7) Start deving! 

### Examples
```
// create and insert a new record
var createdOn = DateTime.UtcNow;
var entity = new MyModel {CreatedOn = createdOn};
await _myRepository.InsertOrUpdateAsync(entity);

// get a record by Id
var entityReturned = await _myRepository.GetByIdAsync(entity.Id);

// get records by lambda
var results = await _myRepository.Get(x => x.Name.Contains("John"));

// get all records back in an `IQueryable`
var query = _myRepository.GetAllAsync();

// update a record 
var updatedDateTime = createdOn.AddYears(7);
entity.CreatedOn = updatedDateTime;

await _myRepository.InsertOrUpdateAsync(entityReturned);

// delete a record
await _myRepository.DeleteByIdAsync(entity.Id);
```


## Best Practices
This project is purposely designed with the SOLID principles in mind.

1) It is recommended to use the Dependancy Inversion principle when using your repositories, that is, you should always try to depend on the common interface instead of the concretions. Let your DI container handle that for you based on your registrations. 
2) However, in some situations during (testing or otherwise), there may be need to break this principle and work directly with the concretion or your underlying database provider. For that purpose, our concretions have been made purposely leaky. It is recommended in these situations to cast the interface as your expected concretion type to use it this way. This should always be avoided unless there is a good reason. 
3) While using these repos may simplify your data acess, you will still want to have a great understanding of the underlying databse layers that you depend on. 
4) You should not use the repositories directly, and you shouldn't expose your underlying dataprovider or any `IQueryables`. Instead, you should wrap your reposotories in logic or service layers, and only return simple `IEnumrable` types from queries (arrays, ienumerables, lists)
5) You should always check if you can use your repository before using it in your unit of work. You should always make your repository ready first by initializing it. You can check if your repository is ready by checking `IsInitialized`.
```
 await _myRepository.InitializeAsync();
 if (!_myRepository.IsInitialized)
 {
    // uh oh, nows your chance to handle this gracefully
 }

 var entity = new MyModel {CreatedOn = DateTime.UtcNow};
 await _myRepository.InsertOrUpdateAsync(entity);
```

## EntityFrameworkCore

### Tips
1) `DbContext.SaveChanges()` is automatically called for you on every call to the repository, so you don't have to call it. 
2) Granular control of the entire transaction (while still making mutiple calls to the repository) is still possible. While breaking IoC, you can cast your repo interface to it's concrete type and use the `_myRepository.BeginTransaction()` method. Ex:
```
 /// <summary>
 /// Tests the specified unit of work inside a transaction which get's rolled back, ensuring the work is Idempotent
 /// </summary>
 /// <param name="unitOfWork">The unit of work.</param>
 private async Task Test(Func<Task> unitOfWork)
 {
     using (var transaction = await (_myRepository as EntityFrameworkCoreRepository<MyModel, int>)?.BeginTransactionAsync(unitOfWork))
         transaction?.Rollback();
 }
```
3) All EFCore features can be used still, including `DataAnnotations`

## More to come:
- MongoDbCore
- BrightstarDb

# Contributor Documentation

## Dependencies
If you are developing for this project you will need to have dotnetcore sdk, and Visual Studio. 

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

