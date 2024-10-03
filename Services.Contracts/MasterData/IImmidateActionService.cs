using Entities;
using Entities.CAR;
using Microsoft.AspNetCore.Http;
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
        Task<ApiResponse<IEnumerable<ImmidateActionDto>>> GetImmidateAction(string? search, string? SearchADV);
        Task<ApiResponse<IEnumerable<ImmidateActionDto>>> InsertNewImmidateAction(string ImmidateName, int plant, string userId);
        Task<ApiResponse<IEnumerable<ImmidateActionDto>>> UpdateImmidateAction(string ImmidateName, int id, string userId);
        Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataDelete(int id,string userId);
        Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataPermDelete(int id);
        Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataRecover(int id, string userId);
        Task<byte[]> Template();
        Task<ApiResponse<IEnumerable<string>>> Import(IFormFile file, string userId);
    }
}
