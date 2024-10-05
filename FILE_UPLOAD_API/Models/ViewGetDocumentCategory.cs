using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FILE_UPLOAD_API.Models;

[Keyless]
public partial class ViewGetDocumentCategory
{
    public int CategoryId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string CategoryName { get; set; } = null!;

    public int SectionId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string SectionName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? SectionKeyword { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? FolderName { get; set; }

    [StringLength(101)]
    [Unicode(false)]
    public string? Directory { get; set; }

    public int? DisplayOrder { get; set; }

    public int? CompanyId { get; set; }

    public bool Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreationDate { get; set; }

    public int? CreateUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifyDate { get; set; }

    public int? ModifyUserId { get; set; }
}
