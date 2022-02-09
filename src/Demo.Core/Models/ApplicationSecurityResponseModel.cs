namespace Demo.Core.Models
{
    public class ApplicationSecurityResponseModel
    {
        public int RoleId { get; set; }

        public int PositionId { get; set; }

        public ICollection<ApplicationAccess> ApplicationAccess { get; set; }
    }
}