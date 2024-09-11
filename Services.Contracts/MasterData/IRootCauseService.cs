using Entities;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using Microsoft.AspNetCore.Http;
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
        Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> InsertNewRootCauseCategory(string RootCauseName);
        Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> GetRootCauseCategory(string search,string SearchADV);
        Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> UpdateNewRootCauseCategory(string RootCauseName, int id);
        Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> DataDelete(int id);
        Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> DataPermDelete(int id);
        Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> DataRecover(int id);
        Task<byte[]> Template();
        Task<ApiResponse<IEnumerable<string>>> Import(IFormFile file, string userId);
    }
}
