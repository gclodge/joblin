namespace Joblin.Domain.Common;

/// <summary>
/// Represents an auditable entity that tracks creation and last update information, while extending the <see cref="BaseEntity"/> class.
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity
{
    /// <summary>
    /// The <see cref="DateTimeOffset"/> when the <see cref="BaseAuditableEntity"> was created
    /// </summary>
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// [Nullable] The <see cref="string"/> identifier of who created the <see cref="BaseAuditableEntity">
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The <see cref="DateTimeOffset"/> when the <see cref="BaseAuditableEntity"> was last updated
    /// </summary>
    public DateTimeOffset? LastUpdated { get; set; }

    /// <summary>
    /// [Nullable] The <see cref="string"/> identifier of who last updated the <see cref="BaseAuditableEntity">
    /// </summary>
    public string? LastUpdatedBy { get; set; }
}