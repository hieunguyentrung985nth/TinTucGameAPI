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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using TinTucGameAPI.Models;
using TinTucGameAPI.Models.View;
using TinTucGameAPI.Models2;

namespace TinTucGameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class UsersController : Controller
    {
        private readonly doan5Context _context;
        private readonly doan5Context2 _context2 = new doan5Context2();
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
        [Route("/api/sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(AuthenticateModel _userData)
        {
            var newSalt = GenerateSalt(12);
            if (_context.Users.Any(u => u.Email == _userData.Email))
            {
                return BadRequest("Email bị trùng");
            }
            var userRole = _context.Roles.Where(r => r.Role1 == "User").FirstOrDefault();
            _userData.Password = ComputeHash(Encoding.UTF8.GetBytes(_userData.Password), Encoding.UTF8.GetBytes(newSalt));
            Models.User newUser = new Models.User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = _userData.Email,
                Passwordhash = _userData.Password,
                Salt = newSalt,

            };
            newUser.Roles.Add(userRole);
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return Ok(new
            {
                message = "Success",
                id = newUser.Id
            });
        }
        [Route("/api/login")]
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
                    var claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("ID", user.Id.ToString()),
                        new Claim("Email", user.Email),

                    };
                    foreach (var role in user.Roles)
                    {
                        claims.Add(new Claim("Role", role.Role1));
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddHours(10),
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
        private async Task<Models.User> GetUser(string email, string password)
        {
            return await _context.Users.Include(r => r.Roles).FirstOrDefaultAsync(u => u.Email == email && u.Passwordhash == password);
        }
        [HttpGet]
        public async Task<IActionResult> GetOneUser(string id)
        {
            var data = await _context.staff
                .Include(s => s.User.Roles)
                .Where(s => s.UserId == id)
                .Select(s => new { s.Id, s.Name, roles = s.User.Roles.Select(r => r.Role1) })
                .FirstOrDefaultAsync();
            return Ok(new
            {
                data = data
            });
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

        [HttpGet]
        public async Task<IActionResult> GetAllStaffs(string id)
        {
            //var result =
            //    from staff in _context.staff.Include(s=>s.User)
            //    join room in _context.Rooms on staff.Id equals room.CreatorId into sr
            //    from rs in sr.DefaultIfEmpty()
            //    join part in _context.Participants on rs.Id equals part.RoomId into pr
            //    from rp in pr.DefaultIfEmpty()
            //    select new
            //    {
            //        user = staff,
            //        room = new {room =rs, participants = rp }

            //    };
            //var claims = this.User.Identity as ClaimsIdentity;
            //var userId = claims.FindFirst("Id")?.Value;

            var currentUser = await _context.staff.Where(s => s.Id == id).FirstOrDefaultAsync();

            var result = await _context.staff.Where(s => s.Id != id).ToListAsync();

            //var result = await _context2.staff
            //    .Include(s => s.Rooms)
            //    .ThenInclude(r => r.Messages)
            //    .Select(s =>
            //    new
            //    {
            //        user = new { s.Id, s.Name },
            //        rooms = s.Rooms.Select(r =>
            //        new { r.Id, r.Name, r.CreatedAt, messages = r.Messages.Select(m => new { m.Id, m.RoomId, m.CreatedAt, m.SenderId, m.Sender.Name }).OrderByDescending(m => m.CreatedAt).Take(10).ToList(), participants = r.Users.Where(ru=>ru.Id != currentUser.Id).Select(ru => new { ru.Id, ru.Name }) })
            //    })
            //    .ToListAsync();

            //var part = await _context.Participants
            //    .Join(_context.Rooms, p => p.RoomId, r => r.Id, (p, r) => new { part = p, room = r })
            //    .ToListAsync();


            //var result = await _context2.staff
            //                .Select(s => new { s, roomId = Guid.NewGuid().ToString() })
            //                .ToListAsync();

            //foreach (var item in result)
            //{
            //    if (item.room == null)
            //    {
            //        Room defaultRoom = new()
            //        {
            //            Id = Guid.NewGuid().ToString(),
            //            CreatedAt = DateTime.Now,
            //            Name = "Default " + item.user.Name,
            //            CreatorId = item.user.Id
            //        };
            //        await _context.Rooms.AddAsync(defaultRoom);
            //    }
            //    var participants = await _context.Participants
            //        .Where(p => p.RoomId == item.room.Id)
            //        .ToListAsync();
            //}
            //await _context.SaveChangesAsync();
            //var user = await _context.staff
            //            .GroupJoin(_context.Rooms,
            //            s => s.Id,
            //            r => r.CreatorId,
            //            (s, r) => new { user = s, room = r })
            //            .ToListAsync();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms(string id)
        {
            //var claims = this.User.Identity as ClaimsIdentity;
            //var userId = claims.FindFirst("Id")?.Value;
            var currentUser = await _context.staff.Where(s => s.Id == id).FirstOrDefaultAsync();

            var result = await _context2.staff
                .Include(s => s.Rooms)             
                .ThenInclude(r => r.Messages)
                .ThenInclude(m=>m.MessNotifications)
                .Where(s => s.Id == currentUser.Id)
                .Select(s =>
                new
                {
                    user = new { 
                        s.Id, 
                        s.Name
                       
                    },                  
                    rooms = s.Rooms.Select(r =>
                    new { 
                        r.Id, 
                        r.Name, 
                        r.CreatedAt, 
                        r.Group,
                        r.Latest,
                        unseenCount =_context2.MessNotifications
                                    .Join(_context2.Messages, n=>n.MessId, m=>m.Id, (n, m) => new {not = n, mess = m})
                                    .Join(_context2.Rooms, mn=>mn.mess.RoomId, r=>r.Id, (mn, r) => new {mn = mn, room = r})
                                    .Where(mnr=>mnr.mn.not.Read == DateTime.Parse("0001-01-01 00:00:00.0000000") && mnr.mn.not.OwnerId == currentUser.Id && mnr.mn.mess.RoomId == r.Id).Count(),
                        
                        
                        
                        
                        
                        //s.Messages.Select(t => new
                        //{
                        //    unseen = t.MessNotifications.Where(n => n.Read == DateTime.Parse("0001-01-01 00:00:00.0000000") && n.OwnerId == currentUser.Id).Count(),

                        //}),
                        messages = r.Messages
                        .Select(m => new 
                        { m.Id, m.RoomId, m.CreatedAt, m.SenderId, m.Sender.Name, m.Content })
                        .OrderByDescending(m => m.CreatedAt).Take(10).ToList(), 
                        participants = r.Users
                        .Where(ru => ru.Id != currentUser.Id)
                        .Select(ru => new { ru.Id, ru.Name }) })
                        .OrderByDescending(w=>w.Latest.CreatedAt)
                        .ToList()
                })
                
                .FirstOrDefaultAsync();


            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllManagers()
        {
            var role = await _context.Roles.Where(r => r.Role1 == "Manager").FirstOrDefaultAsync();
            var user = await _context.staff
                        .Include(s => s.User)
                        .ThenInclude(u => u.Roles)
                        .Where(s => s.User.Roles.Contains(role))
                        .Select(s => new { s.Id, s.Name, s.Gender, s.Address, s.User.Email })
                        .ToListAsync();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _context.staff.ToListAsync());
        }
    }

}
