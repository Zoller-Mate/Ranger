using System.Collections.Generic;

namespace Ranger.Dtos
{
    // Users tábla
    internal class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfilePic { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string EmergencyContact { get; set; }
        public string PasswordResetAt { get; set; }
    }

    // User online status tábla
    internal class UserOnlineStatusDto
    {
        public string UserId { get; set; }
        public bool IsOnline { get; set; }
        public string LastSeenAt { get; set; }
        public string UpdatedAt { get; set; }
    }

    // Camps tábla
    internal class CampDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? MinGroupSize { get; set; }
        public string ChatId { get; set; }
        public string StaffChatId { get; set; }
        public string JoinCode { get; set; }
    }

    // Member to Camp tábla
    internal class MemberToCampDto
    {
        public string UserId { get; set; }
        public string CampId { get; set; }
        public string RoomId { get; set; }
        public string GroupId { get; set; }
        public string Role { get; set; }
    }

    // Chats tábla
    internal class ChatDto
    {
        public string Id { get; set; }
        public string LastMessageAt { get; set; }
    }

    // Messages tábla
    internal class MessageDto
    {
        public string Id { get; set; }
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public object Body { get; set; }
        public string ReplyToMessageId { get; set; }
        public string CreatedAt { get; set; }
        public string EditedAt { get; set; }
        public string DeletedAt { get; set; }
    }

    // Chat Members tábla
    internal class ChatMemberDto
    {
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public string LastViewed { get; set; }
        public string JoinedAt { get; set; }
        public string ArchivedAt { get; set; }
    }

    // Groups tábla
    internal class GroupDto
    {
        public string Id { get; set; }
        public string CampId { get; set; }
        public string ChatId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string JoinCode { get; set; }
    }

    // Rooms tábla
    internal class RoomDto
    {
        public string Id { get; set; }
        public string CampId { get; set; }
        public string ChatId { get; set; }
        public string Name { get; set; }
        public string JoinCode { get; set; }
        public string Color { get; set; }
    }

    // Password Resets tábla
    internal class PasswordResetDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string ExpiresAt { get; set; }
    }

    // Tokens tábla
    internal class TokenDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string ClientDeviceType { get; set; }
    }

    // Locations tábla
    internal class LocationDto
    {
        public string UserId { get; set; }
        public string CampId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string LastUpdated { get; set; }
    }

    // Payments tábla
    internal class PaymentDto
    {
        public string Id { get; set; }
        public string CampId { get; set; }
        public string Name { get; set; }
        public string DueDate { get; set; }
        public int? Amount { get; set; }
        public string Currency { get; set; }
    }

    // User Payments tábla
    internal class UserPaymentDto
    {
        public string UserId { get; set; }
        public string PaymentId { get; set; }
        public bool IsPaid { get; set; }
    }

    // Teljes adatbázis wrapper
    internal class DatabaseDto
    {
        public List<UserDto> Users { get; set; }
        public List<UserOnlineStatusDto> UserOnlineStatus { get; set; }
        public List<CampDto> Camps { get; set; }
        public List<MemberToCampDto> MemberToCamp { get; set; }
        public List<ChatDto> Chats { get; set; }
        public List<MessageDto> Messages { get; set; }
        public List<ChatMemberDto> ChatMembers { get; set; }
        public List<GroupDto> Groups { get; set; }
        public List<RoomDto> Rooms { get; set; }
        public List<PasswordResetDto> PasswordResets { get; set; }
        public List<TokenDto> Tokens { get; set; }
        public List<LocationDto> Locations { get; set; }
        public List<PaymentDto> Payments { get; set; }
        public List<UserPaymentDto> UserPayments { get; set; }
    }
}