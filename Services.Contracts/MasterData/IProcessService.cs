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
    public interface IProcessService
    {
        Task<ApiResponse<IEnumerable<ProcessDto>>> GetProcess(GETProcess GETProcess);
        Task<ApiResponse<IEnumerable<ProcessDto>>> InsertNewData(CRUDProcessDto CRUDProcessDto);
        Task<ApiResponse<IEnumerable<ProcessDto>>> UpdateData(CRUDProcessDto CRUDProcessDto);
        Task<ApiResponse<IEnumerable<ProcessDto>>> DataDelete(CRUDProcessDto CRUDProcessDto);
        Task<ApiResponse<IEnumerable<ProcessDto>>> DataPermDelete(CRUDProcessDto CRUDProcessDto);
        Task<ApiResponse<IEnumerable<ProcessDto>>> DataRecover(CRUDProcessDto CRUDProcessDto);
        Task<byte[]> Template();
        Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId);
    }
}
