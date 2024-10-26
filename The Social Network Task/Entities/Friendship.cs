using The_Social_Network_Task.Framework;

namespace The_Social_Network_Task.Entities
{
    public class Friendship
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public FriendshipStatus Status { get; set; }
        public User User { get; set; }
        public User Friend { get; set; }
    }
}
