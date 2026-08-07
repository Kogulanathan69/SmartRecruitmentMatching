namespace NexHire.Domain.Modules.Company_Trust.Entities;

public class CompanyDocument
{
    public Guid CompanyDocumentId { get; set; } = Guid.NewGuid();

    public Guid CompanyId { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    // Real physical/object storage key
    public string StorageKey { get; set; } = string.Empty;

    public string? ContentType { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
}