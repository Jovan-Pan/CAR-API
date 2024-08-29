using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.MasterData
{
    public interface IRootCauseService
    {
        Task<ApiResponse<IEnumerable<string>>> getRootCauseList(int plant);
    }
}
