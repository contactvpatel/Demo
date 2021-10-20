using System;

namespace Demo.Core.Entities.Base
{
    public abstract class Entity
    {
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
