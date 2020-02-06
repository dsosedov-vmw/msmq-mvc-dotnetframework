using System;
using System.IO;
using System.Messaging;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ProbeController : Controller
    {
        const string QueueName = @".\private$\msmq-mvc-dotnetframework-probe";

        [HttpGet]
        public ActionResult Index()
        {
            var queue = new MessageQueue(QueueName);

            try
            {
                if (!MessageQueue.Exists(queue.Path))
                {
                    MessageQueue.Create(queue.Path);
                }

                ViewData["Status"] = "Connection established successfully.";
            }
            catch (Exception ex)
            {
                ViewData["Status"] = ex.Message;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Send(Models.Message message)
        {
            var queue = new MessageQueue(QueueName);

            try
            {
                queue.Send(message.Text);

                ViewData["Status"] = "Message sent successfully.";
            }
            catch (Exception ex)
            {
                ViewData["Status"] = ex.Message;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Receive()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Receive(Models.Message message)
        {
            var queue = new MessageQueue(QueueName);

            Message msg;

            try
            {
                msg = queue.Receive(TimeSpan.FromSeconds(2));

                msg.Formatter = new ActiveXMessageFormatter();

                string msgBody;

                using (var sr = new StreamReader(msg.BodyStream))
                {
                    msgBody = sr.ReadToEnd();
                }

                ViewData["Status"] = msgBody;
            }
            catch (Exception ex)
            {
                ViewData["Status"] = ex.Message;
            }

            return View();
        }
    }
}
