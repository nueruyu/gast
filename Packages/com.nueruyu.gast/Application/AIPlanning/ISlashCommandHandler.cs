using System.Threading;
using System.Threading.Tasks;

namespace Gast.Application.AIPlanning
{
    public interface ISlashCommandHandler
    {
        /// <summary>
        /// The prefix that triggers this handler, e.g. "/story".
        /// Must not include a trailing space.
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Handles the command. <paramref name="instruction"/> is the text after the prefix and its trailing space.
        /// </summary>
        ValueTask<CommandAIResult> HandleAsync(string instruction, CancellationToken cancellationToken);
    }
}
