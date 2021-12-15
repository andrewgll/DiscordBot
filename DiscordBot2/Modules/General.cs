using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot2.Modules
{

    public class General : ModuleBase<SocketCommandContext>
    {
        private const string USERAGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36";

        [Command("h")]
        [Alias("help")]
        public async Task SendHelpMenu()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Информация по командам:")
                .WithColor(Discord.Color.Blue)
                .AddField("Префикс", "s!")
                .AddField("Помощь","h | help")
                .AddField("Манга","m | manga")
                .AddField("Видео","v | video")
                .AddField("Картинка","p | picture")
                .AddField("Удалить сообщение", "c | clear")
                .AddField("Добавь параметр h для помощи по команде", "например p!c h")
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("logcat")]
        [Alias("log")]
        public async Task LogCat(params string[] args)
        {
            var result = "";
            if (args.Length > 0 && args.Contains("-r"))
            {
                await File.WriteAllTextAsync(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ErrorLog.txt", String.Empty);
                result = "Logcat cleared";
            }
            else
            {
                try
                {
                    var text = await File.ReadAllLinesAsync(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ErrorLog.txt");
                    result = string.Join('\n', text);
                }
                catch (Exception ex)
                {
                    result = "Logcat is empty";
                    
                }
            }
            await ReplyAsync(result);

        }


        [Command("nuke")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task NukeChannel()
        {
            try
            {
                await Context.Channel.TriggerTypingAsync();
                await ReplyAsync(@"Испепеляю этот канал (づ ◕‿◕ )づ");
                await Context.Channel.TriggerTypingAsync();
                await Task.Delay(1000);
                await ReplyAsync(@"Это может занять время σ(￣、￣〃)");
                await Context.Channel.TriggerTypingAsync();
                await Task.Delay(1000);
                await ReplyAsync("https://tenor.com/view/countdown-gif-11215002");
                await Task.Delay(5000);
                await ReplyAsync("https://tenor.com/view/boom-blast-gif-10797830");
                await Task.Delay(1200);
                SocketGuildChannel? socketGuildChannel = Context.Guild.Channels.FirstOrDefault(channel => channel.Id == Context.Channel.Id);
                var chn = await Context.Guild.CreateTextChannelAsync(Context.Channel.Name, chnl => {
                    chnl.Name = socketGuildChannel.Name;
                    chnl.Position = socketGuildChannel.Position;
                    chnl.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(socketGuildChannel.PermissionOverwrites);
                    chnl.CategoryId = Context.Guild.CategoryChannels.FirstOrDefault(category => category.Channels.Contains(socketGuildChannel))?.Id;

                });
                chn.SendMessageAsync($"Создан новый мир на руинах старого ＼(￣▽￣)／");

                await socketGuildChannel.DeleteAsync();
            }
            catch (Exception ex)
            {

                await Context.Channel.TriggerTypingAsync();
                await ReplyAsync(@"Кто-то помешал моим коварным планам ╮(￣_￣)╭");
            }
        }


        [Command("c")]
        [Alias("clean")]
        public async Task CleanMessage(string arg)
        {
            if (arg == "help")
            {
                var embed = new EmbedBuilder()
                  .WithTitle("Информация по команде clear")
                  .WithColor(Discord.Color.Red)
                  .AddField("@user time", "Удалить все сообщения за указанное время")
                  .AddField("@user", "Удалить последние сообщения юзера")
                  .AddField("@role", "Удалить последние сообщения этой роли")
                  .AddField("time", "Удалить сообщения за указанное время")
                  .AddField("без ничего", "Удалить все последние сообщения")
                  .AddField("**nuke**", "**ИСПЕПЕЛИТЬ КАНАЛ**")
                  .Build();

                await ReplyAsync(embed: embed);
            }

        }

        [Command("c")]
        [Alias("clean")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CleanMessage(SocketGuildUser socketGuildUser, int timeSecond)
        {

            var time = this.Context.Message.Timestamp.AddSeconds(-timeSecond);

            await ReplyAsync($"Удаляю все сообщения {socketGuildUser} за последние {timeSecond} секунд");

            var messages = Context.Channel.GetMessagesAsync().Flatten();

            int count = 0;


            await foreach (var message in messages)
            {
                if (message.Timestamp > time && message.Id == socketGuildUser.Id)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                    count++;
                }
                else
                    break;
            }
            await ReplyAsync($"Я удалила {count} сообщений для: за последние {timeSecond} секонд");
        }
        [Command("c")]
        [Alias("clean")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CleanMessage(int timeSecond)
        {

            var time = this.Context.Message.Timestamp.AddSeconds(-timeSecond);

            await ReplyAsync($"Удаляю все сообщения за последние {timeSecond} секунд");

            var messages = Context.Channel.GetMessagesAsync().Flatten();

            int count = 0;


            await foreach (var message in messages)
            {
                if (message.Timestamp > time)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                    count++;
                }
                else
                    break;
            }
            await ReplyAsync($"Я удалила {count} сообщений для: за последние {timeSecond} секонд");
        }

        [Command("c")]
        [Alias("clean")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CleanMessage(SocketGuildUser socketGuildUser)
        {

            await ReplyAsync($"Удаляю сообщения {socketGuildUser.Username}");

            var messages = Context.Channel.GetMessagesAsync(100).Flatten();

            int count = 0;

            await foreach (var message in messages)
            {
                if(message.Author.Id == socketGuildUser.Id)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                    count++;
                }
            }

            await ReplyAsync($"Я удалила {count} сообщений для: {socketGuildUser.Id}#{socketGuildUser.Username}");
        }

        [Command("c")]
        [Alias("clean")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CleanMessage(SocketRole socketRoleUser)
        {

            await ReplyAsync($"Удаляю сообщения от {socketRoleUser.Name}");

            var messages = Context.Channel.GetMessagesAsync(100).Flatten();

            int count = 0;

            await foreach (var message in messages)
            {

                if((message.Author as SocketGuildUser).Roles.ToList().Where(role => role.Id == socketRoleUser.Id).Count() > 0)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                    count++;
                }
            }

            await ReplyAsync($"Я удалила {count} сообщений");
        }
        [Command("c")]
        [Alias("clean")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CleanMessage()
        {
            await ReplyAsync($"Удаляю последние 100 сообщений, это может занять время..");

            var messages = Context.Channel.GetMessagesAsync(100).Flatten();

            int count = 0;

            await foreach (var message in messages)
            {
                    await Context.Channel.DeleteMessageAsync(message);
            }
            await ReplyAsync($"Готово, удалено 100 сообщений");
        }

        [Command("m")]
        [Alias("manga", "man")]
        public async Task SendManga(params string[] args)
        {
            var urlToSend = "Что-то пошло не так...";
            var option = string.Join("+", args);
            Random random = new Random();
            if(option=="")
                urlToSend = "https://nhentai.net/g/" + random.Next(0,382000);
            else
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(USERAGENT);
                    var response = await client.GetAsync("https://nhentai.net/search/?q=" + option);
                    if (response.IsSuccessStatusCode)
                    {

                        var htmlBody = await response.Content.ReadAsStringAsync();
                        var htmlDocument = new HtmlDocument();

                        htmlDocument.LoadHtml(htmlBody);
                        int articleId = random.Next(0, 25);
                        while (true)
                        {
                            try
                            {   
                                urlToSend = "https://nhentai.net/" + htmlDocument.DocumentNode.SelectSingleNode($"//*[@id='content']/div[2]/div[{random.Next(0,25)}]/a").Attributes["href"].Value;
                                break;
                            }
                            catch (Exception ex)
                            {
                                if (articleId == 1)
                                {
                                    LogErrorAsync(ex);
                                    urlToSend = "Что-то пошло не так...";
                                    break;
                                }
                                articleId = 1;
                            }
                        }
                    }
                }

            }

            await ReplyAsync(urlToSend);
        }

        private async Task LogErrorAsync(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ErrorLog.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                await writer.WriteLineAsync(message);
                writer.Close();
            }
        }

       

        [Command("v")]
        [Alias("video", "vid")]
        public async Task SendVideo(params string[] args)
        {
            var urlToSend = "Что-то пошло не так...";
            var option = string.Join("+", args);
            string url = $"https://animeidhentai.com/search/{option}";
            Random random = new Random();

            int id = random.Next(0, 10);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(USERAGENT);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var htmlBody = await response.Content.ReadAsStringAsync();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlBody);
                    int articleId = random.Next(0, 25);
                    while (true) {
                        try
                        {
                            ///html/body/main/div[1]/div[2]/div[1]/div[2]/div/iframe
                            urlToSend = htmlDocument.DocumentNode.SelectSingleNode($"/html/body/main/section/div[3]/article[{articleId}]/a").Attributes["href"].Value;


                            urlToSend += "/";
                            //var responseFromVideo = await client.GetAsync(urlToVideo);
                            //var htmlVideoBody = await responseFromVideo.Content.ReadAsStringAsync();    
                            //var htmlVideoDocument = new HtmlDocument();

                            //htmlVideoDocument.LoadHtml(htmlVideoBody);
                            //urlToSend = htmlVideoDocument.DocumentNode.SelectSingleNode($"html/body/main/div[1]/div[2]/div[1]/div[2]/div/iframe").Attributes["data-wpfc-original-src"].Value;



                            break;
                        }
                        catch (Exception ex)
                        {
                            if (articleId == 1)
                            {
                                LogErrorAsync(ex);
                                break;
                            }
                            articleId = 1;
                        }
                    }
                }
            }


            await ReplyAsync(urlToSend);
        }

        [Command("p")]
        [Alias("pic","picture")]
        public async Task SendPic()
        {
            Random random = new Random();
            string url = @"https://rule34.xxx/index.php?page=post&s=view&id=";
            var urlToSend = "";
            int id = random.Next(0, 5352307);
            using (var client = new HttpClient())
            {
                

                client.DefaultRequestHeaders.UserAgent.ParseAdd(USERAGENT);

                var response = await client.GetAsync(url + id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    var htmlBody = await response.Content.ReadAsStringAsync();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlBody);
                    try
                    {
                        urlToSend = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='image']").Attributes["src"].Value;
                    }
                    catch (Exception ex)
                    {
                        LogErrorAsync(ex);
                        urlToSend = "Что-то пошло не так...";
                    }
                }
            }


            await ReplyAsync(urlToSend);
        }
    }
}
