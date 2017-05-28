using LibrameCore.Entity;
using LibrameCore.Entity.Descriptors;
using LibrameCore.Entity.Providers;
using LibrameCore.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LibrameCore.Tests.Entity
{
    public class EntityTests
    {
        [Fact]
        public void UseEntityTest()
        {
            var services = new ServiceCollection();

            // ע��ʵ����
            var connectionString = "Server=(local);Database=LibrameTest;Trusted_Connection=True";
            
            // Ĭ��ʵ��ģ��ʹ�� DbContextProvider��Ҳ�ɸ���Ϊ�Լ�����Ҫ������ͬʱ�޸���������Դ
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
                // �޸�Ĭ�ϵ� DbContextProvider
                source[EntityOptions.EntityProviderTypeNameKey]
                    = typeof(DbContextProvider).AssemblyQualifiedNameWithoutVcp();

                // ����ʵ�����
                source[EntityOptions.AutomappingAssembliesKey]
                    = TypeUtil.GetAssemblyName<Article>().Name;
            });

            // ��ȡʵ��������
            var entity = builder.GetEntityAdapter();
            
            // ��ʼ������
            var article = new Article
            {
                Title = "Test Title",
                Descr = "Test Descr"
            };
            
            var repository = entity.GetRepository<Article>();

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
