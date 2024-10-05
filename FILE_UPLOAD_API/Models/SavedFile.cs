﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FILE_UPLOAD_API.Models;

[Table("SavedFiles", Schema = "XDMS")]
[Index("FileName", Name = "UQ__SavedFil__589E6EEC300A9C4F", IsUnique = true)]
public partial class SavedFile
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

    public int? StorageTypeId { get; set; }

    public bool Status { get; set; }

    public int? CompanyId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreationDate { get; set; }

    public int? CreateUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifyDate { get; set; }

    public int? ModifyUserId { get; set; }

    [ForeignKey("DocumentCategoryId")]
    [InverseProperty("SavedFiles")]
    public virtual DocumentCategory? DocumentCategory { get; set; }
}
