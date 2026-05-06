using Domain.Abstractions;
using System.Security.Cryptography;

namespace Domain.Entites
{
    public sealed class Student : IAuditable
    {
        public int Id { get; set; }
        public string ParentEmail { get; set; } = string.Empty;
        public string? Grade { get; set; }
        public int XP { get; set; } = 0;
        public int Level { get; set; } = 1;
        public string? Goal { get; set; }
        public string? Semester { get; set; }
        public string? InviteCode { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public LessonView? LessonView { get; set; }
        public QuizAttempt? QuizAttempt { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LevelUpdatedAt { get; set; }
        public ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = [];
        public ICollection<SessionParticipant> SessionParticipants { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<StudentGrade> StudentGrades { get; set; } = [];
        public ICollection<StudentSubscription> StudentSubscriptions { get; set; } = [];
        public ICollection<StudentSubject> StudentSubjects { get; set; } = [];
        public StudentStreak StudentStreak { get; set; } = null!;
        public ICollection<Friend> Students1 { get; set; } = [];
        public ICollection<Friend> Students2 { get; set; } = [];
        public ICollection<Friend> Actions { get; set; } = [];

        public static Student Create(string userId, string parentEmail)
        {
            return new Student
            {
                UserId = userId,
                ParentEmail = parentEmail,
                InviteCode = GenerateInviteCode()
            };
        }
        private static string GenerateInviteCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return RandomNumberGenerator.GetString(chars, 6);
        }
        public void RegenerateInviteCode()
        {
            InviteCode = GenerateInviteCode();
        }
    }
}