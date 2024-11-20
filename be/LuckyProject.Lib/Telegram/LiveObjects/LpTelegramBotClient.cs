using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Passport;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Logging;

namespace LuckyProject.Lib.Telegram.LiveObjects
{
    public class LpTelegramBotClient : ILpTelegramBotClient
    {
        #region Internals & ctor & Dispose
        private readonly TelegramBotClient backend;
        private readonly CancellationTokenSource globalCts = new();
        private readonly string lpName;
        private readonly ILogger logger;

        public LpTelegramBotClient(
            string token,
            HttpClient httpClient = default,
            string lpName = null,
            ILogger logger = null)
        {
            backend = new TelegramBotClient(token, httpClient, globalCts.Token);
            this.lpName = lpName;
            this.logger = logger;
            backend.OnError += Backend_OnError;

        }

        private Task Backend_OnError(Exception exception, HandleErrorSource source)
        {
            logger?.LogWarning(exception, "Error in LpTelegramBotClient:{lpName}", lpName);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            backend.OnError -= Backend_OnError;
            globalCts.Cancel();
            globalCts.Dispose();
        }
        #endregion

        #region Interface impl
        #region TelegramBotClient
        public long BotId => backend.BotId;
        public bool LocalBotServer => backend.LocalBotServer;
        public TimeSpan Timeout
        {
            get => backend.Timeout;
            set => backend.Timeout = value;
        }

#pragma warning disable CS1591
        public event TelegramBotClient.OnUpdateHandler OnUpdate
        {
            add => backend.OnUpdate += value;
            remove => backend.OnUpdate -= value;
        }
        public event TelegramBotClient.OnMessageHandler OnMessage
        {
            add => backend.OnMessage += value;
            remove => backend.OnMessage -= value;
        }
#pragma warning restore CS1591

        public CancellationToken GlobalCancelToken => backend.GlobalCancelToken;

        public Task<TResponse> MakeRequestAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default) =>
            backend.MakeRequestAsync(request, cancellationToken);

        public Task<bool> TestApiAsync(CancellationToken cancellationToken = default) =>
            backend.TestApiAsync(cancellationToken);

        public Task DownloadFileAsync(
            string filePath,
            System.IO.Stream destination,
            CancellationToken cancellationToken = default) =>
            backend.DownloadFileAsync(filePath, destination, cancellationToken);
        #endregion

        #region Extensions
        #region Getting updates
        public Task<Update[]> GetUpdatesAsync(
            int? offset = default,
            int? limit = default,
            int? timeout = default,
            IEnumerable<UpdateType> allowedUpdates = default,
            CancellationToken cancellationToken = default) =>
            backend.GetUpdatesAsync(offset, limit, timeout, allowedUpdates, cancellationToken);

        public Task SetWebhookAsync(
            string url,
            InputFileStream certificate = default,
            string ipAddress = default,
            int? maxConnections = default,
            IEnumerable<UpdateType> allowedUpdates = default,
            bool dropPendingUpdates = default,
            string secretToken = default,
            CancellationToken cancellationToken = default) =>
            backend.SetWebhookAsync(
                url,
                certificate,
                ipAddress,
                maxConnections,
                allowedUpdates,
                dropPendingUpdates,
                secretToken,
                cancellationToken);

        public Task DeleteWebhookAsync(
            bool dropPendingUpdates = default,
            CancellationToken cancellationToken = default) =>
            backend.DeleteWebhookAsync(dropPendingUpdates, cancellationToken);

        public Task<WebhookInfo> GetWebhookInfoAsync(
            CancellationToken cancellationToken = default) =>
            backend.GetWebhookInfoAsync(cancellationToken);
        #endregion Getting updates

        #region Available methods
        public Task<User> GetMeAsync(CancellationToken cancellationToken = default) =>
            backend.GetMeAsync(cancellationToken);

        public Task LogOutAsync(CancellationToken cancellationToken = default) =>
            backend.LogOutAsync(cancellationToken);

        public Task CloseAsync(CancellationToken cancellationToken = default) =>
            backend.CloseAsync(cancellationToken);

