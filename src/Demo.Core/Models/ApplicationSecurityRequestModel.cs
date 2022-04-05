using System.ComponentModel.DataAnnotations;

namespace Demo.Core.Models
{
    public class ApplicationSecurityRequestModel
    {
        [Required(ErrorMessage = "Application Id is required")]
        public Guid ApplicationId { get; set; }

        public int? PersonId { get; set; }

        public ICollection<PositionRequestModel> Positions { get; set; }
    }

    public class PositionRequestModel
    {
        [Required(ErrorMessage = "Role Id is required")]
        public int RoleId { get; set; }

        public int PositionId { get; set; }
    }
}
