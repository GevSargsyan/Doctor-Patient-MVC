using Diplomain_Sargsyan_Gevorg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diplomain_Sargsyan_Gevorg.Hubs
{
    public class ChatHub : Hub
    {
        private Context db;
        public ChatHub(Context context)
        {
            db = context;
        }

        public async Task SendPatient(string message, string userName, string to)
        {
            var userto = db.Patients.Include(x=>x.User).FirstOrDefault(x => x.User.Id == int.Parse(to));

            await Clients.User(Context.User.Identity.Name).SendAsync("Send", message, userName);
            await Clients.User(userto.User.Login).SendAsync("Send", message, userName);


        }

        public async Task SendDoctor(string message, string userName, string to)
        {
            var userto = db.Doctors.Include(x => x.User).FirstOrDefault(x => x.User.Id == int.Parse(to));

            await Clients.User(Context.User.Identity.Name).SendAsync("Send", message, userName);
            await Clients.User(userto.User.Login).SendAsync("Send", message, userName);


        }


        public async Task SendSaveDoctor(string message, string fromid, string to,string isdoctor)
        {
            Message mes = new Message() { Text = message, Timestamp = DateTime.Now, IdFrom = int.Parse(fromid), IdTo = int.Parse(to), IsDoctor = isdoctor };


            var user1 = db.Doctors.Include(x=>x.User).FirstOrDefault(x => x.User.Login == Context.User.Identity.Name);
            var user2 = db.Patients.Include(x => x.User).FirstOrDefault(x => x.User.Id == int.Parse(to));
            user1.Messages.Add(mes);
            user2.Messages.Add(mes);
            await db.SaveChangesAsync();

        }
        public async Task SendSavePatient(string message, string fromid, string to,string isdoctor)
        {
            Message mes = new Message() { Text = message, Timestamp = DateTime.Now, IdFrom = int.Parse(fromid), IdTo = int.Parse(to) ,IsDoctor=isdoctor};


            var user1 = db.Patients.Include(x=>x.User).FirstOrDefault(x => x.User.Login == Context.User.Identity.Name);
            var user2 = db.Doctors.Include(x => x.User).FirstOrDefault(x => x.User.Id == int.Parse(to));
            user1.Messages.Add(mes);
            user2.Messages.Add(mes);
            await db.SaveChangesAsync();

        }
    }
}
