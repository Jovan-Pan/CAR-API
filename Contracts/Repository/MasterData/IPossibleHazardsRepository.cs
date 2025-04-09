using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface IPossibleHazardsRepository
    {
        Task<IEnumerable<PossibleHazardsDto>> GetPossibleHazards(GETPossibleHazards GETPossibleHazards);
        Task<IEnumerable<PossibleHazardsDto>> InsertNewData(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<IEnumerable<PossibleHazardsDto>> UpdateData(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<IEnumerable<PossibleHazardsDto>> DataDelete(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<IEnumerable<PossibleHazardsDto>> DataPermDelete(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<IEnumerable<PossibleHazardsDto>> DataRecover(CRUDPossibleHazardsDto CRUDPossibleHazardsDto);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
