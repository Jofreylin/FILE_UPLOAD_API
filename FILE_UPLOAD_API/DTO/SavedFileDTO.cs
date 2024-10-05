using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FILE_UPLOAD_API.DTO;

public class SavedFileDTO
{
    [Key]
    public int FileId { get; set; }

    public Guid FileName { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? OriginalFileName { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? OriginalFileExtension { get; set; }

    public int? DocumentCategoryId { get; set; }

    [Unicode(false)]
    public string? RouteSaved { get; set; }
    public string? PathUrl { get; set; }

    public int? StorageTypeId { get; set; }

    public bool Status { get; set; }

    public int? CompanyId { get; set; }

    public int? UserId { get; set; }
}
