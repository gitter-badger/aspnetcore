using Librame.Entity;
using Librame.Entity.Descriptors;
using Librame.Entity.Providers;
using Librame.Entity.Repositories;
using Librame.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Librame.Tests.Entity
{
    public class EntityRepositoryTests
    {
        [Fact]
        public void UseEntityTest()
        {
            var services = new ServiceCollection();

            // ע��ʵ����
            var connectionString = "Data Source=(local);Initial Catalog=librame;Integrated Security=False;Persist Security Info=False;User ID=librame;Password=123456";
            services.AddEntityFrameworkSqlServer().AddDbContext<DbContextProvider>(options =>
            {
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.UseRowNumberForPaging();
                    sql.MaxBatchSize(50);
                });
            });

            // ע�� Librame ��Ĭ��ʹ���ڴ�����Դ��
            var builder = services.AddLibrameByMemory(source =>
            {
                // ����ʵ�����
                source[EntityOptions.AutomappingAssembliesKey]
                    = TypeUtil.GetAssemblyName<Article>().Name;
            })
            .UseEntity(); // ʹ��ʵ�幦��

            // ����ʵ��ֿ�
            var repository = builder.ServiceProvider.GetService<IRepository<Article>>();

            // ��ʼ������
            var article = new Article
            {
                Title = "Test Title",
                Descr = "Test Descr"
            };

            // ���ⲻ���ظ�
            Article dbArticle;
            if (!repository.Exists(p => p.Title == article.Title, out dbArticle))
            {
                dbArticle = repository.Create(article);
            }
            else
            {
                article = dbArticle;
            }

            // �Ա�
            Assert.Equal(article.CreateTime, dbArticle.CreateTime);
        }
    }

    
    public class Article : AbstractCreateDataIdDescriptor<int>
    {
        public string Title { get; set; }
        
        public string Descr { get; set; }
    }

}
