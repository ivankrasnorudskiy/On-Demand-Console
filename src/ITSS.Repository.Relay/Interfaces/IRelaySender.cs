using System.Threading;
using System.Threading.Tasks;

namespace ITSS.Repository.Relay.Interfaces
{
    public interface IRelaySender
    {
        Task<string> SendRequest(string query, CancellationToken ct);
    }
}
