using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class DraftCARQuery
    {
        public static readonly string GetDraftCAR = @"
        select Plant, FormType, Product, MaterialCode, TttlQtyUOM, Dept, 
	        VendorCode, VendorDesc, SamplingCheck, NcQty, NCCategory, NCDescription,
	        StatusOfFinding, AffectedCavity
        from [dbo].[IssueFeedback] WHERE Status = @status";

        public static readonly string SearchDraftCAR = @"
        SELECT DISTINCT ID, Plant, DraftCAR, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM DraftCAR
        WHERE Plant =@Plant and DraftCAR LIKE '%' + @SearchADV + '%'";

        public static readonly string SearchDraftCARInDB = @"
        SELECT DISTINCT ID, Plant, DraftCAR, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM DraftCAR
        WHERE Plant =@Plant and DraftCAR LIKE '%' + @search + '%'";

        public static readonly string InsertNewDraftCAR = @"
        INSERT INTO IssueFeedback (DraftCAR, Plant, CreatedBy, CreatedByName, CreatedDate) 
        VALUES (@DraftCAR, @Plant, @userId, @userId, GETDATE())";

        public static readonly string UpdateDraftCAR = @"
        UPDATE DraftCAR
        SET DraftCAR = @DraftCAR, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteDraftCAR = @"
        UPDATE DraftCAR
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteDraftCAR = @"DELETE FROM DraftCAR WHERE ID = @id";

        public static readonly string RecoverDraftCAR = @"
        UPDATE DraftCAR 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string ImportDraftCAR = @"
        insert into IssueFeedback(
            Plant, FormNo, FormType, Product, MaterialCode, TttlQtyUOM, 
            TttlQty, Dept, VendorCode, VendorDesc, SamplingCheck, 
            NcQty, NcCategory, NCDescription, StatusOfFinding, AffectedCavity,
            Status, MainStatus, IssueBy, IssueByName, IssueDate, 
            CheckingMethod
        ) 
        SELECT 
            Plant, FormNo, FormType,Product,[Material Code], UOM, 
            [Total Qty],[Supplier Dept],[Supplier Vendor],[Supplier Name], [Inspected Sample], 
            Nonconforming, [NC Category],[NC Description],[Status of finding],[Affected Cavity],
            Status, 'CAR RAISE', UPPER(@userId), @UserName, GETDATE(), 
            @CheckingMethod
        FROM ##temp B
        
";
    }
}
