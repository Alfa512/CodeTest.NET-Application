namespace CodeTest.NET_Application.Data.Models
{
    public class User : IEntity
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Age { get; set; }
    }
}