using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using Services.Contracts.CAR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;
using System.Transactions;
using System.Xml.Linq;

namespace Services.CAR
{
    internal sealed class DraftCARService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : IDraftCARService
    {

        public async Task<byte[]> Template()
        {
            var result = await data.DraftCAR.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId, string userName)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var importResult = await data.DraftCAR.Import(filePath, userId, userName);

            var resultList = new List<ImportResult> { importResult };
            return resultList;
        }
    }
}
