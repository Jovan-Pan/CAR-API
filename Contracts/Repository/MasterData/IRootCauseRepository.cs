using Entities;
using Entities.CAR;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData
{
    public interface IRootCauseRepository
    {
        Task<IEnumerable<string>> getRootCauseList(int plant);
        Task<IEnumerable<RootCauseCategoryDto>> InsertNewRootCauseCategory(string RootCauseName, string userId, int plant);
        Task<IEnumerable<RootCauseCategoryDto>> GetRootCauseCategory(string search,string SearchADV, bool delflag);
        Task<IEnumerable<RootCauseCategoryDto>> UpdateNewRootCauseCategory(string RootCauseName,int id, string userId);
        Task<IEnumerable<RootCauseCategoryDto>> DataDelete(int id, string userId);
        Task<IEnumerable<RootCauseCategoryDto>> DataPermDelete(int id);
        Task<IEnumerable<RootCauseCategoryDto>> DataRecover(int id, string userId);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
