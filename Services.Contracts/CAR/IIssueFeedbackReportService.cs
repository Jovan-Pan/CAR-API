using Entities.ParamRequest;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.CAR;
using Entities.MasterData;

namespace Services.Contracts.CAR
{
    public interface IIssueFeedbackReportService
    {
        Task<ApiResponse<IssueFeedbackResultDto>> GetDataReport(GlobalParam param);
        Task<ApiResponse<TotalRecordForEachSttsDto>> GetTotalRecordForEachStts(GetTotalRecordForEachSttsParam param);
        Task<ApiResponse<IEnumerable<string>>> GetDefDataShow(int plant);
        Task<ApiResponse<IEnumerable<string>>> GetFormNumberListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<ApiResponse<IEnumerable<string>>> GetDeptListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<ApiResponse<IEnumerable<ProcessGroupDto>>> GetprocecessGrpCodeFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<ApiResponse<IEnumerable<string>>> GetproductListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<ApiResponse<IEnumerable<string>>> GetModelListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<ApiResponse<IEnumerable<string>>> GetMatTypeListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<ApiResponse<IEnumerable<TMATERIALDto>>> GetMaterialListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList, string searchTerm);
        Task<ApiResponse<IEnumerable<VendorDto>>> GetVendorListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<ApiResponse<string>> ProcessUpdate(IssueFeedbacReportUpdateParam mydata);

        Task<(Stream FileStream, string MimeType, string FileName)> GetFilePreviewAsync(GetAttachmentParam request);
        Task<List<(string Base64Content, string MimeType, string FileName)>> GetFilesAttchment(IEnumerable<GetAttachmentParam> request);

    }
}
