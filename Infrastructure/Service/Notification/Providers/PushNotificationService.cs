using Application.Services.Notification.Providers;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces.Notification;

namespace Infrastructure.Service.Notification.Providers
{
    public class PushNotificationService : IPushNotificationService
    {
        public async Task<bool> SendAsync(string title, string body, string deviceToken, CancellationToken cancellationToken = default)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = title,
                    Body = body
                }
            };
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    }
