using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.NetCoreIdentity.Models
{
    public class RoleAssignListModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public bool Exist { get; set; }
    }
}
