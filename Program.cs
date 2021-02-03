using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Telegram.Bot;


namespace QRCodeBot
{
    class Program
    {
        static void Main(string[] args)
        {
           
            TelegramBotClient telegramBot = new TelegramBotClient(Startup.token);


            telegramBot.OnMessage += (e, arg) =>
            {
                string txt = "";

                try
                {
                    if (arg.Message.Text == "/start")
                    {
                        txt = "Hi. Send photo or text and i will turn it into QR code";  
                    }
                    else
                    {
                        string textToConvert = "ERROR: QR didn't generated";
                        textToConvert = arg.Message.Text;
                        QRCodeGenerator qRCode = new QRCodeGenerator();
                        QRCodeData data = qRCode.CreateQrCode(textToConvert, QRCodeGenerator.ECCLevel.Q);
                        QRCode code = new QRCode(data);

                        Bitmap bitmap = new Bitmap(code.GetGraphic(10));

                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, ImageFormat.Png);

                            ms.Position = 0;
                            telegramBot.SendPhotoAsync(
                            chatId: arg.Message.Chat.Id,
                            ms
                            );
                        }
   
                    }
                }
                catch (Exception)
                {
                    txt = "Sorry, but i am working only with text";
                }
                finally
                {
                    telegramBot.SendTextMessageAsync(
                        chatId: arg.Message.Chat.Id,
                        text: txt
                        );
                }

                
            };

            telegramBot.StartReceiving();

            Console.WriteLine($"{telegramBot.GetMeAsync().Result.Username} started");


            Console.ReadLine();

        }
    }
}
