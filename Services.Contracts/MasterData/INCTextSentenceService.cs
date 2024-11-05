using Entities.MasterData;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Contracts.MasterData
{
    public interface INCTextSentenceService
    {
        Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> GetDataNcTextSentence(string search, string SearchADV);
        Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> InsertDataNcTextSentence(string TextSentence, bool isFirstSentence, bool isLastSentence, string userId);
        Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> UpdateDataNcTextSentence(int id, string TextSentence, bool isFirstSentence, bool isLastSentence, string userId);
        Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> DataDelete(int id, string userId);
        Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> DataPermDelete(string TextSentence);
        Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> DataRecover(int id, string userId);
        Task<byte[]> Template();
        Task<ApiResponse<IEnumerable<string>>> Import(IFormFile file, string userId);
    }
}
