using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Helpers.Enumerations;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;
using vesact.common.message.v2;
using vesact.common.message.v2.Enumerations;
using vesact.common.message.v2.Interfaces;
using vesact.common.message.v2.Models;

namespace Infoscreens.Management.Functions.Timer_trigger
{
    public class CheckContentAvailable
    {
        #region Constructor / Dependency Injection

        private readonly ILogger<CheckContentAvailable> _logger;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly MessageService<IEmailProvider, FirebaseConfig_V1> _messageService;
        private readonly ILabelTranslationHelper _labelTranslationHelper;


        public CheckContentAvailable(
            ILogger<CheckContentAvailable> logger,
            IDatabaseRepository databaseRepository,
            MessageService<IEmailProvider, FirebaseConfig_V1> messageService,
            ILabelTranslationHelper labelTranslationHelper
        )
        {
            _logger = logger;
            _databaseRepository = databaseRepository;
            _messageService = messageService;
            _labelTranslationHelper = labelTranslationHelper;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "CheckContentAvailable";
        [Function(FUNCTION_NAME)]
        public async Task RunAsync([TimerTrigger("0 0 9 * * 1-5", RunOnStartup = false)] TimerInfo timer)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Timer Function {FUNCTION_NAME}() called."));

                // Get list of infoscreens
                var infoscreens = await _databaseRepository.GetInfoscreensAsync();

                // Filter out without flag
                infoscreens = infoscreens.Where(i => i.SendMailNoContent);

                // SW-Solution cc
                var swMail= new EmailReceiver(eEmailReceiverType.BCC, "sw-solutions@actemium.ch");

                // Container for all mails to send
                var mailsToSend = new Dictionary<EmailReceiver, Dictionary<Infoscreen, List<eSlide>>>();

                // Check infoscreens
                foreach (var infoscreen in infoscreens)
                {
                    try {
                        // Get mail to notify (either infoscreen or tenant level if first not defined)
                        var to = infoscreen.ContentAdminEmail ?? infoscreen.InfoscreenGroup.Tenant.ContentAdminEmail;

                        if (string.IsNullOrWhiteSpace(to))
                            continue;

                        var receiver = new EmailReceiver(eEmailReceiverType.TO, to);

                        var config = await BlobRepository.GetNodeConfigurationAsync(infoscreen.NodeId);
                        var dispalyedSlides = config.FrontendConfig.Slides.Order.Distinct();
                        var allSlides = Enum.GetValues(typeof(eSlide)).Cast<eSlide>();
                        var slidesToCheck = allSlides.Intersect(dispalyedSlides);

                        var slides = new List<eSlide>();

                        
                        foreach (eSlide slide in slidesToCheck)
                        {
                            switch (slide)
                            {
                                case eSlide.NewsInternal:
                                    var news = await _databaseRepository.GetPublishedNewsForInfoscreenAsync(infoscreen.Id, amount: 3, mustBeAssignedToInfoscreens: true);
                                    news = news.Where(n => n.IsForInfoscreens).ToList();
                                    if (news.Count <= 0)
                                        slides.Add(slide);
                                    break;
                                case eSlide.Youtube:
                                    var videos = await _databaseRepository.GetPublishedVideosForInfoscreenAsync(infoscreen.Id, amount: 3, mustBeAssignedToInfoscreens: true);
                                    videos = videos.Where(n => n.IsForInfoscreens).ToList();
                                    if (videos.Count <= 0)
                                        slides.Add(slide);
                                    break;
                                default: break;
                            }
                        }

                        if (slides.Count <= 0)
                            continue;

                        // Ensuring we only register a receiver once
                        if (!mailsToSend.ContainsKey(receiver))
                        {
                            mailsToSend.Add(receiver, new Dictionary<Infoscreen, List<eSlide>>());
                        }

                        // Ensuring we only send a single infoscreen line and its slides once
                        if(mailsToSend[receiver].ContainsKey(infoscreen))
                        {
                            mailsToSend[receiver][infoscreen] = mailsToSend[receiver][infoscreen].Union(slides).ToList();
                        }
                        else
                        {
                            mailsToSend[receiver].Add(infoscreen, slides);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogItem(300, exception, $"{FUNCTION_NAME} has thrown an exception while listing infoscreen missing content to notify : {0}", exception.Message));
                    }
                }

                // Send mails
                foreach(var mailContent in mailsToSend)
                {
                    try
                    {
                        // Get language to use
                        var user = await _databaseRepository.GetUserByEmailAsync(mailContent.Key.Email, isUsernameMatchEnough: true);
                        Language language = null;
                        if (user != null)
                        {
                            language = await _databaseRepository.GetLanguageAsync(user.Iso2);
                        }

                        language ??= mailContent.Value
                                                .Select(i => i.Key.DefaultContentLanguage)
                                                .GroupBy(l => l)
                                                .OrderByDescending(grp => grp.Count())
                                                .Select(grp => grp.Key)
                                                .FirstOrDefault();

                        language ??= await _databaseRepository.GetLanguageAsync("en");


                        // Prepare mail content
                        string subject = await _labelTranslationHelper.GetTextCodeLabelAsync("mail.noContent.subject", language.Iso2);
                        string bodyHeader = await _labelTranslationHelper.GetTextCodeLabelAsync("mail.noContent.header", language.Iso2);
                        string bodyInfoscreens = "<ul>";
                        foreach(var infoscreenContent in mailContent.Value)
                        {
                            bodyInfoscreens += $"<li><b>{infoscreenContent.Key.DisplayName}</b> ({infoscreenContent.Key.DefaultContentLanguage.Iso2.ToUpperInvariant()})</li>";
                            string bodySlide = "<ul>";
                            foreach(var slide in infoscreenContent.Value)
                            {
                                bodySlide += $"<li>{await _labelTranslationHelper.GetTextCodeLabelAsync($"slide.{EnumMemberParamHelper.GetEnumMemberAttrValue(slide)}", language.Iso2)}</li>";
                            }
                            bodySlide += "</ul>";
                            bodyInfoscreens += bodySlide;
                        }
                        bodyInfoscreens += "</ul>";


                        string body = $"<p>{bodyHeader}</p>" +
                                      $"{bodyInfoscreens}";


                        // Send mail
                        await _messageService.SendEmailAsync(CommonConfigHelper.SenderMail, new List<EmailReceiver>() { mailContent.Key, swMail }, subject, body, true);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogItem(300, exception, $"{FUNCTION_NAME} has thrown an exception while preparing and sending a mail : {0}", exception.Message));
                    }
                }

                _logger.LogDebug(new LogItem(11, $"Timer Function {FUNCTION_NAME}() finished, send {mailsToSend.Count} mails."));
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
            }
        }
    }
}
