using LibrameStandard.Entity;
using LibrameStandard.Entity.DbContexts;
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
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<SqlServerDbContextReader>(options =>
                {
                    options.UseSqlServer(connectionString, sql =>
                    {
                        sql.UseRowNumberForPaging();
                        sql.MaxBatchSize(50);
                    });
                });
                //.AddDbContext<SqlServerDbContextWriter>(options =>
                //{
                //    options.UseSqlServer(connectionString, sql =>
                //    {
                //        sql.UseRowNumberForPaging();
                //        sql.MaxBatchSize(50);
                //    });
                //});

            // Ĭ��ʵ�����
            var defaultAssemblies = TypeUtility.AsAssemblyName<Article>().Name;

            // ע�� Librame ���� MVC��Ĭ��ʹ���ڴ�����Դ��
            services.AddLibrameByMemory(entityOptionsAction: opts =>
            {
                // �޸�Ĭ�ϵ����ݿ�������������
                opts.Automappings[0].DbContextTypeName
                    = typeof(SqlServerDbContextReader).AsAssemblyQualifiedNameWithoutVCP();
                opts.Automappings[1].DbContextTypeName
                    = typeof(SqlServerDbContextWriter).AsAssemblyQualifiedNameWithoutVCP();

                //// Ĭ�ϲ����ö�д����
                //options[EntityAdapterSettings.EnableReadWriteSeparationKey]
                //    = false.ToString();

                // �޸�Ĭ�ϵ�ʵ����򼯣���д��
                opts.Automappings[0].Assemblies = defaultAssemblies;
                opts.Automappings[1].Assemblies = defaultAssemblies;
            });

            var serviceProvider = services.BuildServiceProvider();

            // ��ʼ������
            var article = new Article
            {
                Title = "Test Title",
                Descr = "Test Descr"
            };

            //var repository = serviceProvider.GetLibrameRepository<SqlServerDbContextReader, Article>();
            var repository = serviceProvider.GetLibrameRepositoryReader<Article>();

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
