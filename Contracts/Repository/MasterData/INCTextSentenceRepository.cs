using Entities;
using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface INCTextSentenceRepository
    {
        Task<IEnumerable<NCTextSentenceDto>> GetDataNcTextSentence(string search, string SearchADV, bool delflag);
        Task<IEnumerable<NCTextSentenceDto>> InsertDataNcTextSentence(string TextSentence, bool isFirstSentence, bool isLastSentence, string userId);
        Task<IEnumerable<NCTextSentenceDto>> UpdateDataNcTextSentence(int id, string TextSentence, bool isFirstSentence, bool isLastSentence, string userId);
        Task<IEnumerable<NCTextSentenceDto>> DataDelete(int id, string userId);
        Task<IEnumerable<NCTextSentenceDto>> DataPermDelete(int id);
        Task<IEnumerable<NCTextSentenceDto>> DataRecover(int id, string userId);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);

    }
}
