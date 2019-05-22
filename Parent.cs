using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewTechParentPortalV3.Models
{
    public class Parent
    {
        public int ID { get; set; }
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public string LastName { get; set; }
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstMidName { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
