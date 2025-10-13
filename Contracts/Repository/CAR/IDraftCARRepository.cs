using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.CAR
{
    public interface IDraftCARRepository
    {
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId, string userName, int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
    }
}
