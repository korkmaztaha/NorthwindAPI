using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NorthwindApi.Domain.Entities;

public partial class CustomerDemographics
{
    [Key]
    [Column("CustomerTypeID")]
    [StringLength(10)]
    public string CustomerTypeId { get; set; } = null!;

    [Column(TypeName = "ntext")]
    public string? CustomerDesc { get; set; }

    [ForeignKey("CustomerTypeId")]
    [InverseProperty("CustomerType")]
    public virtual ICollection<Customers> Customer { get; set; } = new List<Customers>();
}
