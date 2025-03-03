using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class DynamicFormConfigurationQuery
    {
        public static readonly string GetIssueFeedbackcolumn = @"SELECT 
                                                                    COLUMN_NAME AS ColumnName,
                                                                    CASE
																		WHEN DATA_TYPE IN ('nvarchar', 'varchar', 'char', 'nchar') THEN 'String'
																		ELSE DATA_TYPE
																	END AS DataType,
	                                                                CASE
                                                                    WHEN DATA_TYPE IN ('nvarchar', 'varchar', 'char', 'nchar') THEN CHARACTER_MAXIMUM_LENGTH
                                                                    WHEN DATA_TYPE IN ('decimal', 'numeric') THEN NUMERIC_PRECISION
                                                                    ELSE NULL
                                                                    END AS Length,
	                                                                CASE    
                                                                        WHEN IS_NULLABLE = 'YES' THEN 1 
                                                                        ELSE 0 
                                                                    END AS AllowNull
                                                                    FROM INFORMATION_SCHEMA.COLUMNS
                                                                    WHERE TABLE_NAME = 'IssueFeedback'
                                                                    ORDER BY ORDINAL_POSITION";

        public static readonly string GetDynamicFormConfiguration = @"
         select Distinct dfc.Id,dfc.plant,dfc.FormType,dfc.FieldName,dfc.FieldType,dfc.FieldLength,dfc.Mandatory,dfc.FieldElement,dfc.OptionDataResource,
         dfc.DBResource,dfc.Query,dfc.DataOption,dfc.Sequence,dfc.CreatedBy,dfc.CreatedByName,dfc.CreatedDate,dfc.UpdatedBy,dfc.UpdatedByName,dfc.UpdatedDate,dfc.delflag
         from DynamicFormConfiguration dfc
         INNER JOIN TableMappingFieldName tbfn on dfc.FieldName =tbfn.FieldName and dfc.plant =tbfn.plant
         Where dfc.Plant = @Plant and language = 'EN' and tbfn.DelFlag =0";


        public static readonly string SearchADV = @"
        select Distinct Id,plant,FormType,FieldName,FieldType,FieldLength,Mandatory,FieldElement,OptionDataResource,
        DBResource,Query,DataOption,Sequence,CreatedBy,CreatedByName,CreatedDate,UpdatedBy,UpdatedByName,UpdatedDate,delflag
        from DynamicFormConfiguration WHERE  
        Plant = @plant
        AND (@formTypeAdv IS NULL OR FormType LIKE '%' + @formTypeAdv + '%')
        AND (@fieldNameAdv IS NULL OR FieldName LIKE '%' + @fieldNameAdv + '%')
        AND (@fieldTypeAdv IS NULL OR FieldType LIKE '%' + @fieldTypeAdv + '%')
        AND (@fieldLengthAdv IS NULL OR FieldLength LIKE '%' + @fieldLengthAdv + '%')
        AND (Mandatory = @mandatoryAdv OR @mandatoryAdv IS NULL)
        AND (@fieldElementAdv IS NULL OR FieldElement LIKE '%' + @fieldElementAdv + '%')
        AND (@optionDataResourceAdv IS NULL OR OptionDataResource LIKE '%' + @optionDataResourceAdv + '%')
        AND (@dbResourceAdv IS NULL OR DBResource LIKE '%' + @dbResourceAdv + '%')
        AND (@queryAdv IS NULL OR Query LIKE '%' + @queryAdv + '%')
        AND (@dataOptionAdv IS NULL OR DataOption LIKE '%' + @dataOptionAdv + '%')
        AND (@sequenceAdv IS NULL OR Sequence LIKE '%' + @sequenceAdv + '%')";

        public static readonly string SearchDB = @"
        select Distinct Id,plant,FormType,FieldName,FieldType,FieldLength,Mandatory,FieldElement,OptionDataResource,
        DBResource,Query,DataOption,Sequence,CreatedBy,CreatedByName,CreatedDate,UpdatedBy,UpdatedByName,UpdatedDate,delflag
        from DynamicFormConfiguration WHERE (plant LIKE '%' + @Search + '%' OR 
                                            FormType LIKE '%' + @Search + '%' OR
                                            FieldName LIKE '%' + @Search + '%' OR
                                            FieldType LIKE '%' + @Search + '%' OR
                                            FieldLength LIKE '%' + @Search + '%' OR
                                            Mandatory LIKE '%' + @Search + '%' OR
                                            FieldElement LIKE '%' + @Search + '%' OR
                                            OptionDataResource LIKE '%' + @Search + '%' OR
                                            DBResource LIKE '%' + @Search + '%' OR
                                            Query LIKE '%' + @Search + '%' OR
                                            DataOption LIKE '%' + @Search + '%' OR
                                            Sequence LIKE '%' + @Search + '%' )";

        public static readonly string InsertNewData = @"
        insert into DynamicFormConfiguration(plant,FormType,FieldName,FieldType,FieldLength,Mandatory,FieldElement,OptionDataResource,DBResource,Query,DataOption,sequence,CreatedBy,CreatedByName,CreatedDate)
        values(@plant,@FormType,@FieldName,@FieldType,@FieldLength,@Mandatory,@FieldElement,@OptionDataResource,@DBResource,@Query,@DataOption,@sequence,@userid,@userid,GETDATE())";

        public static readonly string UpdateData = @"
        DECLARE @id1 INT;
        DECLARE @id2 INT;
        declare @sequence1 int;
 
        SELECT @id1 = id
        FROM dynamicformconfiguration
        WHERE sequence = @sequence and plant =@plant and formtype =@formtype;

        SELECT @sequence1 = Sequence
        FROM dynamicformconfiguration
        WHERE id = @id ; 

        UPDATE dynamicformconfiguration
        SET Sequence = @sequence
        where id =@id

        UPDATE dynamicformconfiguration
        SET Sequence = @sequence1
        WHERE id = @id1;

        UPDATE DynamicFormConfiguration
        SET FormType = @FormType,FieldElement = @FieldElement,OptionDataResource = @OptionDataResource,
        DBResource = @DBResource,Query=@Query,DataOption = @DataOption, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteData = @"
        UPDATE DynamicFormConfiguration
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteData = @"
        DELETE FROM DynamicFormConfiguration WHERE ID = @id";

        public static readonly string RecoverData = @"
        UPDATE DynamicFormConfiguration 
        SET Sequence= @Sequence,DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string Import = @"
        Use CAR;UPDATE DynamicFormConfiguration
        SET 
            Plant = B.Plant,
            FormType = B.FormType,
            FieldName = B.FieldName,
            FieldType = B.FieldType,
            FieldLength = B.FieldLength,
            Mandatory = B.Mandatory,
            FieldElement = B.FieldElement,
            OptionDataResource = B.OptionDataResource,
            DBResource = B.DBResource,
            Query = B.Query,
            DataOption = B.DataOption,
            Sequence = B.Sequence,
            UpdatedBy = UPPER(@userId),
            UpdatedByName = UPPER(@userId),
            UpdatedDate = GETDATE(),
            DelFlag = 0
        FROM DynamicFormConfiguration A
        INNER JOIN ##temp B 
        ON (A.FieldName = B.FieldName)

        INSERT INTO DynamicFormConfiguration 
        (Plant, FormType, FieldName, FieldType, FieldLength, Mandatory, FieldElement, OptionDataResource, 
        DBResource, Query, DataOption, Sequence, CreatedBy, CreatedByName, CreatedDate, DelFlag) 
        SELECT
            B.Plant,
            B.FormType,
            B.FieldName,
            B.FieldType,
            B.FieldLength,
            B.Mandatory,
            B.FieldElement,
            B.OptionDataResource,
            B.DBResource,
            B.Query,
            B.DataOption,
            B.Sequence,
            UPPER(@userId),
            UPPER(@userId),
            GETDATE(),
            0
        FROM ##temp B
        WHERE NOT EXISTS (
            SELECT A.FieldName
            FROM DynamicFormConfiguration A 
            WHERE A.FieldName = B.FieldName
        )";

        public static readonly string query = @"
                                                DECLARE @query NVARCHAR(MAX);

                                                -- Retrieve the query string from the table
                                                SELECT @query = Query 
                                                FROM DynamicFormConfiguration
                                                WHERE FormType = @FormType AND FieldName = @FieldName;

                                                -- Execute the query dynamically
                                                EXEC sp_executesql @query;";

        public static readonly string GetTableMappingFieldName = @"WITH RankedData AS (
            SELECT 
                ID, 
                Plant, 
                FieldName, 
                UIDisplay, 
                Language, 
                CreatedBy, 
                CreatedByName, 
                CreatedDate, 
                UpdatedBy, 
                UpdatedByName, 
                UpdatedDate, 
                DelFlag,
                -- Prioritizing 'EN' over 'ZH' using ROW_NUMBER
                ROW_NUMBER() OVER (PARTITION BY FieldName ORDER BY 
                    CASE WHEN Language = 'EN' THEN 1 ELSE 2 END) AS RowNum
            FROM TableMappingFieldName
            WHERE DelFlag = 0 and Plant = @Plant
        )
        SELECT 
            ID, 
            Plant, 
            FieldName, 
            UIDisplay, 
            Language, 
            CreatedBy, 
            CreatedByName, 
            CreatedDate, 
            UpdatedBy, 
            UpdatedByName, 
            UpdatedDate, 
            DelFlag,
            -- FieldNameManipulate based on the condition
            CASE 
                WHEN Language = 'EN' THEN UIDisplay 
                ELSE FieldName
            END AS FieldNameManipulate
        FROM RankedData
        WHERE RowNum = 1;";
    } 
}
