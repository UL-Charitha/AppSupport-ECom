using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class MSGReader
    {
        internal string ReadMsg(string path)
        {
			try
			{
				var App = new Microsoft.Office.Interop.Outlook.Application();
				//var xxxx = App.Session.GetItemFromID("");
				var mailItem = (MailItem) App.Session.OpenSharedItem(path); 
				//as Microsoft.Office.Interop.Outlook.MailItem;
				//string body = mailx.HTMLBody;
				//int att = mailx.Attachments.Count;
				var entryId = mailItem.EntryID;
				mailItem.Display();
				//var idx = App.Session.GetItemFromID("1");
				return entryId;
			}
			catch (System.Exception)
			{
				throw;
			}
        }

		internal string GetEntryIdByMsgFile(string path)
		{
			try
			{
				var App = new Microsoft.Office.Interop.Outlook.Application();
				var mailItem = (MailItem)App.Session.OpenSharedItem(path);
				mailItem.Display();
				var entryID = mailItem.EntryID;
				mailItem = null;
				return entryID;
			}
			catch (System.Exception)
			{
				throw;
			}
		}

		internal void ReadMsgByEntryId(string id)
		{
			var App = new Microsoft.Office.Interop.Outlook.Application();
			var NS = App.GetNamespace("mapi");
			
			// Oopen mail by EntryID
			var mailItem = (MailItem)NS.GetItemFromID(id);
			mailItem.Display();
		}

		internal void ReadMsgRealTime(string path)
		{
			try
			{
				var App = new Microsoft.Office.Interop.Outlook.Application();
				// Get the MAPI namespace.
				var NS = App.GetNamespace("mapi");
				//Get the Inbox folder.
				MAPIFolder oInbox = NS.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderInbox);

				

				//Get the Items collection in the Inbox folder.
				Items inboxItems = oInbox.Items;
				var count = inboxItems.Count;

				// Get the first message.
				MailItem Msg1 = (MailItem)inboxItems.GetFirst();

				MailItem Msg2 = (MailItem)inboxItems.GetLast();
				Console.WriteLine(Msg2.ConversationID);
				var tempx = Msg2.ConversationTopic;
				Console.WriteLine(Msg2.EntryID);
				Console.WriteLine(Msg2.ReceivedTime);
				Console.ReadLine();
				//Msg2.Display();
			}
			catch (System.Exception ex)
			{
				var xxx = ex.Message;
				// https://support.microsoft.com/en-sg/help/310258/how-to-use-the-microsoft-outlook-object-library-to-retrieve-a-message
			}
		}
	}
}
