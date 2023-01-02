using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Globalization;
using System.Security.Claims;
using System.Xml.Linq;
using TinTucGameAPI.Models;
using TinTucGameAPI.Models2;
//using TinTucGameAPI.Models2;
//using Message = TinTucGameAPI.Models2.Message;
//using Participants = TinTucGameAPI.Models2.Participant;
//using Room = TinTucGameAPI.Models2.Room;

namespace TinTucGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly doan5Context _context;
        private readonly TinTucGameAPI.Models2.doan5Context2 _context2;

        public ChatController(doan5Context context, Models2.doan5Context2 context2)
        {
            _context = context;
            _context2 = context2;
        }
        //[HttpPost("create-room")]
        //public async Task<IActionResult> CreateRooms(Dictionary<string,object> form)
        //{
        //    var roomId = Guid.NewGuid().ToString();
        //    var creatorId = form["creatorId"].ToString();
        //    var listParticipants = JsonConvert.DeserializeObject<string[]>(form["userId"].ToJson());

        //    var roomExists = await _context.Rooms
        //                //.Join(_context.Participants, r => r.Id, p => p.RoomId, (r, p) => new { room = r, par = p })
        //                .Where(rp => rp.CreatorId == creatorId)
        //                //.Select(rp=> new {rp.par})
        //                .ToListAsync();

        //    var user = await _context.Participants
        //        .Where(p => listParticipants
        //        .Contains(p.UserId))
        //        .ToListAsync();



        //    var creator = await _context.staff.FindAsync(creatorId);
        //    var createdAt = DateTime.Now;
        //    var name = creator.Name +" - ";
        //    List<Participants> participants = new List<Participants>();
        //    for(int i =0;i<listParticipants.Length;i++)
        //    {
        //        var user = await _context.Participants.Where(p=>p.UserId == listParticipants[i]);
        //        if(roomExists.)

        //        if(i== listParticipants.Length -1)
        //            name += user.Name;
        //        else name += user.Name + " - ";

        //        participants.Add(new Participants
        //        {
        //            RoomId = roomId,
        //            UserId = user.Id
        //        });
        //    }

        //    participants.Add(new Participants
        //    {
        //        RoomId = roomId,
        //        UserId = creator.Id
        //    });

        //    Room room = new Room()
        //    {
        //        Id = roomId,
        //        CreatorId = creator.Id,
        //        CreatedAt = createdAt,
        //        Name = name
        //    };

        //    await _context.Rooms.AddAsync(room);

        //    await _context.Participants.AddRangeAsync(participants);

        //    await _context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        participants = participants,
        //        room = room
        //    });
        //}

        //[HttpGet("get-messages")]
        //public async Task<IActionResult> GetMessages(string roomId, int page = 1, int pageSize = 10)
        //{
        //    var room = await _context.Rooms.FindAsync(roomId);
        //    var messages = await _context.Messages
        //                    .Where(m => m.RoomId == room.Id)
        //                    .Skip((page-1) *pageSize)
        //                    .Take(pageSize)
        //                    .OrderByDescending(m=>m.CreatedAt)
        //                    .ToListAsync();           
        //    return Ok(new
        //    {
        //        room = room,
        //        messages = messages
        //    });
        //}

        [HttpPost("send-messages")]
        public async Task<IActionResult> SendMessages(Dictionary<string, object> form)
        {
            var message = JsonConvert.DeserializeObject<Models.Message>(form["message"].ToJson());

            
            var listParticipants = JsonConvert.DeserializeObject<string[]>(form["participants"]?.ToJson());


            var room = await _context.Participants
                             .Where(p => p.RoomId == message.RoomId)
                             .ToListAsync();

            //message.Id = Guid.NewGuid().ToString();

            message.CreatedAt = DateTime.Now;



            if (room.Count > 0)
            {
                var currentRoom = await _context.Rooms.FindAsync(message.RoomId);

                await _context.Messages.AddAsync(message);

                await _context.SaveChangesAsync();

                currentRoom.LatestId = message.Id;



                foreach (var user in room)
                {
                    if (user.UserId != message.SenderId)
                    {
                        MessNotifications notifications = new MessNotifications()
                        {
                            Id = Guid.NewGuid().ToString(),
                            MessId = message.Id,
                            OwnerId = user.UserId,
                            CreatedAt = DateTime.Now
                        };
                        await _context.MessNotifications.AddAsync(notifications);
                    }

                }
            }
            else
            {
                var sender = await _context.staff.FindAsync(message.SenderId);
                var name = sender.Name + " - ";
                List<Models.Participants> participants = new List<Models.Participants>();
                List<MessNotifications> messNotifications = new List<MessNotifications>();
                Models.Room newRoom = new Models.Room()
                {
                    Id = message.RoomId,
                    CreatedAt = DateTime.Now

                };


                for (int i = 0; i < listParticipants.Length; i++)
                {
                    var user = await _context.staff.Where(p => p.Id == listParticipants[i]).FirstOrDefaultAsync();
                    if (i == listParticipants.Length - 1)
                        name += user.Name;
                    else name += user.Name + " - ";

                    participants.Add(new Models.Participants
                    {
                        RoomId = message.RoomId,
                        UserId = user.Id
                    });

                    messNotifications.Add(new MessNotifications
                    {
                        Id = Guid.NewGuid().ToString(),
                        MessId = message.Id,
                        OwnerId = user.Id,
                        CreatedAt = DateTime.Now
                    });

                    //MessNotifications notifications = new MessNotifications()
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    MessId = message.Id,
                    //    OwnerId = user.Id,
                    //    CreatedAt = DateTime.Now
                    //};
                    //await _context.MessNotifications.AddAsync(notifications);
                }

                participants.Add(new Models.Participants
                {
                    RoomId = message.RoomId,
                    UserId = sender.Id
                });

                newRoom.Name = name;          

                await _context.Rooms.AddAsync(newRoom);

                await _context.SaveChangesAsync();

                await _context.Messages.AddAsync(message);

                await _context.SaveChangesAsync();

                await _context.MessNotifications.AddRangeAsync(messNotifications);

                await _context.SaveChangesAsync();               

                newRoom.LatestId = message.Id;

                await _context.Participants.AddRangeAsync(participants);

                await _context.SaveChangesAsync();

                return Ok(new { 
                    room = newRoom,
                    message = message,
                    participants = participants
                });
            }
           
            await _context.SaveChangesAsync();

            var roomExists = await _context.Rooms.FindAsync(message.RoomId);

            return Ok(new
            {
                room = roomExists,
                message = message
            });
        }


        [HttpPost("get-messages")]
        public async Task<IActionResult> ReadMessages(Dictionary<string,string> form)
        {
            var ownerId = form["ownerId"].ToString();
            var roomId = form["roomId"].ToString();

            var currentRoom = await _context2.Rooms.FindAsync(roomId);

            var unseen = await _context2.Messages
                  .Where(m => m.RoomId == roomId)
                  .Join(_context2.MessNotifications, m => m.Id, n => n.MessId, (m, n) => new { message = m, not = n })
                  .Where(mn => mn.not.OwnerId == ownerId && mn.not.Read == DateTime.Parse("0001-01-01 00:00:00.0000000"))
                  .OrderByDescending(mn => mn.message.CreatedAt)
                  .ToListAsync();


            //var result = await _context2.Messages
            //            .Where(m => m.RoomId == roomId)
            //            .Join(_context2.MessNotifications, m => m.Id, n => n.MessId, (m, n) => new { message = m, not = n })
            //            .Where(mn => mn.not.OwnerId == ownerId)
            //            .OrderByDescending(mn=>mn.message.CreatedAt)
            //            .Select(mn => new
            //            {
            //                notification = new { mn.not.Id, mn.not.MessId, mn.not.Read, mn.not.CreatedAt },
            //                message = new {mn.message.Id, mn.message.RoomId, mn.message.SenderId, mn.message.Content, mn.message.CreatedAt}
            //            })
            //            .Take(10)
            //            .ToListAsync();

            var data = await _context2.Messages
                        .Include(n => n.MessNotifications)
                        .Where(m => m.RoomId == roomId)
                        .OrderByDescending(m => m.CreatedAt)
                        .Take(10)
                        .Select(m => new
                        {
                            message = new
                            {
                                m.Id,
                                m.RoomId,
                                m.SenderId,
                                m.Content,
                                m.CreatedAt,
                                messNotification =  m.MessNotifications.FirstOrDefault()
                            }

                        })
                        .ToListAsync();

            foreach (var item in unseen)
            {
                var mn = await _context.MessNotifications.FindAsync(item.not.Id);
                mn.Read = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(new { unseenCount = unseen.Count,messages = data, room = currentRoom });
        }


        [HttpGet("truncate")]
        public async Task<IActionResult> GetRoom()
        {
            _context.Participants.RemoveRange(_context.Participants.ToList());
            _context.Messages.RemoveRange(_context.Messages.ToList());
            _context.MessNotifications.RemoveRange(_context.MessNotifications.ToList());
            _context.Rooms.RemoveRange(_context.Rooms.ToList());
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
