namespace NHTutorial.Model
{
    public class Account
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual AccountType AccountType { get; set; }
        public virtual User User { get; set; }        
    }
}