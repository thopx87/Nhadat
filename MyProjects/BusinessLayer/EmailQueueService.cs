using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using DataLayer;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class EmailQueueService : BaseService<Entities.EmailQueue>
    {
        private string className { get { return this.GetType().Name; } }
        public EmailQueueService() : base() { }

        public int Insert(Entities.EmailQueue e)
        {
            DataLayer.EmailQueue email = new DataLayer.EmailQueue();
            email.Subject = e.Subject;
            email.Body = e.Body;
            email.SendTo = e.SendTo;
            email.Bcc = e.Bcc;
            email.Status = e.Status;
            email.CreatedTime = DateTime.Now;
            try
            {
                Context.EmailQueues.InsertOnSubmit(email);
                Context.SubmitChanges();
                return e.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            
        }
    }
}
