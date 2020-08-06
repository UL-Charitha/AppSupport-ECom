using PayLater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayLater.DB
{
    public class DBProcessor
    {
        AIREntities db;
        public DBProcessor() {
            db = new AIREntities();
        }

        internal string AddServiceReqInfo(string r1, string r2)
        {
            try
            {
                SERVICE_REQUESTS dbItem = new SERVICE_REQUESTS();
                dbItem.CreatedDateTime = DateTime.Now;
                dbItem.Requester = r1;
                dbItem.RequesterInfo = r2;

                db.SERVICE_REQUESTS.Add(dbItem);
                db.SaveChanges();
                return "saved";
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}