using DTE_2802_1_25H_Oblig1_MVC.Models;

namespace DTE_2802_1_25H_Oblig1_MVC.Repositories
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(int id);
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(int id);
    }
}