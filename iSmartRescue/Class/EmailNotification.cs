using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace iSmartRescue.Class
{
    public class EmailNotification
    {
        public NetworkCredential login;
        public SmtpClient client;
        public MailMessage msg;
        public EmailNotification()
        {
            login = new NetworkCredential("anneblance000", "teamcareftw");
            client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = login;
            msg = new MailMessage { From = new MailAddress(txtboxUname.Text + txtboxSmtp.Text.Replace("smtp.", "@"), "Anne B. Lance", Encoding.UTF8) };
            msg.To.Add(new MailAddress(txtboxTo.Text));
            if (!string.IsNullOrEmpty(txtboxCC.Text))
                msg.To.Add(new MailAddress(txtboxCC.Text));
            msg.Subject = txtboxSubject.Text;
            String healthStatus = "";
            if (checkBoxHealthCard.Checked)
            {
                healthStatus += "YES";
            }
            else
            {
                healthStatus += "NO";
            }
            String info = "";
            if (textBoxHealthCard.Text == "")
            {
                info += "Not Applicable";
            }
            string message = "We would like to request for your ambulance service, here are the details of the patient: <br>";
            msg.Body = message +
                "Patient Name: " + textBoxPatient.Text + "<br>" +
                "Emergency Type: " + comboBoxEmergency.Text + "<br>" +
                "Health Card Present?: " + healthStatus + "<br>" +
                "Health Card Information: " + info + "<br>" +
                "Additional Notes: " + textBoxNotes.Text;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallBack);
            string userstate = "Sending...";
            client.SendAsync(msg, userstate);
        }

    }
}