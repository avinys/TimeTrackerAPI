namespace TimeTrackerAPI.DTOs
{
    public class ResetPasswordDto
    {
        public int UserId { get; set; }
        public string Token { get; set; } = ""; 
        public string NewPassword { get; set; } = "";
    }
}
