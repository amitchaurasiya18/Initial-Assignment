using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.DTO;

namespace SchoolAPI.Listener
{
    public class StudentEventEmailListener : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IEmailService _emailService;
        public StudentEventEmailListener(ISubscriber subscriber, IEmailService emailService)
        {
            _subscriber = subscriber;
            _emailService = emailService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> headers)
        {
            try
            {
                Console.WriteLine("Received message: " + message);

                var studentEventMessage = JsonConvert.DeserializeObject<StudentEventMessage>(message);

                if (studentEventMessage == null)
                {
                    Console.WriteLine("Failed to deserialize message.");
                    return false;
                }

                string subject = "Student Event Notification";
                string body = string.Empty;

                switch (studentEventMessage.EventType)
                {
                    case "created":
                        subject = "Welcome to Our School!";
                        body = $"Dear {studentEventMessage.StudentName},\n\n" +
                               $"Welcome to our school! Your student ID is {studentEventMessage.StudentId}.\n\n" +
                               $"Best Regards,\nSchool Team";
                        break;

                    case "updated":
                        subject = "Your Student Information Was Updated";
                        body = $"Dear {studentEventMessage.StudentName},\n\n" +
                               $"Your student information has been updated. Your student ID is {studentEventMessage.StudentId}.\n\n" +
                               $"Best Regards,\nSchool Team";
                        break;

                    case "deleted":
                        subject = "Your Student Profile Was Deleted";
                        body = $"Dear {studentEventMessage.StudentName},\n\n" +
                               $"We're sorry to inform you that your student profile has been deleted. Your student ID was {studentEventMessage.StudentId}.\n\n" +
                               $"Best Regards,\nSchool Team";
                        break;
                }

                _emailService.SendEmail(studentEventMessage.StudentEmail, subject, body);
                Console.WriteLine($"Email sent to: {studentEventMessage.StudentEmail}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                return false;
            }

        }
    }
}