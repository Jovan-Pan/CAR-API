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

        public static readonly string ImportDraftCAR =
            //@"
            //        insert into IssueFeedback(
            //            Plant, FormNo, FormType, Product, MaterialCode, TttlQtyUOM, 
            //            TttlQty, Dept, VendorCode, VendorDesc, SamplingCheck, 
            //            NcQty, NcCategory, NCDescription, StatusOfFinding, AffectedCavity,
            //            Status, MainStatus, IssueBy, IssueByName, IssueDate, 
            //            CheckingMethod
            //        ) 
            //        SELECT 
            //            Plant, FormNo, FormType,Product,[Material Code], UOM, 
            //            [Total Qty],[Supplier Dept],[Supplier Vendor],[Supplier Name], [Inspected Sample], 
            //            Nonconforming, [NC Category],[NC Description],[Status of finding],[Affected Cavity],
            //            Status, 'CAR RAISE', UPPER(@userId), @UserName, GETDATE(), 
            //            @CheckingMethod
            //        FROM ##temp B

            //";

            @"
                DECLARE @Plant NVARCHAR(MAX),
                    @FormNo NVARCHAR(MAX),
                    @FormType NVARCHAR(MAX),
                    @Product NVARCHAR(MAX),
                    @MaterialCode NVARCHAR(MAX),
                    @UOM NVARCHAR(MAX),
                    @TotalQty NVARCHAR(MAX),
                    @SupplierDept NVARCHAR(MAX),
                    @SupplierVendor NVARCHAR(MAX),
                    @SupplierName NVARCHAR(MAX),
                    @InspectedSample NVARCHAR(MAX),
                    @Nonconforming NVARCHAR(MAX),
                    @NCCategory NVARCHAR(MAX),
                    @NCDescription NVARCHAR(MAX),
                    @StatusOfFinding NVARCHAR(MAX),
                    @AffectedCavity NVARCHAR(MAX),
                    @Status NVARCHAR(MAX),
                    @MainStatus NVARCHAR(MAX),
                    
                    @IssueBy NVARCHAR(MAX),
                    @IssueByName NVARCHAR(MAX)

            DECLARE @NewFormNo NVARCHAR(MAX), @lastformno nvarchar(20), @lastrunNo int, @newrunNo INT

            DECLARE temp_cursor CURSOR FOR
            SELECT 
                Plant, FormType, Product, [Material Code], UOM, 
                [Total Qty], [Supplier Dept], [Supplier Vendor], [Supplier Name], [Inspected Sample], 
                Nonconforming, [NC Category], [NC Description], [Status of finding], [Affected Cavity],
                Status, 'CAR RAISE', UPPER(@userId), @UserName
            FROM ##temp;

            OPEN temp_cursor;
            FETCH NEXT FROM temp_cursor INTO 
                @Plant, @FormType, @Product, @MaterialCode, @UOM, 
                @TotalQty, @SupplierDept, @SupplierVendor, @SupplierName, @InspectedSample, 
                @Nonconforming, @NCCategory, @NCDescription, @StatusOfFinding, @AffectedCavity,
                @Status, @MainStatus, @IssueBy, @IssueByName

            WHILE @@FETCH_STATUS = 0
            BEGIN
	            -- Generate new Form No
	            SET @lastformno=  (select MAX(REPLACE(FormNo,@FormType,'')) 
		            from IssueFeedback 
		            where Plant=@Plant and FormType=@FormType 
		            and left(replace(FormNo,(@FormType+CONVERT(nvarchar(max),@plant)),''),8) = (format(getdate(),'yyyyMMdd')) )
	            set @lastformno = (SELECT LEFT(@lastformno, (LEN(@lastformno) - CHARINDEX('-', REVERSE(@lastformno)) + 1) - 1))

	            if(@lastformno is null)
	            begin
		            set @newformno = (select concat(@FormType,@plant,format(getdate(),'yyyyMMdd'),'0001'))
	            end
	            else
	            begin
		            SET @lastrunNo = (select RIGHT(@lastformno,4))
		            set @newrunNo = (@lastrunNo + 1)
		            set @newformno = (select concat(@FormType,@plant,format(getdate(),'yyyyMMdd'),FORMAT(@newrunNo, '0000')))
	            end

	            insert into IssueFeedback(
		            Plant, FormNo, FormType, [Product], MaterialCode, TttlQtyUOM, 
		            TttlQty, Dept, VendorCode, VendorDesc, SamplingCheck, 
		            NcQty, NcCategory, NCDescription, StatusOfFinding, AffectedCavity,
		            Status, MainStatus, IssueBy, IssueByName, IssueDate, 
		            CheckingMethod
	            ) 
	            SELECT @Plant, @newformno, @FormType, @Product, @MaterialCode, @UOM,
		            @TotalQty, @SupplierDept, @SupplierVendor, @SupplierName, @InspectedSample,
		            @Nonconforming, @NCCategory, @NCDescription, @StatusOfFinding, @AffectedCavity,
		            @Status, @MainStatus, @IssueBy, @IssueByName, GETDATE(), 
		            @CheckingMethod

                FETCH NEXT FROM temp_cursor INTO 
                    @Plant, @FormType, @Product, @MaterialCode, @UOM, 
                    @TotalQty, @SupplierDept, @SupplierVendor, @SupplierName, @InspectedSample, 
                    @Nonconforming, @NCCategory, @NCDescription, @StatusOfFinding, @AffectedCavity,
                    @Status, @MainStatus, @IssueBy, @IssueByName
            END

            CLOSE temp_cursor;
            DEALLOCATE temp_cursor;


            ";
    }
}
