using Entities.CAR;
using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface IImmidateActionRepository
    {
        Task<IEnumerable<string>> getImmidateActionList(int plant);
        Task<IEnumerable<ImmidateActionDto>> GetImmidateAction(string search, string SearchADV);
        Task<IEnumerable<ImmidateActionDto>> InsertNewImmidateAction(string ImmidateName, int plant);
        Task<IEnumerable<ImmidateActionDto>> UpdateImmidateAction(string ImmidateName, int id);
        Task<IEnumerable<ImmidateActionDto>> DataDelete(int id);
        Task<IEnumerable<ImmidateActionDto>> DataPermDelete(int id);
        Task<IEnumerable<ImmidateActionDto>> DataRecover(int id);
        Task<byte[]> Template();
        Task<IEnumerable<string>> Import(string filePath, string userId);

    }
}
