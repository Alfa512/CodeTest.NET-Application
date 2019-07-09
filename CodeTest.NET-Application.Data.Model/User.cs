using System.ComponentModel.DataAnnotations;

namespace CodeTest.NET_Application.Data.Models
{
    public class User : IEntity
    {
        [Display(Name = "ID", Order = 1)]
        public int ID { get; set; }
        [Display(Name = "FirstName", Order = 2)]
        public string FirstName { get; set; }
        [Display(Name = "LastName", Order = 3)]
        public string LastName { get; set; }
        [Display(Name = "Age", Order = 4)]
        public byte Age { get; set; }
    }
}