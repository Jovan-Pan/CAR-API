using Entities.MasterData;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.MasterData
{
    public interface IPossibleHazardsService
    {
        Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> GetPossibleHazards(GETPossibleHazards GETPossibleHazards);
        Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> InsertNewData(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> UpdateData(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> DataDelete(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> DataPermDelete(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<ApiResponse<IEnumerable<PossibleHazardsDto>>> DataRecover(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<byte[]> Template();
        Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId);
    }
}
