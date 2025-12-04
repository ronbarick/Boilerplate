namespace Project.Domain.Interfaces;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
