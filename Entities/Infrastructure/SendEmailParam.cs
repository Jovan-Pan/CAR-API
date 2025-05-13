using System.ComponentModel.DataAnnotations;

namespace Entities.Infrastructure;

public class SendEmailParam
{
    [Required]
    public string FromName { get; set; } = string.Empty;
    [Required]
    public string FromAddress { get; set; } = string.Empty;
    [Required]
    public string Recipient { get; set; } = string.Empty;
    public string CopyRecipient { get; set; } = string.Empty;
    public string BlindCopyRecipient { get; set; } = string.Empty;
    public string ReplyTo { get; set; } = string.Empty;
    [Required]
    public string Subject { get; set; } = string.Empty;
    [Required]
    public string Body { get; set; } = string.Empty;
    public string BodyRemark { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public ImportanceEnum Importance { get; set; } = ImportanceEnum.NORMAL;
    public SensitivityEnum Sensitivity { get; set; } = SensitivityEnum.NORMAL;
    [Required]
    public string CreateUser { get; set; } = string.Empty;

    //public List<string> AttachmentsPath { get; set; } = [];
    public List<LinkedFile> AttachmentsPath { get; set; } = [];
    public List<LinkedFile> LinkedFiles { get; set; } = new();
}

public enum ImportanceEnum
{
    NORMAL,
    HIGH
}

public enum SensitivityEnum
{
    NORMAL,
    CONFIDENTIAL
}


public class LinkedFile
{
    public string FilePath { get; set; }
    public string ContentId { get; set; }
}