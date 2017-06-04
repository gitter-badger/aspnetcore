using LibrameStandard.Entity;
using LibrameStandard.Entity.Descriptors;
using LibrameStandard.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LibrameStandard.Tests.Entity
{
    public class EntityTests
    {
        [Fact]
        public void UseEntityTest()
        {
            var services = new ServiceCollection();
            
            var connectionString = "Data Source=PC-I74910MQ\\SQLEXPRESS;Initial Catalog=librame_test;Integrated Security=True";

            // Ĭ��ʹ�� SqlServerDbContext
            services.AddEntityFrameworkSqlServer().AddDbContext<SqlServerDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.UseRowNumberForPaging();
                    sql.MaxBatchSize(50);
                });
            });

            // ע�� Librame ���� MVC��Ĭ��ʹ���ڴ�����Դ��
            var builder = services.AddLibrameByMemory(options =>
            {
                // �޸�Ĭ�ϵ����ݿ�������������
                options[EntityAutomappingSetting.DbContextTypeNameKey]
                    = typeof(SqlServerDbContext).AsAssemblyQualifiedNameWithoutVCP();

                // �޸�Ĭ�ϵ�ʵ��ӳ�����
                options[EntityAutomappingSetting.AssembliesKey]
                    = TypeUtility.AsAssemblyName<Article>().Name;
            });

            // ��ȡʵ��������
            var adapter = builder.GetEntityAdapter();
            
            // ��ʼ������
            var article = new Article
            {
                Title = "Test Title",
                Descr = "Test Descr"
            };
            
            var repository = adapter.GetSqlServerRepository<Article>();

            // ���ⲻ���ظ�
            Article dbArticle;
            if (!repository.Exists(p => p.Title == article.Title, out dbArticle))
            {
                dbArticle = repository.Writer.Create(article);
            }
            else
            {
                article = dbArticle;
            }

            // �Ա�
            Assert.Equal(article.CreateTime, dbArticle.CreateTime);
        }
    }

    //[Table("Articles")]
    public class Article : AbstractCreateDataIdDescriptor<int>
    {
        public string Title { get; set; }
        
        public string Descr { get; set; }
    }

}
