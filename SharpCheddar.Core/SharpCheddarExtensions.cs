using System.Threading.Tasks;

namespace SharpCheddar.Core
{
    /// <summary>
    /// A place for sharp cheddar extensions
    /// I debated calling this class `extra cheese`
    /// </summary>
    public static class SharpCheddarExtensions
    {

        /// <summary>
        ///     Checks if initialized.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SharpCheddarInitializationException"></exception>
        public static Task CheckIfInitializedAsync<T, TKey>(this IRepository<T, TKey> repo) where T : IDataModel<TKey>
        {
            if (!repo.IsInitialized) throw new SharpCheddarInitializationException();
            return Task.CompletedTask;
        }
    }
}