using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FILE_UPLOAD_API.Models;

[Keyless]
public partial class ViewGetStorageType
{
    public int TypeId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string TypeName { get; set; } = null!;

    public bool Status { get; set; }

    public int? CompanyId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreationDate { get; set; }

    public int? CreateUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifyDate { get; set; }

    public int? ModifyUserId { get; set; }
}
