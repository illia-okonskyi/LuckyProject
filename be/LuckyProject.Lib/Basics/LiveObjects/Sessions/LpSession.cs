using System;

namespace LuckyProject.Lib.Basics.LiveObjects.Sessions
{
    public interface ILpSession<TContext>
        where TContext : class, new()
    {
        Guid Id { get; }
        DateTime CreatedAtUtc { get; }
        DateTime? ExpiresAtUtc { get; set; }
        TContext Context { get; }
    }

    public class LpSession<TContext> : ILpSession<TContext>
        where TContext : class, new()
    {
        public Guid Id { get; }
        public DateTime CreatedAtUtc { get; }
        public DateTime? ExpiresAtUtc { get; set; }
        public TContext Context { get; }

        public LpSession(DateTime createdAtUtc, TContext context)
        {
            Id = Guid.NewGuid();
            CreatedAtUtc = createdAtUtc;
            Context = context;
        }

        public LpSession(DateTime createdAtUtc, TContext context, TimeSpan expiration)
            : this(createdAtUtc, context)
        {
            ExpiresAtUtc = CreatedAtUtc + expiration;
        }
    }
}
