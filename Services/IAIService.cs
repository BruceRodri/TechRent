namespace TechRent.Services
{
    public interface IAIService
    {
        Task<AIResult> GenerateAsync(string instruction, CancellationToken cancellationToken = default);
    }
}
