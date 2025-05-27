using System;
using System.Collections.Generic;
using ParliamentaryVoter.Domain.Common;

namespace ParliamentaryVoter.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// User profile entity containing detailed user information
    /// This is an entity within the User aggregate
    /// </summary>
    public class UserProfile : BaseEntity
    {

        public Guid UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public DateTime? DateOfBirth { get; private set; }
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue)
                    return null;

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                
                if (DateOfBirth.Value.Date > today.AddYears(-age))
                    age--;
                
                return age;
            }
        }

        public Province Province { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? AvatarUrl { get; private set; }
        public bool EmailNotificationsEnabled { get; private set; }
        public bool SmsNotificationsEnabled { get; private set; }
        public List<string> PoliticalInterests { get; private set; }
        public VotingReminderFrequency VotingReminderFrequency { get; private set; }
        public bool PublicVotingHistory { get; private set; }
        public string TimeZone { get; private set; }

        private UserProfile()
        {
            PoliticalInterests = new List<string>();
            TimeZone = "America/Toronto"; // Default to Eastern Time
            VotingReminderFrequency = VotingReminderFrequency.Weekly;
        }

        public static UserProfile Create(
            Guid userId,
            string firstName,
            string lastName,
            Province province,
            string preferredLanguage = "en")
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be null or empty", nameof(firstName));
            
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));
            
            if (province == null)
                throw new ArgumentNullException(nameof(province));

            var profile = new UserProfile
            {
                UserId = userId,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Province = province,
                EmailNotificationsEnabled = true, // Default to enabled
                SmsNotificationsEnabled = false,  // Default to disabled
                PublicVotingHistory = false,      // Default to private
                PoliticalInterests = new List<string>(),
                TimeZone = GetDefaultTimeZoneForProvince(province)
            };

            return profile;
        }

        public void UpdateBasicInfo(string firstName, string lastName, Province province)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be null or empty", nameof(firstName));
            
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));
            
            if (province == null)
                throw new ArgumentNullException(nameof(province));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Province = province;
            
            MarkAsUpdated();
        }

        public void UpdateContactInfo(string? phoneNumber, string preferredLanguage, string timeZone)
        {
            if (string.IsNullOrWhiteSpace(preferredLanguage))
                throw new ArgumentException("Preferred language cannot be null or empty", nameof(preferredLanguage));
            
            if (string.IsNullOrWhiteSpace(timeZone))
                throw new ArgumentException("Timezone cannot be null or empty", nameof(timeZone));

            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
            TimeZone = timeZone.Trim();
            
            MarkAsUpdated();
        }

        public void UpdateNotificationPreferences(
            bool emailNotifications,
            bool smsNotifications,
            VotingReminderFrequency votingReminderFrequency)
        {
            EmailNotificationsEnabled = emailNotifications;
            SmsNotificationsEnabled = smsNotifications;
            VotingReminderFrequency = votingReminderFrequency;
            
            MarkAsUpdated();
        }

        public void UpdateProfileDetails(DateTime? dateOfBirth, string? avatarUrl)
        {
            // Validate date of birth
            if (dateOfBirth.HasValue)
            {
                if (dateOfBirth.Value > DateTime.Today)
                    throw new ArgumentException("Date of birth cannot be in the future", nameof(dateOfBirth));
                
                if (dateOfBirth.Value < DateTime.Today.AddYears(-120))
                    throw new ArgumentException("Date of birth is too far in the past", nameof(dateOfBirth));
            }

            DateOfBirth = dateOfBirth;
            AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();
            
            MarkAsUpdated();
        }

        public void UpdatePoliticalInterests(IEnumerable<string> interests)
        {
            if (interests == null)
                throw new ArgumentNullException(nameof(interests));

            PoliticalInterests.Clear();
            
            foreach (var interest in interests)
            {
                if (!string.IsNullOrWhiteSpace(interest))
                {
                    PoliticalInterests.Add(interest.Trim());
                }
            }
            
            MarkAsUpdated();
        }

        public void SetVotingHistoryVisibility(bool isPublic)
        {
            PublicVotingHistory = isPublic;
            MarkAsUpdated();
        }

        public bool IsEligibleToVote()
        {
            return Age >= 18;
        }

        private static string GetDefaultTimeZoneForProvince(Province province)
        {
            return province.Code switch
            {
                "BC" or "YT" => "America/Vancouver",
                "AB" or "NT" => "America/Edmonton", 
                "SK" => "America/Regina",
                "MB" => "America/Winnipeg",
                "ON" or "NU" => "America/Toronto",
                "QC" => "America/Montreal",
                "NB" or "NS" or "PE" => "America/Halifax",
                "NL" => "America/St_Johns",
                _ => "America/Toronto" // Default to Eastern
            };
        }

        public bool IsCompleteForVoting()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   Province != null &&
                   (Age == null || Age >= 18); // If age is unknown, allow voting (will be checked elsewhere)
        }
    }


    public enum VotingReminderFrequency
    {
        Never = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        OnNewBillsOnly = 4
    }
}