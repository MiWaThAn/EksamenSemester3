using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared.Model.Webhook
{
    public class WebhookPayload
    {
        public string Cvr { get; set; }
        public string Entity { get; set; }
        public string Url { get; set; }
        [JsonPropertyName("old id")]  //Er kun sat fordi vi pt har et mellemrum i vores payload fra economci ved oldid
        public int OldId { get; set; }
        public string Provider { get; set; }


    }
}
