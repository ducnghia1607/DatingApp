﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using API.Entities;

namespace API;

[Table("Photos")]
public class Photo
{

    public int Id { get; set; }

    public string Url { get; set; }

    public bool isMain { get; set; }

    public string? PublicId { get; set; }

    public int AppUserId { get; set; }

    public AppUser AppUser { get; set; }
}
