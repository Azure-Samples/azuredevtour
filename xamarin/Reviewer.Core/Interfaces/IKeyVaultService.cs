using System;
using System.Threading.Tasks;
namespace PhotoTour.Core
{
    public interface IKeyVaultService
    {
        Task<string> GetValueForKey(string keyName);
    }
}
