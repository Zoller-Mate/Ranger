using System.Text.Json.Serialization;


namespace Ranger.Dtos
{
    // Users tábla
    internal class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [JsonPropertyName("profile_pic")]
        public string ProfilePic { get; set; }

        [JsonPropertyName("date_of_birth")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("emergency_contact")]
        public string EmergencyContact { get; set; }

        [JsonPropertyName("password_reset_at")]
        public string PasswordResetAt { get; set; }
    }

    // User online status tábla
    internal class UserOnlineStatusDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("is_online")]
        public bool IsOnline { get; set; }

        [JsonPropertyName("last_seen_at")]
        public string LastSeenAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }

    // Camps tábla
    internal class CampDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("min_group_size")]
        public int? MinGroupSize { get; set; }

        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }

        [JsonPropertyName("staff_chat_id")]
        public string StaffChatId { get; set; }

        [JsonPropertyName("join_code")]
        public string JoinCode { get; set; }
    }

    // Member to Camp tábla
    internal class MemberToCampDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("camp_id")]
        public string CampId { get; set; }

        [JsonPropertyName("room_id")]
        public string RoomId { get; set; }

        [JsonPropertyName("group_id")]
        public string GroupId { get; set; }

        public string Role { get; set; }
    }

    // Chats tábla
    internal class ChatDto
    {
        public string Id { get; set; }

        [JsonPropertyName("last_message_at")]
        public string LastMessageAt { get; set; }
    }

    // Messages tábla
    internal class MessageDto
    {
        public string Id { get; set; }

        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        public object Body { get; set; }

        [JsonPropertyName("reply_to_message_id")]
        public string ReplyToMessageId { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("edited_at")]
        public string EditedAt { get; set; }

        [JsonPropertyName("deleted_at")]
        public string DeletedAt { get; set; }
    }

    // Chat Members tábla
    internal class ChatMemberDto
    {
        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("last_viewed")]
        public string LastViewed { get; set; }

        [JsonPropertyName("joined_at")]
        public string JoinedAt { get; set; }

        [JsonPropertyName("archived_at")]
        public string ArchivedAt { get; set; }
    }

    // Groups tábla
    internal class GroupDto
    {
        public string Id { get; set; }

        [JsonPropertyName("camp_id")]
        public string CampId { get; set; }

        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }

        public string Name { get; set; }
        public string Color { get; set; }

        [JsonPropertyName("join_code")]
        public string JoinCode { get; set; }
    }

    // Rooms tábla
    internal class RoomDto
    {
        public string Id { get; set; }

        [JsonPropertyName("camp_id")]
        public string CampId { get; set; }

        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("join_code")]
        public string JoinCode { get; set; }

        public string Color { get; set; }
    }

    // Password Resets tábla
    internal class PasswordResetDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        public string Token { get; set; }

        [JsonPropertyName("expires_at")]
        public string ExpiresAt { get; set; }
    }

    // Tokens tábla
    internal class TokenDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        public string Token { get; set; }

        [JsonPropertyName("client_device_type")]
        public string ClientDeviceType { get; set; }
    }

    // Locations tábla
    internal class LocationDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("camp_id")]
        public string CampId { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [JsonPropertyName("last_updated")]
        public string LastUpdated { get; set; }
    }

    // Payments tábla
    internal class PaymentDto
    {
        public string Id { get; set; }

        [JsonPropertyName("camp_id")]
        public string CampId { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("due_date")]
        public string DueDate { get; set; }

        public int? Amount { get; set; }
        public string Currency { get; set; }
    }

    // User Payments tábla
    internal class UserPaymentDto
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }

        [JsonPropertyName("is_paid")]
        public bool IsPaid { get; set; }
    }

    // Teljes adatbázis wrapper (ha az API egy objectben adja vissza az összeset)
    internal class DatabaseDto
    {
        public List<UserDto> Users { get; set; }

        [JsonPropertyName("user_online_status")]
        public List<UserOnlineStatusDto> UserOnlineStatus { get; set; }

        public List<CampDto> Camps { get; set; }

        [JsonPropertyName("member_to_camp")]
        public List<MemberToCampDto> MemberToCamp { get; set; }

        public List<ChatDto> Chats { get; set; }
        public List<MessageDto> Messages { get; set; }

        [JsonPropertyName("chat_members")]
        public List<ChatMemberDto> ChatMembers { get; set; }

        public List<GroupDto> Groups { get; set; }
        public List<RoomDto> Rooms { get; set; }

        [JsonPropertyName("password_resets")]
        public List<PasswordResetDto> PasswordResets { get; set; }

        public List<TokenDto> Tokens { get; set; }
        public List<LocationDto> Locations { get; set; }
        public List<PaymentDto> Payments { get; set; }

        [JsonPropertyName("user_payments")]
        public List<UserPaymentDto> UserPayments { get; set; }
    }
}
