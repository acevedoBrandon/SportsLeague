using SportsLeague.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsLeague.API.DTOs.Request
{
    public class UpdateCategoryDTO
    {
        public SponsorCategory Category { get; set; }
    }
}
