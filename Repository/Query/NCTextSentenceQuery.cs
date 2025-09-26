using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class NCTextSentenceQuery
    {
        public static readonly string GetDataNcTextSentence = @" 
        select distinct id,TextSentence,isFirstSentence,isLastSentence,CreatedBy,
        CreatedByName,CreatedDate,UpdatedBy,UpdatedByName,UpdatedDate,Delflag
        from NCTextSentence Where 1=1";

        public static readonly string InsertDataNcTextSentence = @"
        INSERT INTO NCTextSentence ( TextSentence,isFirstSentence,isLastSentence,CreatedBy,CreatedByName,CreatedDate)
        VALUES (@TextSentence,@isFirstSentence,@isLastSentence,@userId,@userId,GETDATE())";

        public static readonly string UpdateDataNcTextSentence = @" 
        UPDATE NCTextSentence
        SET TextSentence =@TextSentence, isFirstSentence=@isFirstSentence, isLastSentence=@isLastSentence,
        UpdatedBy=@userId, UpdatedByName=@userId, UpdatedDate=GETDATE()
        WHERE id=@id";

        public static readonly string DataDelete = @"
        UPDATE NCTextSentence
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName=@userId, UpdatedDate =getdate() 
        WHERE id=@id";

        public static readonly string DataPermDelete = @"DELETE FROM NCTextSentence WHERE id=@id ";

        public static readonly string DataRecover = @"
        UPDATE NCTextSentence
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName=@userId, UpdatedDate =getdate()
        WHERE id=@id";

        public static readonly string SearchDatainDB = @"
        select distinct id,TextSentence,isFirstSentence,isLastSentence,CreatedBy,
        CreatedByName,CreatedDate,UpdatedBy,UpdatedByName,UpdatedDate,DelFlag FROM NCTextSentence
        WHERE TextSentence LIKE '%' + @search + '%'";

        public static readonly string SearchadvData = @"
        select distinct id,TextSentence,isFirstSentence,isLastSentence,CreatedBy,
        CreatedByName,CreatedDate,UpdatedBy,UpdatedByName,UpdatedDate,DelFlag FROM NCTextSentence 
        WHERE TextSentence LIKE '%' + @SearchADV + '%'";

        public static readonly string Import = @"UPDATE NCTextSentence 
                                                SET 
                                                    TextSentence = B.TextSentence, 
                                                    isFirstSentence = B.isFirstSentence, 
                                                    isLastSentence = B.isLastSentence,
                                                    UpdatedBy = UPPER(@userId),
                                                    UpdatedByName = UPPER(@userId),
                                                    UpdatedDate = GETDATE(), 
                                                    Delflag = 0 
                                                FROM NCTextSentence A 
                                                INNER JOIN ##temp B 
                                                ON (A.TextSentence = B.TextSentence AND A.Plant = B.Plant)
                                                INSERT INTO NCTextSentence 
                                                (TextSentence, isFirstSentence,isLastSentence, CreatedBy, CreatedByName, CreatedDate, Delflag) 
                                                SELECT 
                                                    B.TextSentence,
                                                    UPPER(isFirstSentence), 
                                                    UPPER(isLastSentence), 
                                                    UPPER(@userId),
                                                    UPPER(@userId),
                                                    GETDATE(), 
                                                    0 
                                                FROM ##temp B
                                                WHERE 
                                                    NOT EXISTS (
                                                        SELECT A.TextSentence
                                                        FROM NCTextSentence A 
                                                        WHERE A.TextSentence = B.TextSentence AND A.Plant = B.Plant
                                                    )";
    }
}
    