        public Task <Message> SendTextMessageAsync(
            ChatId chatId,
            string text,
            int? messageThreadId = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> entities = default,
            LinkPreviewOptions linkPreviewOptions = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendTextMessageAsync(
                chatId,
                text,
                messageThreadId,
                parseMode,
                entities,
                linkPreviewOptions,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> ForwardMessageAsync(
            ChatId chatId,
            ChatId fromChatId,
            int messageId,
            int? messageThreadId = default,
            bool disableNotification = default,
            bool protectContent = default,
            CancellationToken cancellationToken = default) =>
            backend.ForwardMessageAsync(
                chatId,
                fromChatId,
                messageId,
                messageThreadId,
                disableNotification,
                protectContent,
                cancellationToken);

        public Task<MessageId[]> ForwardMessagesAsync(
            ChatId chatId,
            ChatId fromChatId,
            IEnumerable<int> messageIds,
            int? messageThreadId = default,
            bool disableNotification = default,
            bool protectContent = default,
            CancellationToken cancellationToken = default) =>
            backend.ForwardMessagesAsync(
                chatId,
                fromChatId,
                messageIds,
                messageThreadId,
                disableNotification,
                protectContent,
                cancellationToken);

        public Task<MessageId> CopyMessageAsync(
            ChatId chatId,
            ChatId fromChatId,
            int messageId,
            int? messageThreadId = default,
            string caption = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            bool showCaptionAboveMedia = default,
            bool disableNotification = default,
            bool protectContent = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default) =>
            backend.CopyMessageAsync(
                chatId,
                fromChatId,
                messageId,
                messageThreadId,
                caption,
                parseMode,
                captionEntities,
                showCaptionAboveMedia,
                disableNotification,
                protectContent,
                replyParameters,
                replyMarkup,
                cancellationToken);

        public Task<MessageId[]> CopyMessagesAsync(
            ChatId chatId,
            ChatId fromChatId,
            IEnumerable<int> messageIds,
            int? messageThreadId = default,
            bool disableNotification = default,
            bool protectContent = default,
            bool removeCaption = default,
            CancellationToken cancellationToken = default) =>
            backend.CopyMessagesAsync(
                chatId,
                fromChatId,
                messageIds,
                messageThreadId,
                disableNotification,
                protectContent,
                removeCaption,
                cancellationToken);

        public Task<Message> SendPhotoAsync(
            ChatId chatId,
            InputFile photo,
            int? messageThreadId = default,
            string caption = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            bool showCaptionAboveMedia = default,
            bool hasSpoiler = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendPhotoAsync(
                chatId,
                photo,
                messageThreadId,
                caption,
                parseMode,
                captionEntities,
                showCaptionAboveMedia,
                hasSpoiler,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendAudioAsync(
            ChatId chatId,
            InputFile audio,
            int? messageThreadId = default,
            string caption = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            int? duration = default,
            string performer = default,
            string title = default,
            InputFile thumbnail = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendAudioAsync(
                chatId,
                audio,
                messageThreadId,
                caption,
                parseMode,
                captionEntities,
                duration,
                performer,
                title,
                thumbnail,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendDocumentAsync(
            ChatId chatId,
            InputFile document,
            int? messageThreadId = default,
            InputFile thumbnail = default,
            string caption = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            bool disableContentTypeDetection = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendDocumentAsync(
                chatId,
                document,
                messageThreadId,
                thumbnail,
                caption,
                parseMode,
                captionEntities,
                disableContentTypeDetection,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendVideoAsync(
            ChatId chatId,
            InputFile video,
            int? messageThreadId = default,
            int? duration = default,
            int? width = default,
            int? height = default,
            InputFile thumbnail = default,
            string caption = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            bool showCaptionAboveMedia = default,
            bool hasSpoiler = default,
            bool supportsStreaming = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendVideoAsync(
                chatId,
                video,
                messageThreadId,
                duration,
                width,
                height,
                thumbnail,
                caption,
                parseMode,
                captionEntities,
                showCaptionAboveMedia,
                hasSpoiler,
                supportsStreaming,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendAnimationAsync(
            ChatId chatId,
            InputFile animation,
            int? messageThreadId = default,
            int? duration = default,
            int? width = default,
            int? height = default,
            InputFile thumbnail = default,
            string caption = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            bool showCaptionAboveMedia = default,
            bool hasSpoiler = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendAnimationAsync(
                chatId,
                animation,
                messageThreadId,
                duration,
                width,
                height,
                thumbnail,
                caption,
                parseMode,
                captionEntities,
                showCaptionAboveMedia,
                hasSpoiler,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendVoiceAsync(
            ChatId chatId,
            InputFile voice,
            int? messageThreadId = default,
            string caption = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            int? duration = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendVoiceAsync(
                chatId,
                voice,
                messageThreadId,
                caption,
                parseMode,
                captionEntities,
                duration,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendVideoNoteAsync(
            ChatId chatId,
            InputFile videoNote,
            int? messageThreadId = default,
            int? duration = default,
            int? length = default,
            InputFile thumbnail = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendVideoNoteAsync(
                chatId,
                videoNote,
                messageThreadId,
                duration,
                length,
                thumbnail,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendPaidMedia(
            ChatId chatId,
            int starCount,
            IEnumerable<InputPaidMedia> media,
            string payload = default,
            string caption = default,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            bool showCaptionAboveMedia = default,
            bool disableNotification = default,
            bool protectContent = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendPaidMedia(
                chatId,
                starCount,
                media,
                payload,
                caption,
                parseMode,
                captionEntities,
                showCaptionAboveMedia,
                disableNotification,
                protectContent,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message[]> SendMediaGroupAsync(
            ChatId chatId,
            IEnumerable<IAlbumInputMedia> media,
            int? messageThreadId = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendMediaGroupAsync(
                chatId,
                media,
                messageThreadId,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendLocationAsync(
            ChatId chatId,
            double latitude,
            double longitude,
            int? messageThreadId = default,
            double? horizontalAccuracy = default,
            int? livePeriod = default,
            int? heading = default,
            int? proximityAlertRadius = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendLocationAsync(
                chatId,
                latitude,
                longitude,
                messageThreadId,
                horizontalAccuracy,
                livePeriod,
                heading,
                proximityAlertRadius,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendVenueAsync(
            ChatId chatId,
            double latitude,
            double longitude,
            string title,
            string address,
            int? messageThreadId = default,
            string foursquareId = default,
            string foursquareType = default,
            string googlePlaceId = default,
            string googlePlaceType = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendVenueAsync(
                chatId,
                latitude,
                longitude,
                title,
                address,
                messageThreadId,
                foursquareId,
                foursquareType,
                googlePlaceId,
                googlePlaceType,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendContactAsync(
            ChatId chatId,
            string phoneNumber,
            string firstName,
            int? messageThreadId = default,
            string lastName = default,
            string vcard = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendContactAsync(
                chatId,
                phoneNumber,
                firstName,
                messageThreadId,
                lastName,
                vcard,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendPollAsync(
            ChatId chatId,
            string question,
            IEnumerable<InputPollOption> options,
            int? messageThreadId = default,
            ParseMode questionParseMode = default,
            IEnumerable<MessageEntity> questionEntities = default,
            bool isAnonymous = true,
            PollType? type = default,
            bool allowsMultipleAnswers = default,
            int? correctOptionId = default,
            string explanation = default,
            ParseMode explanationParseMode = default,
            IEnumerable<MessageEntity> explanationEntities = default,
            int? openPeriod = default,
            DateTime? closeDate = default,
            bool isClosed = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendPollAsync(
                chatId,
                question,
                options,
                messageThreadId,
                questionParseMode,
                questionEntities,
                isAnonymous,
                type,
                allowsMultipleAnswers,
                correctOptionId,
                explanation,
                explanationParseMode,
                explanationEntities,
                openPeriod,
                closeDate,
                isClosed,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SendDiceAsync(
            ChatId chatId,
            int? messageThreadId = default,
            string emoji = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendDiceAsync(
                chatId,
                messageThreadId,
                emoji,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task SendChatActionAsync(
            ChatId chatId,
            ChatAction action,
            int? messageThreadId = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendChatActionAsync(
                chatId,
                action,
                messageThreadId,
                businessConnectionId,
                cancellationToken);

        public Task SetMessageReactionAsync(
            ChatId chatId,
            int messageId,
            IEnumerable<ReactionType> reaction,
            bool isBig = default,
            CancellationToken cancellationToken = default) =>
            backend.SetMessageReactionAsync(
                chatId,
                messageId,
                reaction,
                isBig,
                cancellationToken);

        public Task<UserProfilePhotos> GetUserProfilePhotosAsync(
            long userId,
            int? offset = default,
            int? limit = default,
            CancellationToken cancellationToken = default) =>
            backend.GetUserProfilePhotosAsync(userId, offset, limit, cancellationToken);

        public Task<File> GetFileAsync(
            string fileId,
            CancellationToken cancellationToken = default) =>
            backend.GetFileAsync(fileId, cancellationToken);

        public Task BanChatMemberAsync(
            ChatId chatId,
            long userId,
            DateTime? untilDate = default,
            bool revokeMessages = default,
            CancellationToken cancellationToken = default) =>
            backend.BanChatMemberAsync(
                chatId,
                userId,
                untilDate,
                revokeMessages,
                cancellationToken);

        public Task UnbanChatMemberAsync(
            ChatId chatId,
            long userId,
            bool onlyIfBanned = default,
            CancellationToken cancellationToken = default) =>
            backend.UnbanChatMemberAsync(chatId, userId, onlyIfBanned, cancellationToken);

        public Task RestrictChatMemberAsync(
            ChatId chatId,
            long userId,
            ChatPermissions permissions,
            bool useIndependentChatPermissions = default,
            DateTime? untilDate = default,
            CancellationToken cancellationToken = default) =>
            backend.RestrictChatMemberAsync(
                chatId,
                userId,
                permissions,
                useIndependentChatPermissions,
                untilDate,
                cancellationToken);

        public Task PromoteChatMemberAsync(
            ChatId chatId,
            long userId,
            bool isAnonymous = default,
            bool canManageChat = default,
            bool canPostMessages = default,
            bool canEditMessages = default,
            bool canDeleteMessages = default,
            bool canPostStories = default,
            bool canEditStories = default,
            bool canDeleteStories = default,
            bool canManageVideoChats = default,
            bool canRestrictMembers = default,
            bool canPromoteMembers = default,
            bool canChangeInfo = default,
            bool canInviteUsers = default,
            bool canPinMessages = default,
            bool canManageTopics = default,
            CancellationToken cancellationToken = default) =>
            backend.PromoteChatMemberAsync(
                chatId,
                userId,
                isAnonymous,
                canManageChat,
                canPostMessages,
                canEditMessages,
                canDeleteMessages,
                canPostStories,
                canEditStories,
                canDeleteStories,
                canManageVideoChats,
                canRestrictMembers,
                canPromoteMembers,
                canChangeInfo,
                canInviteUsers,
                canPinMessages,
                canManageTopics,
                cancellationToken);

        public Task SetChatAdministratorCustomTitleAsync(
            ChatId chatId,
            long userId,
            string customTitle,
            CancellationToken cancellationToken = default) =>
            backend.SetChatAdministratorCustomTitleAsync(
                chatId,
                userId,
                customTitle,
                cancellationToken);

        public Task BanChatSenderChatAsync(
            ChatId chatId,
            long senderChatId,
            CancellationToken cancellationToken = default) =>
            backend.BanChatSenderChatAsync(chatId, senderChatId, cancellationToken);

        public Task UnbanChatSenderChatAsync(
            ChatId chatId,
            long senderChatId,
            CancellationToken cancellationToken = default) =>
            backend.UnbanChatSenderChatAsync(chatId, senderChatId, cancellationToken);

        public Task SetChatPermissionsAsync(
            ChatId chatId,
            ChatPermissions permissions,
            bool useIndependentChatPermissions = default,
            CancellationToken cancellationToken = default) =>
            backend.SetChatPermissionsAsync(
                chatId,
                permissions,
                useIndependentChatPermissions,
                cancellationToken);

        public Task<string> ExportChatInviteLinkAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.ExportChatInviteLinkAsync(chatId, cancellationToken);

        public Task<ChatInviteLink> CreateChatInviteLinkAsync(
            ChatId chatId,
            string name = default,
            DateTime? expireDate = default,
            int? memberLimit = default,
            bool createsJoinRequest = default,
            CancellationToken cancellationToken = default) =>
            backend.CreateChatInviteLinkAsync(
                chatId,
                name,
                expireDate,
                memberLimit,
                createsJoinRequest,
                cancellationToken);

        public Task<ChatInviteLink> EditChatInviteLinkAsync(
            ChatId chatId,
            string inviteLink,
            string name = default,
            DateTime? expireDate = default,
            int? memberLimit = default,
            bool createsJoinRequest = default,
            CancellationToken cancellationToken = default) =>
            backend.EditChatInviteLinkAsync(
                chatId,
                inviteLink,
                name,
                expireDate,
                memberLimit,
                createsJoinRequest,
                cancellationToken);

        public Task<ChatInviteLink> CreateChatSubscriptionInviteLink(
            ChatId chatId,
            int subscriptionPeriod,
            int subscriptionPrice,
            string name = default,
            CancellationToken cancellationToken = default) =>
            backend.CreateChatSubscriptionInviteLink(
                chatId,
                subscriptionPeriod,
                subscriptionPrice,
                name,
                cancellationToken);

        public Task<ChatInviteLink> EditChatSubscriptionInviteLink(
            ChatId chatId,
            string inviteLink,
            string name = default,
            CancellationToken cancellationToken = default) =>
            backend.EditChatSubscriptionInviteLink(
                chatId,
                inviteLink,
                name,
                cancellationToken);

        public Task<ChatInviteLink> RevokeChatInviteLinkAsync(
            ChatId chatId,
            string inviteLink,
            CancellationToken cancellationToken = default) =>
            backend.RevokeChatInviteLinkAsync(chatId, inviteLink, cancellationToken);

        public Task ApproveChatJoinRequest(
            ChatId chatId,
            long userId,
            CancellationToken cancellationToken = default) =>
            backend.ApproveChatJoinRequest(chatId, userId, cancellationToken);

        public Task DeclineChatJoinRequest(
            ChatId chatId,
            long userId,
            CancellationToken cancellationToken = default) =>
            backend.DeclineChatJoinRequest(chatId, userId, cancellationToken);

        public Task SetChatPhotoAsync(
            ChatId chatId,
            InputFileStream photo,
            CancellationToken cancellationToken = default) =>
            backend.SetChatPhotoAsync(chatId, photo, cancellationToken);

        public Task DeleteChatPhotoAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.DeleteChatPhotoAsync(chatId, cancellationToken);

        public Task SetChatTitleAsync(
            ChatId chatId,
            string title,
            CancellationToken cancellationToken = default) =>
            backend.SetChatTitleAsync(chatId, title, cancellationToken);

        public Task SetChatDescriptionAsync(
            ChatId chatId,
            string description = default,
            CancellationToken cancellationToken = default) =>
            backend.SetChatDescriptionAsync(chatId, description, cancellationToken);

        public Task PinChatMessageAsync(
            ChatId chatId,
            int messageId,
            bool disableNotification = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.PinChatMessageAsync(
                chatId,
                messageId,
                disableNotification,
                businessConnectionId,
                cancellationToken);

        public Task UnpinChatMessageAsync(
            ChatId chatId,
            int? messageId = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.UnpinChatMessageAsync(
                chatId,
                messageId,
                businessConnectionId,
                cancellationToken);

        public Task UnpinAllChatMessages(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.UnpinAllChatMessages(chatId, cancellationToken);

        public Task LeaveChatAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.LeaveChatAsync(chatId, cancellationToken);

        public Task<ChatFullInfo> GetChatAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.GetChatAsync(chatId, cancellationToken);

        public Task<ChatMember[]> GetChatAdministratorsAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.GetChatAdministratorsAsync(chatId, cancellationToken);

        public Task<int> GetChatMemberCountAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.GetChatMemberCountAsync(chatId, cancellationToken);

        public Task<ChatMember> GetChatMemberAsync(
            ChatId chatId,
            long userId,
            CancellationToken cancellationToken = default) =>
            backend.GetChatMemberAsync(chatId, userId, cancellationToken);

        public Task SetChatStickerSetAsync(
            ChatId chatId,
            string stickerSetName,
            CancellationToken cancellationToken = default) =>
            backend.SetChatStickerSetAsync(chatId, stickerSetName, cancellationToken);

        public Task DeleteChatStickerSetAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.DeleteChatStickerSetAsync(chatId, cancellationToken);

        public Task<Sticker[]> GetForumTopicIconStickersAsync(
            CancellationToken cancellationToken = default) =>
            backend.GetForumTopicIconStickersAsync(cancellationToken);

        public Task<ForumTopic> CreateForumTopicAsync(
            ChatId chatId,
            string name,
            int? iconColor = default,
            string iconCustomEmojiId = default,
            CancellationToken cancellationToken = default) =>
            backend.CreateForumTopicAsync(
                chatId,
                name,
                iconColor,
                iconCustomEmojiId,
                cancellationToken);

        public Task EditForumTopicAsync(
            ChatId chatId,
            int messageThreadId,
            string name = default,
            string iconCustomEmojiId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditForumTopicAsync(
                chatId,
                messageThreadId,
                name,
                iconCustomEmojiId,
                cancellationToken);

        public Task CloseForumTopicAsync(
            ChatId chatId,
            int messageThreadId,
            CancellationToken cancellationToken = default) =>
            backend.CloseForumTopicAsync(chatId, messageThreadId, cancellationToken);

        public Task ReopenForumTopicAsync(
            ChatId chatId,
            int messageThreadId,
            CancellationToken cancellationToken = default) =>
            backend.ReopenForumTopicAsync(chatId, messageThreadId, cancellationToken);

        public Task DeleteForumTopicAsync(
            ChatId chatId,
            int messageThreadId,
            CancellationToken cancellationToken = default) =>
            backend.DeleteForumTopicAsync(chatId, messageThreadId, cancellationToken);

        public Task UnpinAllForumTopicMessagesAsync(
            ChatId chatId,
            int messageThreadId,
            CancellationToken cancellationToken = default) =>
            backend.UnpinAllForumTopicMessagesAsync(chatId, messageThreadId, cancellationToken);

        public Task EditGeneralForumTopicAsync(
            ChatId chatId,
            string name,
            CancellationToken cancellationToken = default) =>
            backend.EditGeneralForumTopicAsync(chatId, name, cancellationToken);

        public Task CloseGeneralForumTopicAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.CloseGeneralForumTopicAsync(chatId, cancellationToken);

        public Task ReopenGeneralForumTopicAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.ReopenGeneralForumTopicAsync(chatId, cancellationToken);

        public Task HideGeneralForumTopicAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.HideGeneralForumTopicAsync(chatId, cancellationToken);

        public Task UnhideGeneralForumTopicAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.UnhideGeneralForumTopicAsync(chatId, cancellationToken);

        public Task UnpinAllGeneralForumTopicMessagesAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default) =>
            backend.UnpinAllGeneralForumTopicMessagesAsync(chatId, cancellationToken);

        public Task AnswerCallbackQueryAsync(
            string callbackQueryId,
            string text = default,
            bool showAlert = default,
            string url = default,
            int? cacheTime = default,
            CancellationToken cancellationToken = default) =>
            backend.AnswerCallbackQueryAsync(
                callbackQueryId,
                text,
                showAlert,
                url,
                cacheTime,
                cancellationToken);

        public Task<UserChatBoosts> GetUserChatBoostsAsync(
            ChatId chatId,
            long userId,
            CancellationToken cancellationToken = default) =>
            backend.GetUserChatBoostsAsync(chatId, userId, cancellationToken);

        public Task<BusinessConnection> GetBusinessConnection(
            string businessConnectionId,
            CancellationToken cancellationToken = default) =>
            backend.GetBusinessConnection(businessConnectionId, cancellationToken);

        public Task SetMyCommandsAsync(
            IEnumerable<BotCommand> commands,
            BotCommandScope scope = default,
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.SetMyCommandsAsync(
                commands,
                scope,
                languageCode,
                cancellationToken);

        public Task DeleteMyCommandsAsync(
            BotCommandScope scope = default,
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.DeleteMyCommandsAsync(scope, languageCode, cancellationToken);

        public Task<BotCommand[]> GetMyCommandsAsync(
            BotCommandScope scope = default,
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.GetMyCommandsAsync(scope, languageCode, cancellationToken);

        public Task SetMyNameAsync(
            string name = default,
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.SetMyNameAsync(name, languageCode, cancellationToken);

        public Task<BotName> GetMyNameAsync(
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.GetMyNameAsync(languageCode, cancellationToken);

        public Task SetMyDescriptionAsync(
            string description = default,
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.SetMyDescriptionAsync(description, languageCode, cancellationToken);

        public Task<BotDescription> GetMyDescriptionAsync(
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.GetMyDescriptionAsync(languageCode, cancellationToken);

        public Task SetMyShortDescriptionAsync(
            string shortDescription = default,
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.SetMyShortDescriptionAsync(shortDescription, languageCode, cancellationToken);

        public Task<BotShortDescription> GetMyShortDescriptionAsync(
            string languageCode = default,
            CancellationToken cancellationToken = default) =>
            backend.GetMyShortDescriptionAsync(languageCode, cancellationToken);

        public Task SetChatMenuButtonAsync(
            long? chatId = default,
            MenuButton menuButton = default,
            CancellationToken cancellationToken = default) =>
            backend.SetChatMenuButtonAsync(chatId, menuButton, cancellationToken);

        public Task<MenuButton> GetChatMenuButtonAsync(
            long? chatId = default,
            CancellationToken cancellationToken = default) =>
            backend.GetChatMenuButtonAsync(chatId, cancellationToken);

        public Task SetMyDefaultAdministratorRightsAsync(
            ChatAdministratorRights rights = default,
            bool forChannels = default,
            CancellationToken cancellationToken = default) =>
            backend.SetMyDefaultAdministratorRightsAsync(rights, forChannels, cancellationToken);

        public Task<ChatAdministratorRights> GetMyDefaultAdministratorRightsAsync(
            bool forChannels = default,
            CancellationToken cancellationToken = default) =>
            backend.GetMyDefaultAdministratorRightsAsync(forChannels, cancellationToken);
        #endregion Available methods

        #region Updating messages
        public Task<Message> EditMessageTextAsync(
            ChatId chatId,
            int messageId,
            string text,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> entities = default,
            LinkPreviewOptions linkPreviewOptions = default,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageTextAsync(
                chatId,
                messageId,
                text,
                parseMode,
                entities,
                linkPreviewOptions,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task EditMessageTextAsync(
            string inlineMessageId,
            string text,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> entities = default,
            LinkPreviewOptions linkPreviewOptions = default,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageTextAsync(
                inlineMessageId,
                text,
                parseMode,
                entities,
                linkPreviewOptions,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> EditMessageCaptionAsync(
            ChatId chatId,
            int messageId,
            string caption,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            bool showCaptionAboveMedia = default,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageCaptionAsync(
                chatId,
                messageId,
                caption,
                parseMode,
                captionEntities,
                showCaptionAboveMedia,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task EditMessageCaptionAsync(
            string inlineMessageId,
            string caption,
            ParseMode parseMode = default,
            IEnumerable<MessageEntity> captionEntities = default,
            bool showCaptionAboveMedia = default,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageCaptionAsync(
                inlineMessageId,
                caption,
                parseMode,
                captionEntities,
                showCaptionAboveMedia,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> EditMessageMediaAsync(
            ChatId chatId,
            int messageId,
            InputMedia media,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageMediaAsync(
                chatId,
                messageId,
                media,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task EditMessageMediaAsync(
            string inlineMessageId,
            InputMedia media,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageMediaAsync(
                inlineMessageId,
                media,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> EditMessageLiveLocationAsync(
            ChatId chatId,
            int messageId,
            double latitude,
            double longitude,
            int? livePeriod = default,
            double? horizontalAccuracy = default,
            int? heading = default,
            int? proximityAlertRadius = default,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageLiveLocationAsync(
                chatId,
                messageId,
                latitude,
                longitude,
                livePeriod,
                horizontalAccuracy,
                heading,
                proximityAlertRadius,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task EditMessageLiveLocationAsync(
            string inlineMessageId,
            double latitude,
            double longitude,
            int? livePeriod = default,
            double? horizontalAccuracy = default,
            int? heading = default,
            int? proximityAlertRadius = default,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageLiveLocationAsync(
                inlineMessageId,
                latitude,
                longitude,
                livePeriod,
                horizontalAccuracy,
                heading,
                proximityAlertRadius,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> StopMessageLiveLocationAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.StopMessageLiveLocationAsync(
                chatId,
                messageId,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task StopMessageLiveLocationAsync(
            string inlineMessageId,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.StopMessageLiveLocationAsync(
                inlineMessageId,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> EditMessageReplyMarkupAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageReplyMarkupAsync(
                chatId,
                messageId,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task EditMessageReplyMarkupAsync(
            string inlineMessageId,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.EditMessageReplyMarkupAsync(
                inlineMessageId,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Poll> StopPollAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.StopPollAsync(
                chatId,
                messageId,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task DeleteMessageAsync(
            ChatId chatId,
            int messageId,
            CancellationToken cancellationToken = default) =>
            backend.DeleteMessageAsync(chatId, messageId, cancellationToken);

        public Task DeleteMessagesAsync(
            ChatId chatId,
            IEnumerable<int> messageIds,
            CancellationToken cancellationToken = default) =>
            backend.DeleteMessagesAsync(chatId, messageIds, cancellationToken);
        #endregion Updating messages

        #region Stickers
        public Task<Message> SendStickerAsync(
            ChatId chatId,
            InputFile sticker,
            int? messageThreadId = default,
            string emoji = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            IReplyMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendStickerAsync(
                chatId,
                sticker,
                messageThreadId,
                emoji,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<StickerSet> GetStickerSetAsync(
            string name,
            CancellationToken cancellationToken = default) =>
            backend.GetStickerSetAsync(name, cancellationToken);

        public Task<Sticker[]> GetCustomEmojiStickersAsync(
            IEnumerable<string> customEmojiIds,
            CancellationToken cancellationToken = default) =>
            backend.GetCustomEmojiStickersAsync(customEmojiIds, cancellationToken);

        public Task<File> UploadStickerFileAsync(
            long userId,
            InputFileStream sticker,
            StickerFormat stickerFormat,
            CancellationToken cancellationToken = default) =>
            backend.UploadStickerFileAsync(userId, sticker, stickerFormat, cancellationToken);

        public Task CreateNewStickerSetAsync(
            long userId,
            string name,
            string title,
            IEnumerable<InputSticker> stickers,
            StickerType? stickerType = default,
            bool needsRepainting = default,
            CancellationToken cancellationToken = default) =>
            backend.CreateNewStickerSetAsync(
                userId,
                name,
                title,
                stickers,
                stickerType,
                needsRepainting,
                cancellationToken);

        public Task AddStickerToSetAsync(
            long userId,
            string name,
            InputSticker sticker,
            CancellationToken cancellationToken = default) =>
            backend.AddStickerToSetAsync(userId, name, sticker, cancellationToken);

        public Task SetStickerPositionInSetAsync(
            InputFileId sticker,
            int position,
            CancellationToken cancellationToken = default) =>
            backend.SetStickerPositionInSetAsync(sticker, position, cancellationToken);

        public Task DeleteStickerFromSetAsync(
            InputFileId sticker,
            CancellationToken cancellationToken = default) =>
            backend.DeleteStickerFromSetAsync(sticker, cancellationToken);

        public Task ReplaceStickerInSet(
            long userId,
            string name,
            string oldSticker,
            InputSticker sticker,
            CancellationToken cancellationToken = default) =>
            backend.ReplaceStickerInSet(userId, name, oldSticker, sticker, cancellationToken);

        public Task SetStickerEmojiListAsync(
            InputFileId sticker,
            IEnumerable<string> emojiList,
            CancellationToken cancellationToken = default) =>
            backend.SetStickerEmojiListAsync(sticker, emojiList, cancellationToken);

        public Task SetStickerKeywordsAsync(
            InputFileId sticker,
            IEnumerable<string> keywords = default,
            CancellationToken cancellationToken = default) =>
            backend.SetStickerKeywordsAsync(sticker, keywords, cancellationToken);

        public Task SetStickerMaskPositionAsync(
            InputFileId sticker,
            MaskPosition maskPosition = default,
            CancellationToken cancellationToken = default) =>
            backend.SetStickerMaskPositionAsync(sticker, maskPosition, cancellationToken);

        public Task SetStickerSetTitleAsync(
            string name,
            string title,
            CancellationToken cancellationToken = default) =>
            backend.SetStickerSetTitleAsync(name, title, cancellationToken);

        public Task SetStickerSetThumbnailAsync(
            string name,
            long userId,
            StickerFormat format,
            InputFile thumbnail = default,
            CancellationToken cancellationToken = default) =>
            backend.SetStickerSetThumbnailAsync(name, userId, format, thumbnail, cancellationToken);

        public Task SetCustomEmojiStickerSetThumbnailAsync(
            string name,
            string customEmojiId = default,
            CancellationToken cancellationToken = default) =>
            backend.SetCustomEmojiStickerSetThumbnailAsync(name, customEmojiId, cancellationToken);

        public Task DeleteStickerSetAsync(
            string name,
            CancellationToken cancellationToken = default) =>
            backend.DeleteStickerSetAsync(name, cancellationToken);
        #endregion

        #region Inline mode
        public Task AnswerInlineQueryAsync(
            string inlineQueryId,
            IEnumerable<InlineQueryResult> results,
            int? cacheTime = default,
            bool isPersonal = default,
            string nextOffset = default,
            InlineQueryResultsButton button = default,
            CancellationToken cancellationToken = default) =>
            backend.AnswerInlineQueryAsync(
                inlineQueryId,
                results,
                cacheTime,
                isPersonal,
                nextOffset,
                button,
                cancellationToken);

        public Task<SentWebAppMessage> AnswerWebAppQueryAsync(
            string webAppQueryId,
            InlineQueryResult result,
            CancellationToken cancellationToken = default) =>
            backend.AnswerWebAppQueryAsync(webAppQueryId, result, cancellationToken);
        #endregion Inline mode

        #region Payments
        public Task<Message> SendInvoiceAsync(
            ChatId chatId,
            string title,
            string description,
            string payload,
            string providerToken,
            string currency,
            IEnumerable<LabeledPrice> prices,
            int? messageThreadId = default,
            int? maxTipAmount = default,
            IEnumerable<int> suggestedTipAmounts = default,
            string startParameter = default,
            string providerData = default,
            string photoUrl = default,
            int? photoSize = default,
            int? photoWidth = default,
            int? photoHeight = default,
            bool needName = default,
            bool needPhoneNumber = default,
            bool needEmail = default,
            bool needShippingAddress = default,
            bool sendPhoneNumberToProvider = default,
            bool sendEmailToProvider = default,
            bool isFlexible = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default) =>
            backend.SendInvoiceAsync(
                chatId,
                title,
                description,
                payload,
                providerToken,
                currency,
                prices,
                messageThreadId,
                maxTipAmount,
                suggestedTipAmounts,
                startParameter,
                providerData,
                photoUrl,
                photoSize,
                photoWidth,
                photoHeight,
                needName,
                needPhoneNumber,
                needEmail,
                needShippingAddress,
                sendPhoneNumberToProvider,
                sendEmailToProvider,
                isFlexible,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                cancellationToken);

        public Task<string> CreateInvoiceLinkAsync(
            string title,
            string description,
            string payload,
            string providerToken,
            string currency,
            IEnumerable<LabeledPrice> prices,
            int? maxTipAmount = default,
            IEnumerable<int> suggestedTipAmounts = default,
            string providerData = default,
            string photoUrl = default,
            int? photoSize = default,
            int? photoWidth = default,
            int? photoHeight = default,
            bool needName = default,
            bool needPhoneNumber = default,
            bool needEmail = default,
            bool needShippingAddress = default,
            bool sendPhoneNumberToProvider = default,
            bool sendEmailToProvider = default,
            bool isFlexible = default,
            CancellationToken cancellationToken = default) =>
            backend.CreateInvoiceLinkAsync(
                title,
                description,
                payload,
                providerToken,
                currency,
                prices,
                maxTipAmount,
                suggestedTipAmounts,
                providerData,
                photoUrl,
                photoSize,
                photoWidth,
                photoHeight,
                needName,
                needPhoneNumber,
                needEmail,
                needShippingAddress,
                sendPhoneNumberToProvider,
                sendEmailToProvider,
                isFlexible,
                cancellationToken);

        public Task AnswerShippingQueryAsync(
            string shippingQueryId,
            IEnumerable<ShippingOption> shippingOptions,
            CancellationToken cancellationToken = default) =>
            backend.AnswerShippingQueryAsync(shippingQueryId, shippingOptions, cancellationToken);

        public Task AnswerShippingQueryAsync(
            string shippingQueryId,
            string errorMessage,
            CancellationToken cancellationToken = default) =>
            backend.AnswerShippingQueryAsync(shippingQueryId, errorMessage, cancellationToken);

        public Task AnswerPreCheckoutQueryAsync(
            string preCheckoutQueryId,
            string errorMessage = default,
            CancellationToken cancellationToken = default) =>
            backend.AnswerPreCheckoutQueryAsync(
                preCheckoutQueryId,
                errorMessage,
                cancellationToken);

        public Task<StarTransactions> GetStarTransactions(
            int? offset = default,
            int? limit = default,
            CancellationToken cancellationToken = default) =>
            backend.GetStarTransactions(offset, limit, cancellationToken);

        public Task RefundStarPayment(
            long userId,
            string telegramPaymentChargeId,
            CancellationToken cancellationToken = default) =>
            backend.RefundStarPayment(userId, telegramPaymentChargeId, cancellationToken);

        public Task SetPassportDataErrors(
            long userId,
            IEnumerable<PassportElementError> errors,
            CancellationToken cancellationToken = default) =>
            backend.SetPassportDataErrors(userId, errors, cancellationToken);
        #endregion Payments

        #region Games
        public Task<Message> SendGameAsync(
            long chatId,
            string gameShortName,
            int? messageThreadId = default,
            bool disableNotification = default,
            bool protectContent = default,
            string messageEffectId = default,
            ReplyParameters replyParameters = default,
            InlineKeyboardMarkup replyMarkup = default,
            string businessConnectionId = default,
            CancellationToken cancellationToken = default) =>
            backend.SendGameAsync(
                chatId,
                gameShortName,
                messageThreadId,
                disableNotification,
                protectContent,
                messageEffectId,
                replyParameters,
                replyMarkup,
                businessConnectionId,
                cancellationToken);

        public Task<Message> SetGameScoreAsync(
            long userId,
            int score,
            long chatId,
            int messageId,
            bool force = default,
            bool disableEditMessage = default,
            CancellationToken cancellationToken = default) =>
            backend.SetGameScoreAsync(
                userId,
                score,
                chatId,
                messageId,
                force,
                disableEditMessage,
                cancellationToken);

        public Task SetGameScoreAsync(
            long userId,
            int score,
            string inlineMessageId,
            bool force = default,
            bool disableEditMessage = default,
            CancellationToken cancellationToken = default) =>
            backend.SetGameScoreAsync(
                userId,
                score,
                inlineMessageId,
                force,
                disableEditMessage,
                cancellationToken);

        public Task<GameHighScore[]> GetGameHighScoresAsync(
            long userId,
            long chatId,
            int messageId,
            CancellationToken cancellationToken = default) =>
            backend.GetGameHighScoresAsync(userId, chatId, messageId, cancellationToken);

        public Task<GameHighScore[]> GetGameHighScoresAsync(
            long userId,
            string inlineMessageId,
            CancellationToken cancellationToken = default) =>
            backend.GetGameHighScoresAsync(userId, inlineMessageId, cancellationToken);
        #endregion Games
        #endregion Extensions
        #endregion
    }
}
