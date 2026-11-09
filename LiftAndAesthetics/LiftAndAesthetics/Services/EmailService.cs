using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LiftAndAesthetics.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IWebHostEnvironment _env;

        // GitHub Team email addresses (Prezentarea 3 B6)
        private static readonly string[] TeamEmails = new[]
        {
            "dariusploae@yahoo.com",
            "peca.ana.p2f@student.ucv.ro",
            "nica.david.p7c@student.ucv.ro"
        };

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }

        public async Task SendOrderConfirmationEmailAsync(Order order)
        {
            if (order == null) return;

            string subject = $"Order Confirmation #{order.OrderNumber} — Lift & Aesthetics";

            // Plain text body
            var sb = new StringBuilder();
            sb.AppendLine("==================================================");
            sb.AppendLine("           LIFT & AESTHETICS - ORDER RECEIPT      ");
            sb.AppendLine("==================================================");
            sb.AppendLine($"Order Number  : {order.OrderNumber}");
            sb.AppendLine($"Date          : {order.OrderDate:dd MMM yyyy, HH:mm}");
            sb.AppendLine($"Customer Name : {order.CustomerName}");
            sb.AppendLine($"Customer Email: {order.CustomerEmail}");
            sb.AppendLine($"Team Copy To  : {string.Join(", ", TeamEmails)}");
            sb.AppendLine($"Delivery To   : {order.DeliveryAddress}, {order.DeliveryCity}");
            sb.AppendLine($"Method        : {order.DeliveryMethod} ({order.DeliveryCost:0.00} LEI)");
            sb.AppendLine($"Status        : {order.Status}");
            sb.AppendLine($"Payment Via   : {order.PaymentMethod}");
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine("ITEMS PURCHASED:");
            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    string name = item.Product?.Name ?? $"Product #{item.ProductId}";
                    sb.AppendLine($" • {name} x{item.Quantity} @ {item.UnitPrice:0.00} LEI = {item.TotalPrice:0.00} LEI");
                }
            }
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine($"Subtotal      : {order.Subtotal:0.00} LEI");
            if (order.DiscountAmount > 0)
            {
                sb.AppendLine($"Discount ({order.DiscountCode}): -{order.DiscountAmount:0.00} LEI");
            }
            sb.AppendLine($"Shipping      : {order.DeliveryCost:0.00} LEI");
            sb.AppendLine($"TOTAL PAID    : {order.TotalAmount:0.00} LEI");
            sb.AppendLine("==================================================");
            sb.AppendLine("Thank you for your order! Train hard, stay aesthetic.");

            string textBody = sb.ToString();

            // HTML formatted receipt
            var htmlSb = new StringBuilder();
            htmlSb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'/><style>");
            htmlSb.AppendLine("body { font-family: 'Segoe UI', Arial, sans-serif; background-color: #0a0c10; color: #f1f5f9; padding: 20px; }");
            htmlSb.AppendLine(".card { background: #141721; max-width: 650px; margin: 0 auto; border: 1px solid #282e3e; border-top: 4px solid #dc3545; border-radius: 8px; padding: 30px; }");
            htmlSb.AppendLine("h1 { color: #ffffff; letter-spacing: 2px; font-size: 26px; margin-bottom: 5px; }");
            htmlSb.AppendLine(".badge { background: #28a745; color: #ffffff; padding: 5px 12px; border-radius: 4px; font-weight: bold; font-size: 13px; }");
            htmlSb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            htmlSb.AppendLine("th { text-align: left; border-bottom: 2px solid #dc3545; padding: 10px; color: #94a3b8; font-size: 13px; }");
            htmlSb.AppendLine("td { padding: 10px; border-bottom: 1px solid #242938; font-size: 14px; color: #e2e8f0; }");
            htmlSb.AppendLine(".total-row { font-size: 18px; font-weight: bold; color: #dc3545; }");
            htmlSb.AppendLine(".notice { background: rgba(13,202,240,0.1); border-left: 3px solid #0dcaf0; padding: 12px; margin-top: 25px; font-size: 13px; color: #cbd5e1; }");
            htmlSb.AppendLine("</style></head><body><div class='card'>");
            htmlSb.AppendLine("<h1>LIFT & AESTHETICS - ORDER RECEIPT</h1>");
            htmlSb.AppendLine($"<p style='color: #94a3b8;'>Order <strong>#{order.OrderNumber}</strong> &bull; {order.OrderDate:dd MMM yyyy, HH:mm}</p>");
            htmlSb.AppendLine("<p><span class='badge'>STATUS: PAID</span></p>");
            htmlSb.AppendLine("<hr style='border: 0; border-top: 1px solid #282e3e;'/>");
            htmlSb.AppendLine($"<p><strong>Customer:</strong> {order.CustomerName} ({order.CustomerEmail})<br/>");
            htmlSb.AppendLine($"<strong>Delivery Address:</strong> {order.DeliveryAddress}, {order.DeliveryCity}<br/>");
            htmlSb.AppendLine($"<strong>Shipping Method:</strong> {order.DeliveryMethod} ({order.DeliveryCost:0.00} LEI)<br/>");
            htmlSb.AppendLine($"<strong>Payment Verified:</strong> {order.PaymentMethod}</p>");
            htmlSb.AppendLine("<table><thead><tr><th>Product</th><th>Qty</th><th style='text-align:right;'>Price</th></tr></thead><tbody>");
            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    string name = item.Product?.Name ?? $"Product #{item.ProductId}";
                    htmlSb.AppendLine($"<tr><td>{name}</td><td>{item.Quantity}</td><td style='text-align:right;'>{item.TotalPrice:0.00} LEI</td></tr>");
                }
            }
            htmlSb.AppendLine($"<tr><td colspan='2' style='text-align:right;'>Subtotal:</td><td style='text-align:right;'>{order.Subtotal:0.00} LEI</td></tr>");
            if (order.DiscountAmount > 0)
            {
                htmlSb.AppendLine($"<tr><td colspan='2' style='text-align:right; color:#28a745;'>Discount ({order.DiscountCode}):</td><td style='text-align:right; color:#28a745;'>-{order.DiscountAmount:0.00} LEI</td></tr>");
            }
            htmlSb.AppendLine($"<tr><td colspan='2' style='text-align:right;'>Shipping:</td><td style='text-align:right;'>{order.DeliveryCost:0.00} LEI</td></tr>");
            htmlSb.AppendLine($"<tr class='total-row'><td colspan='2' style='text-align:right;'>Grand Total:</td><td style='text-align:right;'>{order.TotalAmount:0.00} LEI</td></tr>");
            htmlSb.AppendLine("</tbody></table>");
            htmlSb.AppendLine("<div class='notice'>");
            htmlSb.AppendLine($"This transactional confirmation receipt has been dispatched to customer <strong>{order.CustomerEmail}</strong> and team lead <strong>dariusploae@yahoo.com</strong>.");
            htmlSb.AppendLine("</div></div></body></html>");

            string htmlBody = htmlSb.ToString();

            // 1. Save HTML receipt to wwwroot/receipts so it can be previewed/opened directly in browser
            try
            {
                string webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                string receiptsDir = Path.Combine(webRoot, "receipts");
                Directory.CreateDirectory(receiptsDir);
                string htmlPath = Path.Combine(receiptsDir, $"{order.OrderNumber}.html");
                await File.WriteAllTextAsync(htmlPath, htmlBody);
                _logger.LogInformation("HTML Receipt written to {Path}", htmlPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write HTML receipt.");
            }

            // 2. Save text receipt to App_Data/Emails
            try
            {
                string emailsDir = Path.Combine(_env.ContentRootPath, "App_Data", "Emails");
                Directory.CreateDirectory(emailsDir);
                string textPath = Path.Combine(emailsDir, $"{order.OrderNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                await File.WriteAllTextAsync(textPath, textBody);
                _logger.LogInformation("Order confirmation email saved to {FilePath}", textPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write text email file to disk.");
            }

            // 3. Send over SMTP / Mailtrap to customer and GitHub Team
            string? host = _configuration["Mailtrap:Host"] ?? _configuration["Smtp:Host"];
            string? portStr = _configuration["Mailtrap:Port"] ?? _configuration["Smtp:Port"];
            string? username = _configuration["Mailtrap:Username"] ?? _configuration["Smtp:Username"];
            string? password = _configuration["Mailtrap:Password"] ?? _configuration["Smtp:Password"];

            if (!string.IsNullOrWhiteSpace(host) && int.TryParse(portStr, out int port) && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    using var client = new SmtpClient(host, port)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(username, password),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Timeout = 15000
                    };

                    string senderEmail = _configuration["Smtp:FromEmail"] ?? username;
                    // If Yahoo or Gmail SMTP is used, the From address MUST match the authenticated username
                    if (host.Contains("yahoo", StringComparison.OrdinalIgnoreCase) || host.Contains("gmail", StringComparison.OrdinalIgnoreCase))
                    {
                        senderEmail = username;
                    }

                    var mail = new MailMessage
                    {
                        From = new MailAddress(senderEmail, _configuration["Smtp:SenderName"] ?? "Lift & Aesthetics Store"),
                        Subject = subject,
                        Body = htmlBody,
                        IsBodyHtml = true
                    };

                    // Add primary customer
                    if (!string.IsNullOrWhiteSpace(order.CustomerEmail))
                    {
                        mail.To.Add(order.CustomerEmail);
                    }

                    // Add team recipients
                    foreach (var teamEmail in TeamEmails)
                    {
                        if (!string.Equals(teamEmail, order.CustomerEmail, StringComparison.OrdinalIgnoreCase))
                        {
                            mail.CC.Add(teamEmail);
                        }
                    }

                    await client.SendMailAsync(mail);
                    _logger.LogInformation("Order confirmation email sent via SMTP to {Customer} and Team.", order.CustomerEmail);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[SMTP SUCCESS] Emailul de confirmare #{order.OrderNumber} a fost trimis prin {host} la {order.CustomerEmail} si CC la echipa!");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SMTP delivery failed to {Host}:{Port}: {Message}", host, port, ex.Message);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[SMTP ERROR] Trimiterea emailului prin {host}:{port} a esuat: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                _logger.LogWarning("SMTP credentials not provided in appsettings.json. Email saved locally only.");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[SMTP SKIPPED] Credentialele SMTP (Username/Password) sunt necompletate in appsettings.json.");
                Console.WriteLine($"[SMTP SKIPPED] Factura electronica a fost generata si salvata local in wwwroot/receipts/{order.OrderNumber}.html.");
                Console.ResetColor();
            }
        }
    }
}
