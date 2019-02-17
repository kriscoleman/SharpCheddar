namespace SharpCheddar.Core
{
    /// <summary>
    /// The DataModel interface.
    /// At the core of the repository pattern, implements a Id field that can identify an entity.
    /// </summary>
    public interface IDataModel<TKey>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        TKey Id { get; set; }
    }
}