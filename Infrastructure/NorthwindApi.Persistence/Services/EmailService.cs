// NorthwindApi.Persistence/Services/EmailService.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NorthwindApi.Application.Interfaces.Infrastructure;

namespace NorthwindApi.Persistence.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOrderConfirmationAsync(
        int orderId,
        string customerEmail,
        string companyName,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            _configuration["Email:FromName"],
            _configuration["Email:FromEmail"]));

        message.To.Add(new MailboxAddress(companyName, customerEmail));

        message.Subject = $"Sipariş Onayı - #{orderId}";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <html>
                <body style="font-family: Arial, sans-serif; padding: 20px;">
                    <h2 style="color: #2c3e50;">Sipariş Onayı</h2>
                    <p>Sayın <strong>{companyName}</strong>,</p>
                    <p>Siparişiniz başarıyla oluşturuldu.</p>
                    <table style="border-collapse: collapse; width: 100%;">
                        <tr style="background-color: #f2f2f2;">
                            <td style="padding: 8px; border: 1px solid #ddd;"><strong>Sipariş No</strong></td>
                            <td style="padding: 8px; border: 1px solid #ddd;">#{orderId}</td>
                        </tr>
                        <tr>
                            <td style="padding: 8px; border: 1px solid #ddd;"><strong>Tarih</strong></td>
                            <td style="padding: 8px; border: 1px solid #ddd;">{DateTime.UtcNow:dd.MM.yyyy HH:mm}</td>
                        </tr>
                    </table>
                    <br/>
                    <p>Teşekkür ederiz.</p>
                    <p style="color: #7f8c8d;"><em>Northwind API</em></p>
                </body>
                </html>
                """
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _configuration["Email:Host"],
            int.Parse(_configuration["Email:Port"]!),
            SecureSocketOptions.StartTls,
            cancellationToken);

        await client.AuthenticateAsync(
            _configuration["Email:Username"],
            _configuration["Email:Password"],
            cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}