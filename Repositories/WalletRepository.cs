using PakinProject.Data;
using PakinProject.Models;
using System;
using System.Linq;

namespace PakinProject.Repositories
{
    public class WalletRepository
    {
        private readonly PakinProjectContext _context;

        public WalletRepository(PakinProjectContext context)
        {
            _context = context;
        }

        // ดึงยอดเงินใน Wallet จาก UserEmail
        public decimal GetWalletBalance(string userEmail)
        {
            var wallet = _context.Wallets.FirstOrDefault(w => w.UserEmail == userEmail);
            return wallet?.Balance ?? 0; // คืนค่า 0 หากไม่พบ Wallet
        }

        // ดึง UserId จาก UserEmail
        public int GetUserIdByEmail(string userEmail)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            return user?.Id ?? 0; // คืนค่า 0 หากไม่พบ User
        }

        // หักยอดเงินจาก Wallet
        public void DeductBalance(int userId, decimal amount)
        {
            var wallet = _context.Wallets.FirstOrDefault(w => w.UserId == userId);
            if (wallet == null)
            {
                throw new InvalidOperationException($"Wallet not found for UserId: {userId}");
            }

            if (wallet.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance in wallet.");
            }

            wallet.Balance -= amount;
            _context.SaveChanges();
        }

        // เติมเงินเข้ากระเป๋า
        public void AddBalance(int userId, decimal amount)
        {
            var wallet = _context.Wallets.FirstOrDefault(w => w.UserId == userId);
            if (wallet != null)
            {
                wallet.Balance += amount; // เพิ่มยอดเงินใน Balance
                _context.SaveChanges();   // บันทึกการเปลี่ยนแปลง
            }
            else
            {
                // หากยังไม่มี Wallet สำหรับ UserId นี้ ให้สร้างใหม่
                _context.Wallets.Add(new Wallet
                {
                    UserId = userId,
                    Balance = amount,
                    UserEmail = _context.Users.FirstOrDefault(u => u.Id == userId)?.Email
                });
                _context.SaveChanges();   // บันทึกการเปลี่ยนแปลง
            }
        }
    }
}
