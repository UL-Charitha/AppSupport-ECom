﻿using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Processor proc = new Processor();

            //proc.DoWork();
            proc.TestSentBox(null);

            //var response = proc.ReadOutlookMsg();
        }

        //private static void DoWork()
        //{
        //    var App = new Microsoft.Office.Interop.Outlook.Application();
        //    MailItem mailItem = App.CreateItem(OlItemType.olMailItem);
        //    mailItem.Subject = "Hi test charitha";
        //    mailItem.To = "wter@lkk.lk";
        //    mailItem.CC = "cclol@lkk.lk";
        //    mailItem.BCC = "bccm@lkkk.lk";
        //    mailItem.Body = "Hi this is the body of the msg"; 

        //    string FileAtachment = @"D:\Projects\CSP\Repo.RnD\ConsoleCSP\ConsoleApp1\pic1.jpg";
        //    string msgSavePath = @"D:\Projects\CSP\Repo.RnD\ConsoleCSP\ConsoleApp1\msgOut.msg";
        //    // make sure a filename was passed
        //    if (string.IsNullOrEmpty(FileAtachment) == false)
        //    {
        //        // need to check to see if file exists before we attach !
        //        if (!File.Exists(FileAtachment))
        //            Console.WriteLine("Attached document " + FileAtachment + " does not exist", "File Error");
        //        else
        //        {
        //            Attachment attachment = mailItem.Attachments.Add(FileAtachment, Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);
        //        }
        //    }
        //    mailItem.Display();     // display the email
        //    mailItem.SaveAs(msgSavePath, Type.Missing);
        //}
    }
}
