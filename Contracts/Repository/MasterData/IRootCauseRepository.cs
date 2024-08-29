using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface IRootCauseRepository
    {
        Task<IEnumerable<string>> getRootCauseList(int plant);
    }
}
