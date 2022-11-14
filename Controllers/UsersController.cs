using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using TinTucGameAPI.Models;
using TinTucGameAPI.Models.View;

namespace TinTucGameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class UsersController : Controller
    {
        private readonly doan5Context _context;
        private readonly IConfiguration _configuration;

        public UsersController(doan5Context context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok(await _context.Users.ToListAsync());
        }
        [HttpPost]
        [Route("/sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(AuthenticateModel _userData)
        {
            var newSalt = GenerateSalt(12);
            var userRole = _context.Roles.Where(r => r.Role1 == "User").FirstOrDefault();
            _userData.Password = ComputeHash(Encoding.UTF8.GetBytes(_userData.Password), Encoding.UTF8.GetBytes(newSalt));
            _userData.Salt = newSalt;
            User newUser = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = _userData.Email,
                Passwordhash = _userData.Password,
                Salt = newSalt,
               
            };
            newUser.Roles.Add(userRole);
            _context.Users.Add(newUser);          
            _context.SaveChanges();
            return Ok(_userData);
        }
        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Post(AuthenticateModel _userData)
        {
            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {
                var userTemp = _context.Users.Where(e => e.Email == _userData.Email).FirstOrDefault();
                if (userTemp == null)
                    return BadRequest("Invalid credentials");
                var HashPassword = ComputeHash(Encoding.UTF8.GetBytes(_userData.Password), Encoding.UTF8.GetBytes(userTemp.Salt));
                var user = await GetUser(_userData.Email, HashPassword);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("ID", user.Id.ToString()),
                        new Claim("Email", user.Email),
                        
                    };
                    foreach (var role in user.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Role1));
                    }
                    
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    return Ok(new { User = user.Email, token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        [NonAction]
        private async Task<User> GetUser(string email, string password)
        {
            return await _context.Users.Include(r=>r.Roles).FirstOrDefaultAsync(u => u.Email == email && u.Passwordhash == password);
        }

        [NonAction]
        public string GenerateSalt(int size)
        {
            var saltBytes = new byte[size];
            using (var provider = RandomNumberGenerator.Create())
            {
                provider.GetNonZeroBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
        [NonAction]
        public string ComputeHash(byte[] bytesToHash, byte[] salt)
        {
            var byteResult = new Rfc2898DeriveBytes(bytesToHash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(24));
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> updateRoleUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            return Ok(user);
        }
    }

}
