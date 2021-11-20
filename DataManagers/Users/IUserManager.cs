namespace NetFinal.DataManagers.Users
{
    public abstract class IUserManager
    {
        public abstract void AddUser();
        
        public abstract void AddUserRating();
        public abstract void ShowUserRatings();
        public abstract void AddOccupation();
        public abstract void DisplayUsers();
        
    }
}