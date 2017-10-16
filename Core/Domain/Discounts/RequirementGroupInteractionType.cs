using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Discounts
{
    /// <summary>
    /// Represents an interaction type within the group of requirements
    /// </summary>
    public enum RequirementGroupInteractionType
    {
        /// <summary>
        /// All requirements within the group must be met
        /// </summary>
        And = 0,

        /// <summary>
        /// At least one of the requirements within the group must be met 
        /// </summary>
        Or = 2,
    }
}
