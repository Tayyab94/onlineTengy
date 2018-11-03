using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using onlineTengy.Services;
using onlineTengy.Utility;

namespace onlineTengy.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        //Send Email to Customer for the confirmation of the order that he / here has been ordered
        public static Task SendOrderStatusAsync(this IEmailSender emailSender, string email, string OrderNo, string OrderStatus)
        {
            string Subject = "";
            string Message = "";

            if(OrderStatus==SD.StatusCancelled)
            {
                Subject = "Order Cancelled";

                Message = "Order No :" + OrderNo + " has been Cancelled! please feel free to contact us if you have any Question.";
            }

            if (OrderStatus == SD.StatusSubmitted)
            {
                Subject = "Order Submitted";

                Message = "Order No :" + OrderNo + " has been Submitted.";
            }

            if (OrderStatus == SD.StatusReady)
            {
                Subject = "Order Ready for PickUp";

                Message = "Order No :" + OrderNo + " is ready for PickUp now.";
            }
            if (OrderStatus == SD.StatusCompleted)
            {
                Subject = "Order Completed";

                Message = "Order No :<strong>" + OrderNo + "</strong> has been Completed Now... 1 Hour me ap k Pass";
            }

            //Calling EmailSender Method that have been define in the Services/emailSenderExtension  Class

            return emailSender.SendEmailAsync(email, Subject, Message);
        }
    }
}
