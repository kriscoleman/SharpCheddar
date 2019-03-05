# SharpCheddar 🧀
 [![pipeline status](https://gitlab.com/user/userproject/badges/master/pipeline.svg)](https://gitlab.com/kriscoleman/SharpCheddar/commits/master)
 
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
