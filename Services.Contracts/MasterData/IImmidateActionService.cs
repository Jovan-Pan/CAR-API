using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.MasterData
{
    public interface IImmidateActionService
    {
        Task<ApiResponse<IEnumerable<string>>> getImmidateActionList(int plant);
    }
}
