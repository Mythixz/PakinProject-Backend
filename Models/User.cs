using System;
using System.ComponentModel.DataAnnotations;

public class User
{
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User"; // ค่าเริ่มต้นเป็น User
        public bool IsAdmin { get; set; }

}
