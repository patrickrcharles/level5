//using System.Net;
//using System.Net.Mail;
//using System.Net.Security;
//using System.Security.Cryptography.X509Certificates;
//using UnityEngine;

//public class SendEmail : MonoBehaviour
//{
//    public string fromEmail = "yourGmailAccountFromWhereYouWantTo//SendEmail";
//    public string toEmail = "WhomYouWantTo//SendEmail";
//    public string subject = "SubjectName";
//    public string body = "Body of the email";
//    public string password = "YourGmailAccountPassword";

//    public static SendEmail instance;
//    // Start is called before the first frame update
//    void Awake()
//    {
//        //EmailSending();
//        instance = this;
//    }

//    // Update is called once per frame
//    public void SendEmailOnEvent(string emailsubject, string emailbody)
//    {
//        MailMessage mail = new MailMessage();
//        mail.From = new MailAddress(fromEmail);
//        mail.To.Add(toEmail);
//        mail.Subject = emailsubject;
//        mail.Body = emailbody;
//        // you can use others too.
//        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com", 587);
//        //smtpServer.Port = 587;
//        smtpServer.Credentials = new System.Net.NetworkCredential(fromEmail, password) as ICredentialsByHost;
//        smtpServer.EnableSsl = true;
//        ServicePointManager.ServerCertificateValidationCallback =
//        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
//        { return true; };
//        smtpServer.Send(mail);
//    }
//}