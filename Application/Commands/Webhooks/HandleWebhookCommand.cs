using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Webhooks
{
    public record HandleWebhookCommand(string Cvr,string Entity,string Url,int OldId,string Provider) : IRequest;
}
