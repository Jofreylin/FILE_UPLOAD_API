using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FILE_UPLOAD_API.Models;

[Table("ServicesLog", Schema = "XDMS")]
public partial class ServicesLog
{
    [Key]
    public int LogId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string MethodName { get; set; } = null!;

    [Unicode(false)]
    public string? SentParameters { get; set; }

    [Unicode(false)]
    public string? LogMessages { get; set; }

    public long? CreateUserId { get; set; }
}
