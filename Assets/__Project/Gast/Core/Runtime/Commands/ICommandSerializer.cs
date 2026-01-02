namespace Gast.Core.Commands
{
    /// <summary>
    /// Defines the contract for serializing and deserializing command objects.
    /// </summary>
    public interface ICommandSerializer
    {
        /// <summary>
        /// Serializes any command object into a string format.
        /// </summary>
        /// <param name="command">The command object to serialize (must implement ICommand or ICommand<TResult>).</param>
        /// <returns>A string representation of the command.</returns>
        string Serialize(object command);

        /// <summary>
        /// Deserializes a string into a command object.
        /// </summary>
        /// <param name="data">The string data to deserialize.</param>
        /// <returns>The deserialized command object.</returns>
        object Deserialize(string data);
    }
}
