using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Processor
    {

        internal async Task<string> TestApi()
        {

            CspModel model = null;
            string path = @"api/pnrTest/QWE123";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:13235/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                

                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    //model = await response.Content.ReadAsAsync<CspModel>();
                    string xxx = await response.Content.ReadAsStringAsync();
                    return xxx;
                }


                return null;
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }
        internal string ReadOutlookMsg()
        {
			try
			{
				return "";
			}
			catch (System.Exception)
			{
				throw;
			}
        }



        internal void DoWork()
        {
            var App = new Microsoft.Office.Interop.Outlook.Application();

            MailItem mailItem = App.CreateItem(OlItemType.olMailItem);

            mailItem.Subject = "Hi test charitha Live";
            mailItem.To = "tester123@lkk.lk";
            mailItem.CC = "cclol@lkk.lk";
            mailItem.BCC = "bccm@lkkk.lk";
            mailItem.Body = "Hi this is the body of the msg";

            string FileAtachment = @"D:\Projects\CSP\Repo.RnD\ConsoleCSP\ConsoleApp1\pic1.jpg";
            string msgSavePath = @"D:\Projects\CSP\Repo.RnD\ConsoleCSP\ConsoleApp1\msgOut.msg";

            // make sure a filename was passed
            if (string.IsNullOrEmpty(FileAtachment) == false)
            {
                // need to check to see if file exists before we attach !
                if (!File.Exists(FileAtachment))
                    Console.WriteLine("Attached document " + FileAtachment + " does not exist", "File Error");
                else
                {
                    Attachment attachment = mailItem.Attachments.Add(FileAtachment, Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);
                }
            }
            mailItem.Display();     // display the email
            mailItem.SaveAs(msgSavePath, Type.Missing);
            //TestMailSave(mailItem, msgSavePath); {For Future Ref if Nedded}
            //TestSentBox(mailItem);
        }

        public void TestSentBox(MailItem mailItems)
        {
            string msgSavePath = @"D:\Projects\CSP\Repo.RnD\ConsoleCSP\ConsoleApp1\msgSentOut.msg";
            var App = new Microsoft.Office.Interop.Outlook.Application();
            MailItem mailItem = App.CreateItem(OlItemType.olMailItem);

            mailItem.Subject = "Charitha Live 2020-05-22-Eve. Rep OK.";
            mailItem.To = "charithawaravita@gmail.com";
            mailItem.CC = "freshercm@gmail.com";
            mailItem.Body = "Hi this is the body of the msg. Noted. Happy : " + DateTime.Now.ToShortTimeString();

            //mailItem.DeleteAfterSubmit = false; // force storage to sent items folder (ignore user options)
            //var sentFolder = App.Session.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderSentMail);    
            //if (sentFolder != null)
            //    mailItem.SaveSentMessageFolder = sentFolder; // override the default sent items location

            mailItem.SaveAs(msgSavePath, Type.Missing);
            mailItem.Send();
        }



        #region For_Future_Ref
        private void TestMailSave(MailItem mailItem, string msgSavePath)
        {
            Inspector inspector = mailItem.GetInspector;
            inspector.Activate();

            ((ItemEvents_10_Event)mailItem).Send += MailItemSendHandler;
            void MailItemSendHandler(ref bool isSended)
            {
                mailItem.SaveAs(msgSavePath);
            }
            var _sentFolderItems = mailItem.SaveSentMessageFolder;
        }
        #endregion
    }
}
