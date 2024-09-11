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
        Task<IEnumerable<RootCauseCategoryDto>> InsertNewRootCauseCategory(string RootCauseName);
        Task<IEnumerable<RootCauseCategoryDto>> GetRootCauseCategory(string search,string SearchADV);
        Task<IEnumerable<RootCauseCategoryDto>> UpdateNewRootCauseCategory(string RootCauseName,int id);
        Task<IEnumerable<RootCauseCategoryDto>> DataDelete(int id);
        Task<IEnumerable<RootCauseCategoryDto>> DataPermDelete(int id);
        Task<IEnumerable<RootCauseCategoryDto>> DataRecover(int id);
        Task<byte[]> Template();
        Task<IEnumerable<string>> Import(string filePath, string userId);
    }
}
