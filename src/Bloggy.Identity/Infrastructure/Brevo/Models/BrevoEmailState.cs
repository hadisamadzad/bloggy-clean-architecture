using System.ComponentModel;

namespace Bloggy.Identity.Infrastructure.Brevo.Models;

public enum BrevoEmailState
{
    [Description("Sent")]
    request = 1,

    [Description("Delivered")]
    delivered,

    [Description("Opened")]
    opened,

    [Description("Error")]
    error,

    [Description("InvalidEmail")]
    invalid_email,

    [Description("Deferred")]
    deferred,

    [Description("Complaint")]
    spam,

    [Description("Unsubscribed")]
    unsubscribed,

    [Description("Blocked")]
    blocked
}