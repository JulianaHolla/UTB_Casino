using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using UTB.Casino.Web.Validation;

namespace UTB.Casino.Web.Areas.Admin.Models;

public class GameEditViewModel
{
    public int Id { get; set; }

    [Required, StringLength(60, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Required]
    [Display(Name = "Game type")]
    public string Slug { get; set; } = "";

    [StringLength(200)]
    public string Description { get; set; } = "";

    [Range(typeof(decimal), "0.01", "100000")]
    [Display(Name = "Min bet")]
    public decimal MinBet { get; set; } = 1;

    [Range(typeof(decimal), "0.01", "100000"), GreaterThanOrEqualTo(nameof(MinBet))]
    [Display(Name = "Max bet")]
    public decimal MaxBet { get; set; } = 100;

    [Display(Name = "Open for players")]
    public bool IsActive { get; set; } = true;

    public string? ImagePath { get; set; }

    [Display(Name = "Image (jpg, png, webp, gif; max 2 MB)")]
    public IFormFile? Image { get; set; }

    public IList<SelectListItem> Slugs { get; set; } = new List<SelectListItem>();
}

public class UserEditViewModel
{
    [Required]
    public string Id { get; set; } = "";

    [Required, StringLength(30, MinimumLength = 3), RegularExpression(@"^[A-Za-z0-9_.-]+$")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = "";

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = "";

    [Required, StringLength(60, MinimumLength = 2)]
    [Display(Name = "Display name")]
    public string DisplayName { get; set; } = "";

    [Required]
    public string Role { get; set; } = "";

    [Display(Name = "Blocked")]
    public bool IsBlocked { get; set; }

    public decimal Balance { get; set; }
    public IList<SelectListItem> RoleItems { get; set; } = new List<SelectListItem>();
}

public class AdjustBalanceViewModel
{
    [Required]
    public string UserId { get; set; } = "";

    [Range(typeof(decimal), "-100000", "100000")]
    [Display(Name = "Amount (negative removes credits)")]
    public decimal Amount { get; set; }

    [StringLength(200)]
    [Display(Name = "Reason")]
    public string? Note { get; set; }
}

public class BetEditViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Result { get; set; } = "";

    public string Summary { get; set; } = "";
}

public class TransactionEditViewModel
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Note { get; set; } = "";

    public string Summary { get; set; } = "";
}
