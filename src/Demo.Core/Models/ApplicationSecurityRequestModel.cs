namespace Demo.Core.Models
{
    public class ApplicationSecurityRequestModel
    {
        public Guid ApplicationId { get; set; }

        public int? PersonId { get; set; }

        public ICollection<PositionRequestModel> Positions { get; set; }
    }

    public class PositionRequestModel
    {
        public int RoleId { get; set; }

        public int PositionId { get; set; }
    }
}
