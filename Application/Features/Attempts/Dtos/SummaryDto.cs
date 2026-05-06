namespace Application.Features.Attempts.Dtos
{
    public sealed class SummaryDto
    {
        public int Correct { get; set; }
        public int Wrong { get; set; }
        public int Skipped { get; set; }
    }
}
